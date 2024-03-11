const productName = $('#productName')
const bar = $('#barCode')
const lastModified = $('#lastModified')
const price = $('#price')
async function showProduct() {
    const url = window.location.href
    const productId = url
        .substring(url.lastIndexOf('/') + 1)
        .replace(/\D/g, '')
    $.get(`http://localhost:5000/Product/${productId}`)
        .done(result => {
            productName.text(result.productName)
            bar.text(result.barCode)
            lastModified.text(result.lastModifiedTime.split('T')[0])
            price.text('$' + result.price)
        })
        .fail(err => {
            console.log(err)
        })
}

showProduct()