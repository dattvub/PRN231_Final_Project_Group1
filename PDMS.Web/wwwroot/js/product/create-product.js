
const productName = $('#productName')
const importPrice = $('#importPrice')
const price = $('#price')
const bar = $('#bar')
const createdBy = $('#createdBy')
const lastModified = $('#lastModified')
const image = $('#image')
const brand = $('#brand')
const supplier = $('#supplier')
const major = $('#major')

const productform = $('#productForm')
async function newProduct() {
    productform.submit((e) => {
        e.preventDefault()
        const data = {
            productName: productName.val().trim(),
            importPrice: importPrice.val(),
            price: price.val(),
            barCode: bar.val().trim(),
            createdById: createdBy.val(),
            lastModifiedById: lastModified.val(),
            image: "string",
            brandId: brand.val(),
            suppilerId: supplier.val(),
            majorId: major.val()
        }

        $.ajax({
            type: 'POST',
            url: `http://localhost:5000/Product/create`,
            dataType: 'json',
            contentType: 'application/json',
            data: JSON.stringify(data)
        })
            .done(result => 
                console.log(result))
            .fail(err => {
                console.log(err)
            })

    })
}

newProduct()