const options = {
    valueNames: [
        'majorCode',
        'majorName',
        {
            data: ['majorId'],
        }
    ],
    page: 10
}
const formTitle = $('#formTitle')
const defaultFormTitle = formTitle.text()
const majorList = new List('majorList', options)
const mainCard = $('#mainCard')
const newBtnGroup = $('#newBtnGroup')
const majorForm = $('#majorForm')
const majorIdInput = $('#majorId')
const majorCodeInput = $('#majorCode')
const majorNameInput = $('#majorName')
/** @type {[{id: number, type: string}]} */
const highlightList = []
let newOpened = mainCard.hasClass('newOpened');
let currentPage = 1
let currentTotalPage = 1
let searchKeyword = ''
let searchTimer
let handleByPaste = false

async function loadMajors(page, search) {
    page = page || 1
    const params = new URLSearchParams()
    params.append('page', page)
    params.append('quantity', '10')
    if (search && search.trim().length > 0) {
        params.append('query', search)
        params.append('queryByName', $('input[name=searchBy]:checked').val())
    }
    $.get(`http://localhost:5000/Major/ListMajor?${params}`)
        .done(result => {
            currentPage = page
            $('#mainGrid tbody .delete-btn').each((i, elm) => {
                bootstrap.Popover.getInstance(elm).dispose()
            })
            majorList.clear()
            majorList.add(result.items, onLoadMajorList)
            const totalPage = Math.max(Math.ceil(result.total / result.itemsPerPage), 1)
            currentTotalPage = totalPage
            if (page > totalPage) {
                page = totalPage
                loadMajors(page, search)
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
        const paginationItem = $('<li></li>').attr({ 'data-page': page })
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
                    loadMajors(page, searchKeyword)
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
    majorForm.removeClass('was-validated')
    majorIdInput.val('')
    majorForm[0].reset()
}

function toggleNewForm() {
    newOpened = !newOpened
    if (!newOpened) {
        formClear()
        mainCard.removeClass('updating')
        formTitle.text(defaultFormTitle)
        $('majorList').find('.actions > button').removeClass('disabled')
    }
    mainCard.toggleClass('newOpened', newOpened)
}

function loadMajorIntoForm(major) {
    majorIdInput.val(major.majorId)
    majorCodeInput.val(major.majorCode)
    majorNameInput.val(major.majorName)
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

function onLoadMajorList(items) {
    items.forEach(item => {
        const itemId = item.values().majorId
        const rowElm = $(item.elm)

        rowElm.find('.edit-btn').on('click', () => {
            if (newOpened && !majorIdInput.val()) {
                return
            }

            $('#formTitle').text(`Cập nhật: ${item.values().majorName}`)
            loadMajorIntoForm(item.values())
            $('#majorList').find('.delete-btn').addClass('disabled')
            if (!newOpened) {
                mainCard.addClass('updating')
                toggleNewForm()
            }
        })

        function deleteMajor() {
            if (!popOver) {
                return
            }
            $.ajax({
                type: 'DELETE',
                url: `http://localhost:5000/Major/${itemId}`,
                dataType: 'json',
                contentType: 'application/json',
            })
                .done(result => {
                    console.log(result)
                    loadMajors(currentPage, searchKeyword)
                })
                .fail(err => {
                    console.log(err)
                })
        }

        const popOverElement = $('<div></div>').append([
            $('<p class="mb-2"></p>')
                .append($('<span></span>').text('Xác nhận xoá Ngành '))
                .append($('<strong></strong>').text(item.values().brandCode)),
            $('<button class="btn btn-sm btn-danger w-50 mx-auto d-block">Xoá</button>').on('click', deleteMajor),
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

majorCodeInput.on({
    keypress: e => {
        const key = String.fromCharCode(e.charCode || e.which)
        if (!/^[a-zA-Z0-9]+$/.test(key)) {
            e.preventDefault()
            return false
        }
    },
    change: e => {
        majorCodeInput.val(majorCodeInput.val().replace(/[^a-zA-Z0-9]+/g, ''))
    }
})

majorNameInput.on('input', trimInputElement)

majorList.clear()
loadMajors()

newBtnGroup.on('click', () => {
    $('#majorList').find('.actions > button').addClass('disabled')
    toggleNewForm()
})

$('#paginationBar .pagination-prev').on('click', () => {
    if (currentPage <= 1) {
        return
    }

    loadMajors(--currentPage, searchKeyword)
})

$('#paginationBar .pagination-next').on('click', () => {
    if (currentPage >= currentTotalPage) {
        return
    }

    loadMajors(++currentPage, searchKeyword)
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
            loadMajors(1, searchKeyword)
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
        loadMajors(1, searchKeyword)
    }
})

$('#clearSearchBtn').on('click', () => {
    $('#searchInput').val('').trigger('input')
})

$(':is(#byName, #byCode)').on('change', () => {
    if (!searchKeyword) {
        return
    }
    loadMajors(1, searchKeyword)
})

majorForm.submit((e) => {
    e.preventDefault()
    e.stopPropagation()

    if (majorForm.find(':invalid').length > 0) {
        majorForm.addClass('was-validated')
        return
    }

    const data = {
        majorCode: majorCodeInput.val().trim().toUpperCase(),
        majorName: majorNameInput.val().trim()
    }
    const majorId = parseInt($('#majorId').val())
    const url = majorId ? `http://localhost:5000/Major/${majorId}` : 'http://localhost:5000/major/add'
    const method = majorId ? 'PUT' : 'POST'

    if (data.majorCode.length === 0 || data.majorName.length === 0) {
        return
    }

    $('#majorForm > fieldset').prop('disabled', true)
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
                id: result.majorId,
                type: majorId ? 'info' : 'success'
            })
            loadMajors(currentPage, searchKeyword)
        })
        .fail(err => {
            console.log(err)
        })
        .always(() => {
            $('#majorForm > fieldset').prop('disabled', false)
        })
})