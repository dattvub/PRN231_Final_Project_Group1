const options = {
    valueNames: [
        'customerTypeCode',
        'customerTypeName',
        {
            data: ['customerTypeId'],
        }
    ],
    page: 10
}
const initValues = [
    {
        customerTypeId: 1,
        customerTypeCode: 'ABC',
        customerTypeName: 'Pfizer',
        status: true,
    },
    {
        customerTypeId: 2,
        customerTypeCode: 'ABC',
        customerTypeName: 'AbbVie',
        status: true,
    },
    {
        customerTypeId: 3,
        customerTypeCode: 'ABC',
        customerTypeName: 'Johnson & Johnson',
        status: true,
    },
    {
        customerTypeId: 4,
        customerTypeCode: 'ABC',
        customerTypeName: 'Merck & Co',
        status: true,
    },
    {
        customerTypeId: 5,
        customerTypeCode: 'ABC',
        customerTypeName: 'Novartis',
        status: true,
    },
    {
        customerTypeId: 6,
        customerTypeCode: 'ABC',
        customerTypeName: 'Roche',
        status: true,
    },
    {
        customerTypeId: 7,
        customerTypeCode: 'ABC',
        customerTypeName: 'Bristol-Myers Squibb',
        status: true,
    },
    {
        customerTypeId: 8,
        customerTypeCode: 'ABC',
        customerTypeName: 'Sanofi',
        status: true,
    },
    {
        customerTypeId: 9,
        customerTypeCode: 'ABC',
        customerTypeName: 'AstraZeneca',
        status: true,
    },
    {
        customerTypeId: 10,
        customerTypeCode: 'ABC',
        customerTypeName: 'GSK',
        status: true,
    },
]
const formTitle = $('#formTitle')
const defaultFormTitle = formTitle.text()
const customerTypeList = new List('customerTypeList', options)
const mainCard = $('#mainCard')
const newBtnGroup = $('#newBtnGroup')
const customerTypeForm = $('#customerTypeForm')
const customerTypeIdInput = $('#customerTypeId')
const customerTypeCodeInput = $('#customerTypeCode')
const customerTypeNameInput = $('#customerTypeName')
/** @type {[{id: number, type: string}]} */
const highlightList = []
let newOpened = mainCard.hasClass('newOpened');
let currentPage = 1
let currentTotalPage = 1
let searchKeyword = ''
let searchTimer
let handleByPaste = false

