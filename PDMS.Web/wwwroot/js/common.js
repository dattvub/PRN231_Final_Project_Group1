﻿async function checkToken() {
    await fetchWithCredentials('http://localhost:5000/auth/checktoken', {
        redirectOnUnauthorized: true,
        onSuccess: async (response) => {
            window.user = await response.json()
            if (window.acceptRoles && !window.acceptRoles.includes(window.user.role)) {
                window.location.replace(document.getElementById('login-route').href)
            }
            const event = new CustomEvent('userLoaded', {
                detail: window.user
            })
            document.getElementById('top').classList.remove('d-none')
            window.dispatchEvent(event)
        }
    })
}

/**
 * @typedef FetchWithCredentialsEvents
 * @type {Object}
 * @property {boolean} autoRefreshToken
 * @property {boolean} redirectOnUnauthorized
 * @property {(Response) => void} onFail
 * @property {(Response) => void} onSuccess
 */

/**
 * @param {RequestInfo | URL} url
 * @param {RequestInit & FetchWithCredentialsEvents} options
 * @returns {Promise<Response>}
 */
async function fetchWithCredentials(url, options = {}) {
    const {autoRefreshToken = true, redirectOnUnauthorized = false, onSuccess, onFail, ...otherOptions} = options
    const firstResponse = await fetch(url, {
        credentials: 'include',
        ...options
    })

    /**
     * @param {Response} response
     * @returns {Response}
     */
    function successHandler(response) {
        if (onSuccess) {
            onSuccess(response)
        }
        return response
    }

    /**
     * @param {Response} response
     * @returns {Response}
     */
    function failHandler(response) {
        if (response.status === 401 && redirectOnUnauthorized) {
            window.location.replace(document.getElementById('login-route').href)
        } else if (onFail) {
            onFail(response)
        }
        return response
    }

    if (firstResponse.ok) {
        return successHandler(firstResponse)
    }
    if (firstResponse.status === 401 && autoRefreshToken) {
        const refreshResponse = await fetch('http://localhost:5000/auth/refresh', {
            credentials: 'include',
        })
        if (!refreshResponse.ok) {
            return failHandler(firstResponse)
        }
        const secondResponse = await fetch(url, {
            credentials: 'include',
            ...options
        })
        if (secondResponse.ok) {
            return successHandler(secondResponse)
        }
        return failHandler(secondResponse)
    }
    return failHandler(firstResponse)
}

/**
 * @param {String} title
 * @param {String} content
 * @returns {void}
 */
function showToast(title, content) {
    const toastElm = $('<div class="toast"></div>').append([
        $('<div class="toast-header"></div>').append([
            $('<div class="me-2"></div>').append(
                $('<i class="iconify-inline" data-icon="ooui:notice"></i>')
            ),
            $('<strong class="me-auto"></strong>').text(title),
            $('<button type="button" class="btn-close m-0" data-bs-dismiss="toast" aria-label="Close"></button>')
        ]),
        $('<div class="toast-body"></div>').text(content)
    ]).one('hidden.bs.toast', () => {
        bootstrap.Toast.getOrCreateInstance(toastElm)?.dispose()
        toastElm.remove()
    })
    $('#toast-container')?.prepend(toastElm)
    bootstrap.Toast.getOrCreateInstance(toastElm).show()
}

/**
 * @param {{route: string,href: string} | string} routes
 */
function setBreadcrumb(...routes) {
    $('#breadcrumb > ol.breadcrumb').html('').append(
        routes.map(route => {
            if (typeof route === 'string') {
                return $('<li class="breadcrumb-item active cursor-default" aria-current="page"></li>').text(route)
            }
            return $('<li class="breadcrumb-item"></li>').append(
                $(`<a href="${route.href}"></a>`).text(route.route)
            )
        })
    )
}