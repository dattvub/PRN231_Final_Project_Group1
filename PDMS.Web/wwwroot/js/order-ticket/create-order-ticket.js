const flatpickrConfig = {
    disableMobile: true,
    dateFormat: 'd-m-Y',
    locale: 'vn',
}
const expectedOrderDate = $('#expected-order-date').flatpickr({
    ...flatpickrConfig,
    minDate: new Date(),
    onChange: e => {
        const date = e[0] ? flatpickr.formatDate(e[0], 'd/m/Y') : ''
        $('#expected-order-date-summary').text(date)
        expectedReceiveDate.config.minDate = e[0] || new Date()
        if (!expectedReceiveDate.selectedDates[0]) {
            $('#expected-receive-date-summary').text('')
        }
    }
})
const expectedReceiveDate = $('#expected-receive-date').flatpickr({
    ...flatpickrConfig,
    minDate: new Date(),
    onChange: e => {
        const date = e[0] ? flatpickr.formatDate(e[0], 'd/m/Y') : ''
        $('#expected-receive-date-summary').text(date)
    }
})
const options = {
    valueNames: [
        'productName',
        'productCode',
        'quantity',
        'totalPrice',
        {
            name: 'image',
            attr: 'src'
        },
        {
            data: ['productId'],
        }
    ]
}
const productList = new List('product-list', options)
let products
let totalPrice = 0
let oldAddress

productList.clear()

setBreadcrumb(
    {
        route: 'Trang chủ',
        href: '/'
    },
    {
        route: 'Giỏ hàng',
        href: '/Cart'
    },
    'Tạo phiếu đặt hàng'
)

if (data.length > 0) {
    fetchWithCredentials(`http://localhost:5000/Product/list?${data.map(x => `OnlyIn=${x.ProductId}`).join('&')}`, {
        onSuccess: async r => {
            const data = await r.json()
            products = data.items.map(x => ({
                productId: x.productId,
                productCode: x.productCode,
                productName: x.productName,
                quantity: cart[x.productId],
                totalPrice: String(x.price * cart[x.productId]).replace(/\B(?=(\d{3})+(?!\d))/g, ','),
                totalPriceNumber: x.price * cart[x.productId],
                image: `http://localhost:5000/${JSON.parse(x.image)[0]}`
            }))
            productList.clear()
            productList.add(products)
            totalPrice = products.reduce((prev, cur) => prev + cur.totalPriceNumber, 0)
            $('#total-price').text(String(totalPrice).replace(/\B(?=(\d{3})+(?!\d))/g, ','))
        }
    })
} else {
    location.replace('/Cart')
}

fetchWithCredentials(`http://localhost:5000/Employee/ByCustomer`, {
    onSuccess: async r => {
        const emp = await r.json()
        $('#emp-img').attr('src', emp.imageUrl ? `http://localhost:5000/${emp.imageUrl}` : '/images/blank_avatar.jpg')
        $('#emp-name').text(emp.empName)
        $('#emp-position').text(emp.position)
        console.log(emp)
    }
})

$('#expected-order-date-clear').on('click', e => {
    e.stopPropagation()
    e.preventDefault()
    expectedOrderDate?.clear()
})
$('#expected-receive-date-clear').on('click', e => {
    e.stopPropagation()
    e.preventDefault()
    expectedReceiveDate?.clear()
})
$('#create-btn').on('click', () => {
    const address = $('#address').val().trim()
    if (!address) {
        showToast('Dữ liệu không hợp lệ', 'Địa chỉ nhận hàng không được để trống')
    }
    const formData = new FormData()
    formData.set('totalPay', String(totalPrice))
    formData.set('address', address)

    let orderDate = expectedOrderDate.selectedDates[0]
    let receiveDate = expectedReceiveDate.selectedDates[0]

    if (orderDate) {
        orderDate = new Date(orderDate)
        orderDate.setMinutes(orderDate.getMinutes() - orderDate.getTimezoneOffset())
        formData.set('expectedOrderDate', orderDate.toJSON())
    }
    if (receiveDate) {
        receiveDate = new Date(receiveDate)
        receiveDate.setMinutes(receiveDate.getMinutes() - receiveDate.getTimezoneOffset())
        formData.set('expectedReceiveDate', receiveDate.toJSON())
    }

    const note = $('#note').val().trim()
    if (note) {
        formData.set('note', note)
    }

    data.forEach((x, i) => {
        formData.set(`cartItems[${i}].productId`, x.ProductId)
        formData.set(`cartItems[${i}].quantity`, x.Quantity)
    })

    fetchWithCredentials('http://localhost:5000/OrderTicket/Create', {
        method: 'POST',
        body: formData,
        onSuccess: () => {
            removeAllFromCart()
            location.replace('/OrderTicket')
        },
        onFail: async r => {
            const json = await r.json()
            if (json.errors) {
                showToast('Đã xảy ra lỗi', json.errors.join('. '))
            } else {
                showToast('Đã xảy ra lỗi', 'Đã xảy ra lỗi trong quá trình tạo phiếu đặt hàng')
            }
        }
    })
})
$('#edit-address').on('click', () => {
    const addressInput = $('#address')[0]
    addressInput.readOnly = !addressInput.readOnly
    $('#edit-address').text(addressInput.readOnly ? 'Sửa' : 'Hoàn tác')
    if (addressInput.readOnly) {
        addressInput.value = oldAddress
        $('#address-summary').text(addressInput.value)
    } else {
        addressInput.focus()
    }
})
$('#address').on('input', e => {
    $('#address-summary').text(e.delegateTarget.value)
})
$('#note').on('input', e => {
    $('#note-summary').text(e.delegateTarget.value)
})

;(async () => {
    await waitCheckToken
    fetchWithCredentials(`http://localhost:5000/Customer/${window.user.associationId}`, {
        onSuccess: async r => {
            const customer = await r.json()
            oldAddress = customer.address
            $('#cus-lastname').val(customer.lastName)
            $('#cus-firstname').val(customer.firstName)
            $('#phone').val(customer.phoneNumber)
            $('#email').val(customer.email)
            $('#address').val(customer.address)
            $('#cus-name-summary').text(`${customer.lastName} ${customer.firstName}`.trim())
            $('#email-summary').text(customer.email)
            $('#phone-summary').text(customer.phoneNumber)
            $('#address-summary').text(customer.address)
        }
    })
})()