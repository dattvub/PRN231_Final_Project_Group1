const options = {
    valueNames: [
        'customerGroupCode',
        'customerGroupName',
        {
            data: ['customerGroupId'],
        }
    ],
    page: 10
}
const initValues = [
    {
        customerGroupId: 1,
        customerGroupCode: 'ABC',
        customerGroupName: 'Pfizer',
        status: true,
    },
    {
        customerGroupId: 2,
        customerGroupCode: 'ABC',
        customerGroupName: 'AbbVie',
        status: true,
    },
    {
        customerGroupId: 3,
        customerGroupCode: 'ABC',
        customerGroupName: 'Johnson & Johnson',
        status: true,
    },
    {
        customerGroupId: 4,
        customerGroupCode: 'ABC',
        customerGroupName: 'Merck & Co',
        status: true,
    },
    {
        customerGroupId: 5,
        customerGroupCode: 'ABC',
        customerGroupName: 'Novartis',
        status: true,
    },
    {
        customerGroupId: 6,
        customerGroupCode: 'ABC',
        customerGroupName: 'Roche',
        status: true,
    },
    {
        customerGroupId: 7,
        customerGroupCode: 'ABC',
        customerGroupName: 'Bristol-Myers Squibb',
        status: true,
    },
    {
        customerGroupId: 8,
        customerGroupCode: 'ABC',
        customerGroupName: 'Sanofi',
        status: true,
    },
    {
        customerGroupId: 9,
        customerGroupCode: 'ABC',
        customerGroupName: 'AstraZeneca',
        status: true,
    },
    {
        customerGroupId: 10,
        customerGroupCode: 'ABC',
        customerGroupName: 'GSK',
        status: true,
    },
]
const formTitle = $('#formTitle')
const defaultFormTitle = formTitle.text()
const customerGroupList = new List('customerGroupList', options)
const mainCard = $('#mainCard')
const newBtnGroup = $('#newBtnGroup')
const customerGroupForm = $('#customerGroupForm')
const customerGroupIdInput = $('#customerGroupId')
const customerGroupCodeInput = $('#customerGroupCode')
const customerGroupNameInput = $('#customerGroupName')
/** @type {[{id: number, type: string}]} */
const highlightList = []
let newOpened = mainCard.hasClass('newOpened');
let currentPage = 1
let currentTotalPage = 1
let searchKeyword = ''
let searchTimer
let handleByPaste = false

