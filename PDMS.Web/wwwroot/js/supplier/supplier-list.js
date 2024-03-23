const options = {
    valueNames: [
        'supplierCode',
        'supplierName',
        {
            data: ['supplierId'],
        }
    ],
    page: 10
}
const initValues = [
    {
        supplierId: 1,
        supplierCode: 'ABC',
        supplierName: 'Pfizer',
        status: true,
    },
    {
        supplierId: 2,
        supplierCode: 'ABC',
        supplierName: 'AbbVie',
        status: true,
    },
    {
        supplierId: 3,
        supplierCode: 'ABC',
        supplierName: 'Johnson & Johnson',
        status: true,
    },
    {
        supplierId: 4,
        supplierCode: 'ABC',
        supplierName: 'Merck & Co',
        status: true,
    },
    {
        supplierId: 5,
        supplierCode: 'ABC',
        supplierName: 'Novartis',
        status: true,
    },
    {
        supplierId: 6,
        supplierCode: 'ABC',
        supplierName: 'Roche',
        status: true,
    },
    {
        supplierId: 7,
        supplierCode: 'ABC',
        supplierName: 'Bristol-Myers Squibb',
        status: true,
    },
    {
        supplierId: 8,
        supplierCode: 'ABC',
        supplierName: 'Sanofi',
        status: true,
    },
    {
        supplierId: 9,
        supplierCode: 'ABC',
        supplierName: 'AstraZeneca',
        status: true,
    },
    {
        supplierId: 10,
        supplierCode: 'ABC',
        supplierName: 'GSK',
        status: true,
    },
]
const formTitle = $('#formTitle')
const defaultFormTitle = formTitle.text()
const supplierList = new List('supplierList', options)
const mainCard = $('#mainCard')
const newBtnGroup = $('#newBtnGroup')
const supplierForm = $('#supplierForm')
const supplierIdInput = $('#supplierId')
const supplierCodeInput = $('#supplierCode')
const supplierNameInput = $('#supplierName')
/** @type {[{id: number, type: string}]} */
const highlightList = []
let newOpened = mainCard.hasClass('newOpened');
let currentPage = 1
let currentTotalPage = 1
let searchKeyword = ''
let searchTimer
let handleByPaste = false

async function loadsuppliers(page, search) {
    page = page || 1
    const params = new URLSearchParams()
    params.append('page', page)
    params.append('quantity', '10')
    if (search && search.trim().length > 0) {
        params.append('query', search)
        params.append('queryByName', $('input[name=searchBy]:checked').val())
    }
    $.get(`http://localhost:5000/supplier/list?${params}`)
        .done(result => {
            currentPage = page
            $('#mainGrid tbody .delete-btn').each((i, elm) => {
                bootstrap.Popover.getInstance(elm).dispose()
            })
            supplierList.clear()
            supplierList.add(result.items, onLoadsupplierList)
            const totalPage = Math.max(Math.ceil(result.total / result.itemsPerPage), 1)
            currentTotalPage = totalPage
            if (page > totalPage) {
                page = totalPage
                loadsuppliers(page, search)
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
                    loadsuppliers(page, searchKeyword)
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
    supplierForm.removeClass('was-validated')
    supplierIdInput.val('')
    supplierForm[0].reset()
}

function toggleNewForm() {
    newOpened = !newOpened
    if (!newOpened) {
        formClear()
        mainCard.removeClass('updating')
        formTitle.text(defaultFormTitle)
        $('#supplierList').find('.actions > button').removeClass('disabled')
    }
    mainCard.toggleClass('newOpened', newOpened)
}

function loadsupplierIntoForm(supplier) {
    supplierIdInput.val(supplier.supplierId)
    supplierCodeInput.val(supplier.supplierCode)
    supplierNameInput.val(supplier.supplierName)
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

function onLoadsupplierList(items) {
    items.forEach(item => {
        const itemId = item.values().supplierId
        const rowElm = $(item.elm)

        rowElm.find('.edit-btn').on('click', () => {
            if (newOpened && !supplierIdInput.val()) {
                return
            }

            $('#formTitle').text(`Cập nhật: ${item.values().supplierName}`)
            loadsupplierIntoForm(item.values())
            $('#supplierList').find('.delete-btn').addClass('disabled')
            if (!newOpened) {
                mainCard.addClass('updating')
                toggleNewForm()
            }
        })

        function deletesupplier() {
            if (!popOver) {
                return
            }
            $.ajax({
                type: 'DELETE',
                url: `http://localhost:5000/supplier/${itemId}`,
                dataType: 'json',
                contentType: 'application/json',
            })
                .done(result => {
                    console.log(result)
                    loadsuppliers(currentPage, searchKeyword)
                })
                .fail(err => {
                    console.log(err)
                })
        }

        const popOverElement = $('<div></div>').append([
            $('<p class="mb-2"></p>')
                .append($('<span></span>').text('Xác nhận xoá supplier '))
                .append($('<strong></strong>').text(item.values().supplierCode)),
            $('<button class="btn btn-sm btn-danger w-50 mx-auto d-block">Xoá</button>').on('click', deletesupplier),
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

supplierCodeInput.on({
    keypress: e => {
        const key = String.fromCharCode(e.charCode || e.which)
        if (!/^[a-zA-Z0-9]+$/.test(key)) {
            e.preventDefault()
            return false
        }
    },
    change: e => {
        supplierCodeInput.val(supplierCodeInput.val().replace(/[^a-zA-Z0-9]+/g, ''))
    }
})

// supplierNameInput.on('input', trimInputElement)

supplierList.clear()
loadsuppliers()

newBtnGroup.on('click', () => {
    $('#supplierList').find('.actions > button').addClass('disabled')
    toggleNewForm()
})

$('#paginationBar .pagination-prev').on('click', () => {
    if (currentPage <= 1) {
        return
    }

    loadsuppliers(--currentPage, searchKeyword)
})

$('#paginationBar .pagination-next').on('click', () => {
    if (currentPage >= currentTotalPage) {
        return
    }

    loadsuppliers(++currentPage, searchKeyword)
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
            loadsuppliers(1, searchKeyword)
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
        loadsuppliers(1, searchKeyword)
    }
})

$('#clearSearchBtn').on('click', () => {
    $('#searchInput').val('').trigger('input')
})

$(':is(#byName, #byCode)').on('change', () => {
    if (!searchKeyword) {
        return
    }
    loadsuppliers(1, searchKeyword)
})

supplierForm.submit((e) => {
    e.preventDefault()
    e.stopPropagation()

    if (supplierForm.find(':invalid').length > 0) {
        supplierForm.addClass('was-validated')
        return
    }

    const data = {
        supplierCode: supplierCodeInput.val().trim().toUpperCase(),
        supplierName: supplierNameInput.val().trim()
    }
    const supplierId = parseInt($('#supplierId').val())
    const url = supplierId ? `http://localhost:5000/supplier/${supplierId}` : 'http://localhost:5000/supplier/create'
    const method = supplierId ? 'PUT' : 'POST'

    if (data.supplierCode.length === 0 || data.supplierName.length === 0) {
        return
    }

    $('#supplierForm > fieldset').prop('disabled', true)
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
                id: result.supplierId,
                type: supplierId ? 'info' : 'success'
            })
            loadsuppliers(currentPage, searchKeyword)
        })
        .fail(err => {
            console.log(err)
        })
        .always(() => {
            $('#supplierForm > fieldset').prop('disabled', false)
        })
})