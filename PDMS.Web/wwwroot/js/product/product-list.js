const options = {
    valueNames: [
        'productName',
        'productCode',
        'barCode',
        'brandId',
        'majorId',
        'suppilerId',
        'createdById',
        'createdTime',
        'importPrice',
        'lastModifiedById',
        'lastModifiedTime',
        'price',
        'quantity',
        {
            data: ['productId'],
        }
    ],
    page: 10
}

const productList = new List("productList", options)

let currentPage = 1
let currentTotalPage = 1
async function loadProducts(page, search) {
    page = page || 1
    const params = new URLSearchParams()
    params.append('page', page)
    params.append('quantity', '9')
    if (search && search.trim().length > 0) {
        params.append('query', search)
        params.append('queryByName', $('input[name=searchBy]:checked').val())
    }
    $.get(`http://localhost:5000/Product/list?${params}`)
        .done(result => {
            currentPage = page
            productList.clear()
            productList.add(result.items, x => {
                x.forEach(y => {
                    const index = y.elm.querySelector('.thumb-wrapper').href.split('/')
                    index.pop()
                    index.push(y.values().productId)
                    y.elm.querySelector('.thumb-wrapper').href = index.join('/')
                    JsBarcode(y.elm.querySelector('.barcodeImg'), y.values().barCode, {
                        format: "CODE128",
                        height: 70,
                        displayValue: false
                    });
                    if (y.elm.querySelector('.update-wrapper') == null) {
                        return
                    } else {
                        const updt = y.elm.querySelector('.update-wrapper').href.split('/')
                        updt.pop()
                        updt.push(y.values().productId)
                        y.elm.querySelector('.update-wrapper').href = updt.join('/')
                    }
                    if (y.elm.querySelector('.delete-wrapper') != null) {
                        const itemId = y.values().productId

                        y.elm.querySelector('.delete-wrapper').addEventListener('click', function () {
                            $.ajax({
                                type: 'DELETE',
                                url: `http://localhost:5000/Product/${itemId}`,
                                dataType: 'json',
                                contentType: 'application/json',
                            })
                                .done(result => {
                                    console.log(result)
                                    loadProducts()
                                })
                                .fail(err => {
                                    console.log(err)
                                })
                        })
                    }

                })
            })
            const totalPage = Math.max(Math.ceil(result.total / result.itemsPerPage), 1)
            currentTotalPage = totalPage
            if (page > totalPage) {
                page = totalPage
                loadProducts(page, search)
                return
            }
            renderPaginationNumber(currentPage, totalPage)
        })
        .fail(err => {
            console.log(err)
        })
}

function onLoadItems(items) {
    items.forEach(item => {
        const itemId = item.value().productId

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
                    loadProducts(page)
                })
            )
        }
        paginationGroup.append(paginationItem)
    }

    paginationGroup.find(`[data-page=${active}]`).addClass('active')
    $('#paginationBar > .pagination-prev').toggleClass('disabled', active === 1)
    $('#paginationBar > .pagination-next').toggleClass('disabled', active === totalPage)
}

productList.clear()
loadProducts()