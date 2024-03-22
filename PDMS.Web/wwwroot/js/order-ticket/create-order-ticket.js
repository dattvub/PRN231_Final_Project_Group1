const flatpickrConfig = {
    disableMobile: true,
    dateFormat: 'd-m-Y',
    locale: 'vn',
}
const expectedOrderDate = $('#expected-order-date').flatpickr({
    ...flatpickrConfig,
    onChange: e => {
        const date = e[0] ? flatpickr.formatDate(e[0], 'd/m/Y') : ''
        $('#expected-order-date-summary').text(date)
    }
})
const expectedReceiveDate = $('#expected-receive-date').flatpickr({
    ...flatpickrConfig,
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

fetchWithCredentials(`http://localhost:5000/Customer/${window.user.associationId}`, {
    onSuccess: async r => {
        console.log(await r.json())
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
    console.log('submited')
})