async function loadcustomerGroups(page, search) {
    page = page || 1
    const params = new URLSearchParams()
    params.append('page', page)
    params.append('quantity', '10')
    if (search && search.trim().length > 0) {
        params.append('query', search)
        params.append('queryByName', $('input[name=searchBy]:checked').val())
    }
    $.get(`http://localhost:5000/CustomerGroup/list?${params}`)
        .done(result => {
            currentPage = page
            $('#mainGrid tbody .delete-btn').each((i, elm) => {
                bootstrap.Popover.getInstance(elm).dispose()
            })
            customerGroupList.clear()
            customerGroupList.add(result.items, onLoadcustomerGroupList)
            const totalPage = Math.max(Math.ceil(result.total / result.itemsPerPage), 1)
            currentTotalPage = totalPage
            if (page > totalPage) {
                page = totalPage
                loadcustomerGroups(page, search)
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
                    loadcustomerGroups(page, searchKeyword)
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
    customerGroupForm.removeClass('was-validated')
    customerGroupIdInput.val('')
    customerGroupForm[0].reset()
}

function toggleNewForm() {
    newOpened = !newOpened
    if (!newOpened) {
        formClear()
        mainCard.removeClass('updating')
        formTitle.text(defaultFormTitle)
        $('#customerGroupList').find('.actions > button').removeClass('disabled')
    }
    mainCard.toggleClass('newOpened', newOpened)
}

function loadcustomerGroupIntoForm(customerGroup) {
    customerGroupIdInput.val(customerGroup.customerGroupId)
    customerGroupCodeInput.val(customerGroup.customerGroupCode)
    customerGroupNameInput.val(customerGroup.customerGroupName)
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

function onLoadcustomerGroupList(items) {
    items.forEach(item => {
        const itemId = item.values().customerGroupId
        const rowElm = $(item.elm)

        rowElm.find('.edit-btn').on('click', () => {
            if (newOpened && !customerGroupIdInput.val()) {
                return
            }

            $('#formTitle').text(`Cập nhật: ${item.values().customerGroupName}`)
            loadcustomerGroupIntoForm(item.values())
            $('#customerGroupList').find('.delete-btn').addClass('disabled')
            if (!newOpened) {
                mainCard.addClass('updating')
                toggleNewForm()
            }
        })

        function deletecustomerGroup() {
            if (!popOver) {
                return
            }
            $.ajax({
                type: 'DELETE',
                url: `http://localhost:5000/CustomerGroup/${itemId}`,
                dataType: 'json',
                contentType: 'application/json',
            })
                .done(result => {
                    console.log(result)
                    loadcustomerGroups(currentPage, searchKeyword)
                })
                .fail(err => {
                    console.log(err)
                })
        }

        const popOverElement = $('<div></div>').append([
            $('<p class="mb-2"></p>')
                .append($('<span></span>').text('Xác nhận xoá customerGroup '))
                .append($('<strong></strong>').text(item.values().customerGroupCode)),
            $('<button class="btn btn-sm btn-danger w-50 mx-auto d-block">Xoá</button>').on('click', deletecustomerGroup),
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

customerGroupCodeInput.on({
    keypress: e => {
        const key = String.fromCharCode(e.charCode || e.which)
        if (!/^[a-zA-Z0-9]+$/.test(key)) {
            e.preventDefault()
            return false
        }
    },
    change: e => {
        customerGroupCodeInput.val(customerGroupCodeInput.val().replace(/[^a-zA-Z0-9]+/g, ''))
    }
})

// customerGroupNameInput.on('input', trimInputElement)

customerGroupList.clear()
loadcustomerGroups()

newBtnGroup.on('click', () => {
    $('#customerGroupList').find('.actions > button').addClass('disabled')
    toggleNewForm()
})

$('#paginationBar .pagination-prev').on('click', () => {
    if (currentPage <= 1) {
        return
    }

    loadcustomerGroups(--currentPage, searchKeyword)
})

$('#paginationBar .pagination-next').on('click', () => {
    if (currentPage >= currentTotalPage) {
        return
    }

    loadcustomerGroups(++currentPage, searchKeyword)
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
            loadcustomerGroups(1, searchKeyword)
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
        loadcustomerGroups(1, searchKeyword)
    }
})

$('#clearSearchBtn').on('click', () => {
    $('#searchInput').val('').trigger('input')
})

$(':is(#byName, #byCode)').on('change', () => {
    if (!searchKeyword) {
        return
    }
    loadcustomerGroups(1, searchKeyword)
})

customerGroupForm.submit((e) => {
    e.preventDefault()
    e.stopPropagation()

    if (customerGroupForm.find(':invalid').length > 0) {
        customerGroupForm.addClass('was-validated')
        return
    }

    const data = {
        customerGroupCode: customerGroupCodeInput.val().trim().toUpperCase(),
        customerGroupName: customerGroupNameInput.val().trim()
    }
    const customerGroupId = parseInt($('#customerGroupId').val())
    const url = customerGroupId ? `http://localhost:5000/CustomerGroup/${customerGroupId}` : 'http://localhost:5000/CustomerGroup/create'
    const method = customerGroupId ? 'PUT' : 'POST'

    if (data.customerGroupCode.length === 0 || data.customerGroupName.length === 0) {
        return
    }

    $('#customerGroupForm > fieldset').prop('disabled', true)
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
                id: result.customerGroupId,
                type: customerGroupId ? 'info' : 'success'
            })
            loadcustomerGroups(currentPage, searchKeyword)
        })
        .fail(err => {
            console.log(err)
        })
        .always(() => {
            $('#customerGroupForm > fieldset').prop('disabled', false)
        })
})