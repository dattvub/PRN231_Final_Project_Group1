const options = {
    valueNames: [
        'brandCode',
        'brandName'
    ],
    page: 10
}
const initValues = [
    {
        brandId: 1,
        brandCode: 'ABC',
        brandName: 'Pfizer',
        status: true,
    },
    {
        brandId: 2,
        brandCode: 'ABC',
        brandName: 'AbbVie',
        status: true,
    },
    {
        brandId: 3,
        brandCode: 'ABC',
        brandName: 'Johnson & Johnson',
        status: true,
    },
    {
        brandId: 4,
        brandCode: 'ABC',
        brandName: 'Merck & Co',
        status: true,
    },
    {
        brandId: 5,
        brandCode: 'ABC',
        brandName: 'Novartis',
        status: true,
    },
    {
        brandId: 6,
        brandCode: 'ABC',
        brandName: 'Roche',
        status: true,
    },
    {
        brandId: 7,
        brandCode: 'ABC',
        brandName: 'Bristol-Myers Squibb',
        status: true,
    },
    {
        brandId: 8,
        brandCode: 'ABC',
        brandName: 'Sanofi',
        status: true,
    },
    {
        brandId: 9,
        brandCode: 'ABC',
        brandName: 'AstraZeneca',
        status: true,
    },
    {
        brandId: 10,
        brandCode: 'ABC',
        brandName: 'GSK',
        status: true,
    },
]
const formTitle = $('#formTitle')
const defaultFormTitle = formTitle.text()
const brandList = new List('brandList', options)
const mainCard = $('#mainCard')
const newBtnGroup = $('#newBtnGroup')
const brandForm = $('#brandForm')
const brandCodeInput = $('#brandCode')
let newOpened = mainCard.hasClass('newOpened');

function formClear() {
    brandForm.removeClass('was-validated')
    $('#brandId').val('')
    brandForm[0].reset()
}

function toggleNewForm() {
    newOpened = !newOpened
    if (!newOpened) {
        formClear()
        mainCard.removeClass('updating')
        formTitle.text(defaultFormTitle)
        $('#brandList').find('.actions > button').removeClass('disabled')
    }
    mainCard.toggleClass('newOpened', newOpened)
}

function loadBrandIntoForm(brand) {
    $('#brandId').val(brand.brandId)
    brandCodeInput.val(brand.brandCode)
    $('#brandName').val(brand.brandName)
}

brandCodeInput.on('keypress', e => {
    const key = String.fromCharCode(e.charCode || e.which)
    if (!/^[a-zA-Z0-9]+$/.test(key)) {
        e.preventDefault()
        return false
    }
}).on('change', e => {
    brandCodeInput.val(brandCodeInput.val().replace(/[^a-zA-Z0-9]+/g, ''))
})

brandList.clear()
brandList.add(initValues, (items) => {
    items.forEach(item => {
        $(item.elm).find('.edit-btn').on('click', () => {
            const brandIdElm = $('#brandId')

            if (newOpened && !brandIdElm.val()) {
                return
            }

            $('#formTitle').text(`Cập nhật Brand: ${item.values().brandName}`)
            loadBrandIntoForm(item.values())
            $('#brandList').find('.delete-btn').addClass('disabled')
            if (!newOpened) {
                mainCard.addClass('updating')
                toggleNewForm()
            }
        })

        $(item.elm).find('.delete-btn').on('click', () => {
            brandList.remove('brandId', item.values().brandId)
        })
    })
})

newBtnGroup.on('click', () => {
    $('#brandList').find('.actions > button').addClass('disabled')
    toggleNewForm()
})

brandForm.submit((e) => {
    e.preventDefault()
    e.stopPropagation()
    if (brandForm.find(':invalid').length > 0) {
        brandForm.addClass('was-validated')
        return
    }
    // toggleNewForm()
    formClear()
})