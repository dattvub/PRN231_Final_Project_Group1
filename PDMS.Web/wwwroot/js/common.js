const cart = {}

;(() => {
    try {
        const tmp = JSON.parse(localStorage.getItem('cart'))
        Object.assign(cart, tmp)
    } catch (e) {
        Object.keys(cart).forEach(key => {
            delete cart[key];
        })
    }
})()
setCartIconQuantity(Object.keys(cart).length)

function setCartIconQuantity(quantity) {
    const cartIcon = document.querySelector('#cart-btn > a')
    const cartIndicator = document.querySelector('#cart-indicator')
    if (cartIcon) {
        if (quantity > 0) {
            cartIcon.classList.add('notification-indicator', 'notification-indicator-warning', 'notification-indicator-fill')
        } else if (quantity == 0) {
            cartIcon.classList.remove('notification-indicator', 'notification-indicator-warning', 'notification-indicator-fill')
        }
    }
    if (cartIndicator) {
        cartIndicator.textContent = quantity > 0 ? quantity : ''
    }
}

async function checkToken() {
    const response = await fetchWithCredentials('http://localhost:5000/auth/checktoken', {
        redirectOnUnauthorized: true
    })
    window.user = await response.json()
    document.documentElement.classList.add(window.user.role)
    if (window.acceptRoles && !window.acceptRoles.includes(window.user.role)) {
        window.location.replace(document.getElementById('login-route').href)
    }
    const event = new CustomEvent('userLoaded', {
        detail: window.user
    })
    document.getElementById('top').classList.remove('d-none')
    window.dispatchEvent(event)
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
            const loginUrl = new URL(document.getElementById('login-route').href)
            loginUrl.searchParams.set('returnUrl', location.pathname)
            window.location.replace(loginUrl)
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

/**
 * @param {number} productId
 * @param {number} quantity
 */
function addToCart(productId, quantity) {
    if (cart.hasOwnProperty(productId)) {
        cart[productId] += quantity
    } else {
        cart[productId] = quantity
    }
    localStorage.setItem('cart', JSON.stringify(cart))
    setCartIconQuantity(Object.keys(cart).length)
    $('.total-products').text(Object.keys(cart).length)
}

function updateCartItem(productId, quantity) {
    if (!cart.hasOwnProperty(productId)) {
        return
    }
    cart[productId] = quantity
    localStorage.setItem('cart', JSON.stringify(cart))
    setCartIconQuantity(Object.keys(cart).length)
}

function removeFromCart(productId) {
    if (!cart.hasOwnProperty(productId)) {
        return
    }
    delete cart[productId]
    localStorage.setItem('cart', JSON.stringify(cart))
    const cartCount = Object.keys(cart).length
    setCartIconQuantity(cartCount)
    $('.total-products').text(cartCount)
}

function removeAllFromCart() {
    Object.keys(cart).forEach(key => {
        delete cart[key];
    })
    localStorage.setItem('cart', JSON.stringify(cart))
    setCartIconQuantity(Object.keys(cart).length)
    $('.total-products').text(Object.keys(cart).length)
}

function getOrderStatusText(status) {
    switch (status) {
        case 'Pending':
            return 'Đang chờ xử lý'
        case 'Approved':
            return 'Đã chấp nhận'
        case 'Rejected':
            return 'Bị từ chối'
        case 'Received':
            return 'Đã nhận hàng'
        case 'Cancel':
            return 'Đã Huỷ'
        default:
            return undefined
    }
}