async function loadcustomerTypes(page, search) {
    page = page || 1
    const params = new URLSearchParams()
    params.append('page', page)
    params.append('quantity', '10')
    if (search && search.trim().length > 0) {
        params.append('query', search)
        params.append('queryByName', $('input[name=searchBy]:checked').val())
    }
    $.get(`http://localhost:5000/customerType/list?${params}`)
        .done(result => {
            currentPage = page
            $('#mainGrid tbody .delete-btn').each((i, elm) => {
                bootstrap.Popover.getInstance(elm).dispose()
            })
            customerTypeList.clear()
            customerTypeList.add(result.items, onLoadcustomerTypeList)
            const totalPage = Math.max(Math.ceil(result.total / result.itemsPerPage), 1)
            currentTotalPage = totalPage
            if (page > totalPage) {
                page = totalPage
                loadcustomerTypes(page, search)
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
                    loadcustomerTypes(page, searchKeyword)
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
    customerTypeForm.removeClass('was-validated')
    customerTypeIdInput.val('')
    customerTypeForm[0].reset()
}

function toggleNewForm() {
    newOpened = !newOpened
    if (!newOpened) {
        formClear()
        mainCard.removeClass('updating')
        formTitle.text(defaultFormTitle)
        $('#customerTypeList').find('.actions > button').removeClass('disabled')
    }
    mainCard.toggleClass('newOpened', newOpened)
}

function loadcustomerTypeIntoForm(customerType) {
    customerTypeIdInput.val(customerType.customerTypeId)
    customerTypeCodeInput.val(customerType.customerTypeCode)
    customerTypeNameInput.val(customerType.customerTypeName)
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

function onLoadcustomerTypeList(items) {
    items.forEach(item => {
        const itemId = item.values().customerTypeId
        const rowElm = $(item.elm)

        rowElm.find('.edit-btn').on('click', () => {
            if (newOpened && !customerTypeIdInput.val()) {
                return
            }

            $('#formTitle').text(`Cập nhật: ${item.values().customerTypeName}`)
            loadcustomerTypeIntoForm(item.values())
            $('#customerTypeList').find('.delete-btn').addClass('disabled')
            if (!newOpened) {
                mainCard.addClass('updating')
                toggleNewForm()
            }
        })

        function deletecustomerType() {
            if (!popOver) {
                return
            }
            $.ajax({
                type: 'DELETE',
                url: `http://localhost:5000/customerType/${itemId}`,
                dataType: 'json',
                contentType: 'application/json',
            })
                .done(result => {
                    console.log(result)
                    loadcustomerTypes(currentPage, searchKeyword)
                })
                .fail(err => {
                    console.log(err)
                })
        }

        const popOverElement = $('<div></div>').append([
            $('<p class="mb-2"></p>')
                .append($('<span></span>').text('Xác nhận xoá customerType '))
                .append($('<strong></strong>').text(item.values().customerTypeCode)),
            $('<button class="btn btn-sm btn-danger w-50 mx-auto d-block">Xoá</button>').on('click', deletecustomerType),
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

customerTypeCodeInput.on({
    keypress: e => {
        const key = String.fromCharCode(e.charCode || e.which)
        if (!/^[a-zA-Z0-9]+$/.test(key)) {
            e.preventDefault()
            return false
        }
    },
    change: e => {
        customerTypeCodeInput.val(customerTypeCodeInput.val().replace(/[^a-zA-Z0-9]+/g, ''))
    }
})

// customerTypeNameInput.on('input', trimInputElement)

customerTypeList.clear()
loadcustomerTypes()

newBtnGroup.on('click', () => {
    $('#customerTypeList').find('.actions > button').addClass('disabled')
    toggleNewForm()
})

$('#paginationBar .pagination-prev').on('click', () => {
    if (currentPage <= 1) {
        return
    }

    loadcustomerTypes(--currentPage, searchKeyword)
})

$('#paginationBar .pagination-next').on('click', () => {
    if (currentPage >= currentTotalPage) {
        return
    }

    loadcustomerTypes(++currentPage, searchKeyword)
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
            loadcustomerTypes(1, searchKeyword)
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
        loadcustomerTypes(1, searchKeyword)
    }
})

$('#clearSearchBtn').on('click', () => {
    $('#searchInput').val('').trigger('input')
})

$(':is(#byName, #byCode)').on('change', () => {
    if (!searchKeyword) {
        return
    }
    loadcustomerTypes(1, searchKeyword)
})

customerTypeForm.submit((e) => {
    e.preventDefault()
    e.stopPropagation()

    if (customerTypeForm.find(':invalid').length > 0) {
        customerTypeForm.addClass('was-validated')
        return
    }

    const data = {
        customerTypeCode: customerTypeCodeInput.val().trim().toUpperCase(),
        customerTypeName: customerTypeNameInput.val().trim()
    }
    const customerTypeId = parseInt($('#customerTypeId').val())
    const url = customerTypeId ? `http://localhost:5000/customerType/${customerTypeId}` : 'http://localhost:5000/customerType/create'
    const method = customerTypeId ? 'PUT' : 'POST'

    if (data.customerTypeCode.length === 0 || data.customerTypeName.length === 0) {
        return
    }

    $('#customerTypeForm > fieldset').prop('disabled', true)
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
                id: result.customerTypeId,
                type: customerTypeId ? 'info' : 'success'
            })
            loadcustomerTypes(currentPage, searchKeyword)
        })
        .fail(err => {
            console.log(err)
        })
        .always(() => {
            $('#customerTypeForm > fieldset').prop('disabled', false)
        })
})