const options = {
    valueNames: [
        'productName',
        'unitPrice',
        {
            name: 'quantity',
            attr: 'value'
        },
        'totalPrice',
        {
            data: ['productId'],
        }
    ]
}
const cartItemList = new List('cart-items-table', options)
cartItemList.clear()
cartItemList.on('updated', e => {
    if (!addingItem) {
        renderItemsSum()
    }
})
let addingItem = false

setBreadcrumb(
    {
        route: 'Trang chủ',
        href: '/'
    },
    'Giỏ hàng'
)

if (Object.keys(cart).length > 0) {
    $('.total-products').text(Object.keys(cart).length)
    fetchWithCredentials(`http://localhost:5000/Product/list?${Object.keys(cart).map(x => `OnlyIn=${x}`).join('&')}`, {
        onSuccess: async r => {
            const data = await r.json()
            const products = data.items.map(x => ({
                productId: x.productId,
                productName: x.productName,
                quantity: cart[x.productId],
                unitPrice: String(x.price).replace(/\B(?=(\d{3})+(?!\d))/g, ','),
                unitPriceNumber: x.price,
                totalPrice: String(x.price * cart[x.productId]).replace(/\B(?=(\d{3})+(?!\d))/g, ','),
                totalPriceNumber: x.price * cart[x.productId],
                images: JSON.parse(x.image)
            }))
            addingItem = true
            cartItemList.add(products, onCartLoad)
        }
    })
} else {
    $('.content').append($('<h2 class="h2 text-center my-5">Giỏ hàng trống, vui lòng thêm sản phẩm</h2>'))
}

function onCartLoad(e) {
    e.forEach(item => {
        const itemValues = item.values()
        if (itemValues.images.length > 0) {
            $(item.elm)
                .find('img')
                .prop('src', `http://localhost:5000/${itemValues.images[0]}`)
                .end()
                .find('.prod-url')
                .attr('href', `/Category/Product/${itemValues.productId}`)
                .end()
                .find('input.quantity')
                .on('change', () => {
                    console.log('change')
                })
                .prev()
                .on('click', e => {
                    itemValues.quantity = Math.max(itemValues.quantity - 1, 1)
                    onQuantityChange(item)
                })
                .end()
                .next()
                .on('click', e => {
                    itemValues.quantity += 1
                    onQuantityChange(item)
                })
                .end()
                .end()
                .find('.remove-btn')
                .on('click', e => {
                    e.stopPropagation()
                    e.preventDefault()
                    removeFromCart(itemValues.productId)
                    cartItemList.remove('productId', itemValues.productId)
                    if (Object.keys(cart).length === 0) {
                        cartItemList.clear()
                        $('.content').append($('<h2 class="h2 text-center my-5">Giỏ hàng trống, vui lòng thêm sản phẩm</h2>'))
                    }
                })
        }
    })
    renderItemsSum()
    addingItem = false
}

function onQuantityChange(item) {
    const itemValues = item.values()
    updateCartItem(itemValues.productId, itemValues.quantity)
    itemValues.totalPriceNumber = itemValues.quantity * itemValues.unitPriceNumber
    itemValues.totalPrice = String(itemValues.totalPriceNumber).replace(/\B(?=(\d{3})+(?!\d))/g, ',')
    item.values(itemValues)
    $(item.elm).find('input.quantity').val(itemValues.quantity)
    const totalPrice = cartItemList.items.map(x => x.values().totalPriceNumber).reduce((prev, cur) => prev + cur, 0)
    $('#total-price').text(String(totalPrice).replace(/\B(?=(\d{3})+(?!\d))/g, ','))
}

function renderItemsSum() {
    const totalPrice = cartItemList.items.map(x => x.values().totalPriceNumber).reduce((prev, cur) => prev + cur, 0)
    $(cartItemList.list).append(
        $('<tr class="fw-bold"></tr>').append([
            $('<td colspan="2" class="text-end pt-2 pe-2 pb-2">Tổng cộng</td>'),
            $('<td class="text-center p-2"></td>').append([
                $('<span class="total-products"></span>').text(Object.keys(cart).length),
                document.createTextNode(' sản phẩm')
            ]),
            $('<td class="text-end pt-2 ps-2 pb-2"></td>').append([
                $('<span id="total-price"></span>').text(String(totalPrice).replace(/\B(?=(\d{3})+(?!\d))/g, ',')),
                $('<span class="text-decoration-underline fs--1">đ</span>')
            ])
        ])
    )
}