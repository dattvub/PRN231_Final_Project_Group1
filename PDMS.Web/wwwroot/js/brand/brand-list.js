const options = {
    valueNames: [
        'brandCode',
        'brandName',
        {
            data: ['brandId'],
        }
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
const brandIdInput = $('#brandId')
const brandCodeInput = $('#brandCode')
const brandNameInput = $('#brandName')
/** @type {[{id: number, type: string}]} */
const highlightList = []
let newOpened = mainCard.hasClass('newOpened');
let currentPage = 1
let currentTotalPage = 1
let searchKeyword = ''
let searchTimer
let handleByPaste = false

async function loadBrands(page, search) {
    page = page || 1
    const params = new URLSearchParams()
    params.append('page', page)
    params.append('quantity', '10')
    if (search && search.trim().length > 0) {
        params.append('query', search)
        params.append('queryByName', $('input[name=searchBy]:checked').val())
    }
    $.get(`http://localhost:5000/Brand/list?${params}`)
        .done(result => {
            currentPage = page
            $('#mainGrid tbody .delete-btn').each((i, elm) => {
                bootstrap.Popover.getInstance(elm).dispose()
            })
            brandList.clear()
            brandList.add(result.items, onLoadBrandList)
            const totalPage = Math.max(Math.ceil(result.total / result.itemsPerPage), 1)
            currentTotalPage = totalPage
            if (page > totalPage) {
                page = totalPage
                loadBrands(page, search)
                return
            }
            renderPaginationNumber(currentPage, totalPage)
        })
        .fail(err => {
            console.log(err)
        })
}

function renderPaginationNumber(active, totalPage) {
    if (active < 1 || active > totalPage) {
        return
    }

    let limit = 6, lower = active, upper = active
    while (limit > 1 && (lower > 1 || upper < totalPage)) {
        if (lower > 1) {
            lower--;
            limit--;
        }
        if (limit <= 1) {
            break;
        }
        if (upper < totalPage) {
            upper++;
            limit--;
        }
    }

    const paginationGroup = $('#paginationBar > .pagination-number-group')
    paginationGroup.html('')

    for (let i = lower; i <= upper; i++) {
        const page = i
        const paginationItem = $('<li></li>').attr({'data-page': page})
        if (currentPage === i) {
            paginationItem
                .addClass('active fw-medium text-primary mx-1 px-2 d-flex align-items-center')
                .text(page)
        } else {
            paginationItem.append(
                $('<button></button>').attr({
                    class: 'page',
                    type: 'button',
                }).text(page).on('click', () => {
                    loadBrands(page, searchKeyword)
                })
            )
        }
        paginationGroup.append(paginationItem)
    }

    paginationGroup.find(`[data-page=${active}]`).addClass('active')
    $('#paginationBar > .pagination-prev').toggleClass('disabled', active === 1)
    $('#paginationBar > .pagination-next').toggleClass('disabled', active === totalPage)
}

function formClear() {
    brandForm.removeClass('was-validated')
    brandIdInput.val('')
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
    brandIdInput.val(brand.brandId)
    brandCodeInput.val(brand.brandCode)
    brandNameInput.val(brand.brandName)
}

function trimInputElement(e) {
    e.stopPropagation();
    const inputElement = e.delegateTarget
    const cursorPos = inputElement.selectionStart
    const beforeTrim = inputElement.value
    inputElement.value = beforeTrim.trim()
    if (beforeTrim.length > inputElement.value.length) {
        inputElement.selectionStart = cursorPos - 1
        inputElement.selectionEnd = cursorPos - 1
    }
}

function onLoadBrandList(items) {
    items.forEach(item => {
        const itemId = item.values().brandId
        const rowElm = $(item.elm)

        rowElm.find('.edit-btn').on('click', () => {
            if (newOpened && !brandIdInput.val()) {
                return
            }

            $('#formTitle').text(`Cập nhật: ${item.values().brandName}`)
            loadBrandIntoForm(item.values())
            $('#brandList').find('.delete-btn').addClass('disabled')
            if (!newOpened) {
                mainCard.addClass('updating')
                toggleNewForm()
            }
        })

        function deleteBrand() {
            if (!popOver) {
                return
            }
            $.ajax({
                type: 'DELETE',
                url: `http://localhost:5000/Brand/${itemId}`,
                dataType: 'json',
                contentType: 'application/json',
            })
                .done(result => {
                    console.log(result)
                    loadBrands(currentPage, searchKeyword)
                })
                .fail(err => {
                    console.log(err)
                })
        }

        const popOverElement = $('<div></div>').append([
            $('<p class="mb-2"></p>')
                .append($('<span></span>').text('Xác nhận xoá brand '))
                .append($('<strong></strong>').text(item.values().brandCode)),
            $('<button class="btn btn-sm btn-danger w-50 mx-auto d-block">Xoá</button>').on('click', deleteBrand),
        ])
        const popOverOptions = {
            html: true,
            trigger: 'click',
            placement: 'left',
            content: popOverElement
        }
        const popOver = new bootstrap.Popover(rowElm.find('.delete-btn'), popOverOptions)

        const index = highlightList.findIndex(x => x.id === itemId)
        if (index >= 0) {
            const className = `row-${highlightList[index].type}`
            highlightList.splice(index, 1)
            rowElm.addClass(className)
            setTimeout(() => {
                rowElm.removeClass(className)
            }, 300)
        }
    })
}

brandCodeInput.on({
    keypress: e => {
        const key = String.fromCharCode(e.charCode || e.which)
        if (!/^[a-zA-Z0-9]+$/.test(key)) {
            e.preventDefault()
            return false
        }
    },
    change: e => {
        brandCodeInput.val(brandCodeInput.val().replace(/[^a-zA-Z0-9]+/g, ''))
    }
})

brandNameInput.on('input', trimInputElement)

brandList.clear()
loadBrands()

newBtnGroup.on('click', () => {
    $('#brandList').find('.actions > button').addClass('disabled')
    toggleNewForm()
})

$('#paginationBar .pagination-prev').on('click', () => {
    if (currentPage <= 1) {
        return
    }

    loadBrands(--currentPage, searchKeyword)
})

$('#paginationBar .pagination-next').on('click', () => {
    if (currentPage >= currentTotalPage) {
        return
    }

    loadBrands(++currentPage, searchKeyword)
})

$('#searchInput').on({
    input: e => {
        if (handleByPaste) {
            handleByPaste = false
            return
        }

        const keyword = e.delegateTarget.value

        if (searchTimer) {
            clearTimeout(searchTimer)
        }

        searchTimer = setTimeout(() => {
            if (keyword === searchKeyword) {
                return
            }
            searchKeyword = keyword
            loadBrands(1, searchKeyword)
        }, keyword ? 400 : 0)
    },
    paste: e => {
        handleByPaste = true

        if (searchTimer) {
            clearTimeout(searchTimer)
        }

        if (!e.originalEvent.clipboardData.types.includes('text/plain')) {
            return
        }

        searchKeyword = e.originalEvent.clipboardData.getData('Text')
        loadBrands(1, searchKeyword)
    }
})

$('#clearSearchBtn').on('click', () => {
    $('#searchInput').val('').trigger('input')
})

$(':is(#byName, #byCode)').on('change', () => {
    if (!searchKeyword) {
        return
    }
    loadBrands(1, searchKeyword)
})

brandForm.submit((e) => {
    e.preventDefault()
    e.stopPropagation()

    if (brandForm.find(':invalid').length > 0) {
        brandForm.addClass('was-validated')
        return
    }

    const data = {
        brandCode: brandCodeInput.val().trim().toUpperCase(),
        brandName: brandNameInput.val().trim()
    }
    const brandId = parseInt($('#brandId').val())
    const url = brandId ? `http://localhost:5000/Brand/${brandId}` : 'http://localhost:5000/Brand/create'
    const method = brandId ? 'PUT' : 'POST'

    if (data.brandCode.length === 0 || data.brandName.length === 0) {
        return
    }

    $('#brandForm > fieldset').prop('disabled', true)
    $.ajax({
        type: method,
        url,
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(data),
    })
        .done(result => {
            console.log(result)
            if (newOpened) {
                toggleNewForm()
                formClear()
            }
            highlightList.push({
                id: result.brandId,
                type: brandId ? 'info' : 'success'
            })
            loadBrands(currentPage, searchKeyword)
        })
        .fail(err => {
            console.log(err)
        })
        .always(() => {
            $('#brandForm > fieldset').prop('disabled', false)
        })
})