async function checkToken() {
    await fetchWithCredentials('http://localhost:5000/auth/checktoken', {
        redirectOnUnauthorized: true,
        onSuccess: async (response) => {
            window.user = await response.json()
            const event = new CustomEvent('userLoaded', {
                detail: window.user
            })
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
            return failHandler(refreshResponse)
        }
        const secondResponse = await fetch(url, {
            credentials: 'include',
            ...options
        })
        if (secondResponse.ok) {
            return successHandler(firstResponse)
        }
        return failHandler(secondResponse)
    }
    return failHandler(firstResponse)
}