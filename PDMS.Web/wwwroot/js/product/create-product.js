
const productName = $('#productName')
const importPrice = $('#importPrice')
const price = $('#price')
const bar = $('#bar')
const createdBy = $('#createdBy')
const lastModified = $('#lastModified')
const image = $('#image')
let selectedImage = []

var binaryDataList = [];


const brandSelect = new Choices('#brand', {
    placeholder: true,
    allowHTML: false,
    itemSelectText: 'Nhấn để chọn',
    noResultsText: 'Không có kết quả',
    noChoicesText: 'Không có lựa chọn'
})

const supplierSelect = new Choices('#supplier', {
    placeholder: true,
    allowHTML: false,
    itemSelectText: 'Nhấn để chọn',
    noResultsText: 'Không có kết quả',
    noChoicesText: 'Không có lựa chọn'
})

const majorSelect = new Choices('#major', {
    placeholder: true,
    allowHTML: false,
    itemSelectText: 'Nhấn để chọn',
    noResultsText: 'Không có kết quả',
    noChoicesText: 'Không có lựa chọn'
})

const quillDescription = new Quill('#quillEditor', {
        theme: 'snow',
        "placeholder": "Type your description...",
        "modules": {
            "toolbar": [
                ["bold", "italic", "underline", "strike", "link", "image", "blockquote", "code", { "list": "bullet" }]
            ]
    }
})

const supplier = $('#supplier')
const major = $('#major')
const productform = $('#productForm')
const saveProduct = $('#saveProduct')

supplierSelect.setChoices(async () => {
    try {
        const r = await fetchWithCredentials('http://localhost:5000/Supplier/list')
        const fullItems = await r.json()
        return Array.prototype.map.call(fullItems.items, x => ({
            value: x.supplierId,
            label: x.supplierName
        })) 
    } catch (err) {
        console.error(err);
    }
});

brandSelect.setChoices(async () => {
    try {
        const r = await fetchWithCredentials('http://localhost:5000/Brand/list')
        const fullItems = await r.json()
        return Array.prototype.map.call(fullItems.items, x => ({
            value: x.brandId,
            label: x.brandName
        }))
    } catch (err) {
        console.error(err);
    }
});

majorSelect.setChoices(async () => {
    try {
        const r = await fetchWithCredentials('http://localhost:5000/Major/ListMajor')
        const fullItems = await r.json()
        return Array.prototype.map.call(fullItems.items, x => ({
            value: x.majorId,
            label: x.majorName
        }))
    } catch (err) {
        console.error(err);
    }
});
async function newProduct() {
    productform.submit((e) => {
        e.preventDefault()
        e.stopPropagation()

        const form = new FormData()
        form.set('productName', productName.val().trim())
        form.set('importPrice', importPrice.val())
        form.set('price', price.val())
        form.set('barCode', bar.val().trim())

        for (const file of Dropzone.forElement('#attachFilesNewProjectLabel').getAcceptedFiles()) {
            form.append('images', file)
        }

        form.set('description', quillDescription.getText())
        form.set('brandId', brandSelect.getValue().value)
        form.set('suppilerId', supplierSelect.getValue().value)
        form.set('majorId', majorSelect.getValue().value)
        //const data = {
        //    productName: productName.val().trim(),
        //    importPrice: importPrice.val(),
        //    price: price.val(),
        //    barCode: bar.val().trim(),
        //    createdById: createdBy.val(),
        //    lastModifiedById: lastModified.val(),
        //    image: "string",
        //    description: quillDescription.getText(),
        //    brandId: brandSelect.getValue().value,
        //    suppilerId: supplierSelect.getValue().value,
        //    majorId: majorSelect.getValue().value
        //}

        const result = fetchWithCredentials('http://localhost:5000/Product/create',
            {
                method: 'POST',
                //headers: {
                //    "Content-Type": "application/json",
                //},
                //body: JSON.stringify(data)
                body: form
            })
        console.log(result)
        if (result.ok) {
            window.location.href = '/Product'
        }
    })
}
//setTimeout(c => {
//            Dropzone.forElement('#attachFilesNewProjectLabel')
//                .on('addedfile', file => {
//                    selectedImage.push(file) 
//                    console.log("A file has been added")
//                })
//}, 1000);

$(window).on({
    dropzoneInit: () => {
        console.log(Dropzone)
        
        Dropzone.forElement('#attachFilesNewProjectLabel')
            .on('addedfile', file => {
                selectedImage.push(file)
                console.log("A file has been added")
            })
    }
})

newProduct()








