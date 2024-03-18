const initialList = Array(10).fill(undefined).map((item, index) => ({
    cusId: index + 1,
    cusCode: 'KH1',
    cusName: 'Pham Thanh',
    address: 'Ha Noi',
    TaxCode: '0110611386',
    email: 'pthanh@gmail.com',
    phone: '0393797828',
    CustomerTypeId: 1,
    CustomerGroupId: 1
}))
const options = {
    valueNames: [
        'cusName',
        'cusCode',
        'email',
        'phone',
        'address',
        {
            data: ['cusId'],
        }
    ],
    page: 10
}
const customerList = new List('customerList', options)


customerList.clear()
customerList.add(initialList, onLoadCustomerList)

function onLoadCustomerList(items) {
    items.forEach(item => {
        $(item.elm).find('.cusCode, .cusName').each((_, anchor) => {
            anchor.href += `/${item.values().cusId}`
        })
    })
}