
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
let oldGroupSearchText = ''
let currentPage = 1
let currentTotalPage = 1
let groupSearchTimer
const groupSearchInput = $('#search-group-input')
const sortBy = $('.sort-product')
$('#group-search-box').on({
    'hidden.bs.collapse': () => {
        $('#clear-group-search-btn').click()
    },
})
$('#clear-group-search-btn').on({
    click: () => {
        oldGroupSearchText = ''
        setTimeout(() => {
            $('#clear-group-search-btn')[0].disabled = true
            loadProducts()
        }, 0)
    }
})
groupSearchInput.on({
    input: e => {
        const val = e.delegateTarget.value.trim()
        $('#clear-group-search-btn')[0].disabled = !val
        if (val === oldGroupSearchText) {
            return
        }
        if (groupSearchTimer) {
            clearTimeout(groupSearchTimer)
        }
        groupSearchTimer = setTimeout(() => {
            oldGroupSearchText = val
            loadProducts(1, oldGroupSearchText)
        }, 400)
    },
})

sortBy.on('change', e => {
    const params = new URLSearchParams()
    let action = ''
    if (sortBy.val() === 'newest') {
        action = 'newest'
    } else if (sortBy.val() === 'lowest') {
        action = 'lowest'
    } else if (sortBy.val() === 'highest') {
        action = 'highest'
    }
    loadProducts(1, '', action)
})

async function loadProducts(page, search, action) {
    page = page || 1
    const params = new URLSearchParams()
    params.append('page', page)
    params.append('quantity', '9')
    if (search && search.trim().length > 0) {
        params.append('query', search)
        params.append('queryByName', 'true')
    }
    if (action && action.trim().length > 0) {
        params.append('sortAction', action)
    }
    fetchWithCredentials(`http://localhost:5000/Product/list?${params}`, {
        onSuccess: async r => {
            const result = await r.json()
            $('#total-product').text(`Showing ${page}-9 of ${result.items.length} Products`)
            currentPage = page
            productList.clear()
            productList.add(result.items, x => {
                x.forEach(y => {
                    const index = y.elm.querySelector('.thumb-wrapper').href.split('/')
                    index.pop()
                    index.push(y.values().productId)
                    y.elm.querySelector('.thumb-wrapper').href = index.join('/')

                    const index2 = y.elm.querySelector('.thumb-wrapper-btn').href.split('/')
                    index2.pop()
                    index2.push(y.values().productId)
                    y.elm.querySelector('.thumb-wrapper-btn').href = index2.join('/')


                    const index1 = y.elm.querySelector('.thumb-wrapper-name').href.split('/')
                    index1.pop()
                    index1.push(y.values().productId)
                    y.elm.querySelector('.thumb-wrapper-name').href = index1.join('/')
                    if (y.elm.querySelector('.barcodeImg') != null) {
                        JsBarcode(y.elm.querySelector('.barcodeImg'), y.values().barCode, {
                            format: "CODE128",
                            height: 70,
                            displayValue: false
                        });
                    }
                    if (y.elm.querySelector('.update-wrapper') != null) {
                        const updt = y.elm.querySelector('.update-wrapper').href.split('/')
                        updt.pop()
                        updt.push(y.values().productId)
                        y.elm.querySelector('.update-wrapper').href = updt.join('/')
                    }
                    if (y.elm.querySelector('.delete-wrapper') != null) {
                        const itemId = y.values().productId

                        y.elm.querySelector('.delete-wrapper').addEventListener('click', async function () {

                            const r = await fetchWithCredentials(`http://localhost:5000/Product/${itemId}`, {
                                method: 'DELETE',
                            })
                            if (!r.ok) {
                                const json = await r.json()
                                showToast('Xóa sản phẩm', json.errors.join('. '))
                                return
                            } 
                            showToast('Xóa sản phẩm', 'Xóa sản phẩm thành công')
                            loadProducts()
                        })
                    }
                    const urlItem = y.values().image.replace(/[\[\"\]]/g, "").split(',')[0]
                    y.elm.querySelector('.product-img').src = `http://localhost:5000/` + urlItem

                    fetchWithCredentials(`http://localhost:5000/Brand/${y.values().brandId}`, {
                        onSuccess: async i => {
                            const dataBrand = await i.json()
                            y.elm.querySelector('.brand-name').innerText = dataBrand.brandName
                        }
                    })

                    fetchWithCredentials(`http://localhost:5000/Major/${y.values().majorId}`, {
                        onSuccess: async i => {
                            const dataMajor = await i.json()
                            y.elm.querySelector('.major-name').innerText = dataMajor.majorName
                        }
                    })

                    fetchWithCredentials(`http://localhost:5000/Supplier/${y.values().suppilerId}`, {
                        onSuccess: async i => {
                            const dataSupplier = await i.json()
                            y.elm.querySelector('.suppiler-name').innerText = dataSupplier.supplierName
                        }
                    })
                    setTimeout(() => {
                        if (user.role === "CUSTOMER") {
                            $('.add-product-btn').css("display", "none")
                            $('.action-product-btn').css("display", "none")
                            $('.detail-product-btn').css("display", "block")
                        } else {
                            $('.add-product-btn').css("display", "inline-block")
                            $('.action-product-btn').css("display", "block")
                            $('.detail-product-btn').css("display", "none")
                        }
                    }, 500)
                    
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
        }
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