
const productName = $('#productName')
const importPrice = $('#importPrice')
const price = $('#price')
const bar = $('#bar')
const image = $('#image')
let selectedImage = []


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

productform.on('submit', async e => {
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

    const result = await fetchWithCredentials('http://localhost:5000/Product/create',
        {
            method: 'POST',
            body: form
        })
    console.log(result)
    if (result.ok) {
        showToast('Thêm sản phẩm', 'Thêm sản phẩm thành công')
        window.location.replace(`http://localhost:5238/Category/Product`) 
    }
    else {
        if (!result.ok) {
            const json = await result.json()
            showToast('Thêm sản phẩm', json.errors.join('. '))
        }
    }
})
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









