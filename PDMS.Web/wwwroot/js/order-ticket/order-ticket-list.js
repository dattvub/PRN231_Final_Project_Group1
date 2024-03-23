const opened = new Set()
let firstTime = true

if (showOrderId) {
    opened.add(showOrderId)
}

setBreadcrumb(
    {
        route: 'Trang chủ',
        href: '/'
    },
    'Danh sách phiếu đặt hàng'
)

loadOrder()

function getOrderStatusColor(status) {
    switch (status) {
        case 'Pending':
            return 'text-info'
        case 'Approved':
            return 'text-success'
        case 'Rejected':
            return 'text-danger'
        case 'Received':
            return 'text-secondary'
        case 'Cancel':
            return 'text-warning'
        default:
            return undefined
    }
}

function loadOrder() {
    fetchWithCredentials('http://localhost:5000/OrderTicket', {
        onSuccess: async r => {
            const data = await r.json()
            console.log(data.items)
            let scrollToElement
            $('#order-list').html('').append(
                data.items.map(order => {
                    const randomId = `z${crypto.randomUUID()}`
                    const actionElement = []
                    let onDelete = false
                    if (window.user.role === 'CUSTOMER') {
                        if (order.status === 'Pending') {
                            actionElement.push(
                                $('<button class="btn btn-danger" type="button"></button>').append([
                                    $('<span class="iconify-inline" data-icon="material-symbols:cancel"></span>'),
                                    $('<span> Huỷ</span>')
                                ]).on('click', () => {
                                    onDelete = true
                                    fetchWithCredentials(`http://localhost:5000/OrderTicket/${order.orderId}?s=Cancel`, {
                                        method: 'PATCH',
                                        onSuccess: () => {
                                            const collapse = bootstrap.Collapse.getOrCreateInstance(element.find('.collapse'))
                                            collapse?.hide()
                                        },
                                        onFail: async r => {
                                            const json = await r.json()
                                            showToast('Đã xảy ra lỗi', json?.errors.join('. ') || 'Đã xảy ra lỗi trong quá trình cập nhật trạng thái phiếu đặt hàng')
                                        }
                                    })
                                })
                            )
                        }
                        if (order.status === 'Approved') {
                            actionElement.push(
                                $('<button class="btn btn-danger" type="button"></button>').append([
                                    $('<span class="iconify-inline" data-icon="icon-park-solid:receive"></span>'),
                                    $('<span> Đã nhận</span>')
                                ]).on('click', () => {
                                    onDelete = true
                                    fetchWithCredentials(`http://localhost:5000/OrderTicket/${order.orderId}?s=Received`, {
                                        method: 'PATCH',
                                        onSuccess: () => {
                                            const collapse = bootstrap.Collapse.getOrCreateInstance(element.find('.collapse'))
                                            collapse?.hide()
                                        },
                                        onFail: async r => {
                                            const json = await r.json()
                                            showToast('Đã xảy ra lỗi', json?.errors.join('. ') || 'Đã xảy ra lỗi trong quá trình cập nhật trạng thái phiếu đặt hàng')
                                        }
                                    })
                                })
                            )
                        }
                    } else if (window.user.role !== 'CUSTOMER' && order.status === 'Pending') {
                        actionElement.push(
                            $('<button class="btn btn-success" type="button"></button>').append([
                                $('<span class="iconify-inline" data-icon="mdi:approve"></span>'),
                                $('<span> Chấp nhận</span>')
                            ]).on('click', () => {
                                onDelete = true
                                fetchWithCredentials(`http://localhost:5000/OrderTicket/${order.orderId}?s=Approved`, {
                                    method: 'PATCH',
                                    onSuccess: () => {
                                        const collapse = bootstrap.Collapse.getOrCreateInstance(element.find('.collapse'))
                                        collapse?.hide()
                                    },
                                    onFail: async r => {
                                        const json = await r.json()
                                        showToast('Đã xảy ra lỗi', json?.errors.join('. ') || 'Đã xảy ra lỗi trong quá trình cập nhật trạng thái phiếu đặt hàng')
                                    }
                                })
                            }),
                            $('<button class="btn btn-danger ms-2" type="button"></button>').append([
                                $('<span class="iconify-inline" data-icon="material-symbols:cancel"></span>'),
                                $('<span> Từ chối</span>')
                            ]).on('click', () => {
                                onDelete = true
                                fetchWithCredentials(`http://localhost:5000/OrderTicket/${order.orderId}?s=Rejected`, {
                                    method: 'PATCH',
                                    onSuccess: () => {
                                        const collapse = bootstrap.Collapse.getOrCreateInstance(element.find('.collapse'))
                                        collapse?.hide()
                                    },
                                    onFail: async r => {
                                        const json = await r.json()
                                        showToast('Đã xảy ra lỗi', json?.errors.join('. ') || 'Đã xảy ra lỗi trong quá trình cập nhật trạng thái phiếu đặt hàng')
                                    }
                                })
                            })
                        )
                    }
                    const element = $('<div class="mb-3"></div>').append([
                        $(`<a class="btn btn-falcon-default w-100 text-900 py-3 text-start d-flex justify-content-between" data-bs-toggle="collapse" href="#${randomId}" role="button" aria-expanded="true" aria-controls="${randomId}"></a>`).append([
                            $('<span></span>').append([
                                $('<span>Phiếu đặt hàng: </span>'),
                                $('<span class="fw-semi-bold"></span>').text(order.orderCode)
                            ]),
                            $('<span></span>').append([
                                $('<span>Trạng thái: </span>'),
                                $(`<span class="fw-bold ${getOrderStatusColor(order.status)}"></span>`).text(getOrderStatusText(order.status))
                            ])
                        ]),
                        $('<div class="w-100 pt-2"></div>'),
                        $(`<div class="collapse card${opened.has(order.orderId) ? ' show' : ''}" id="${randomId}"></div>`).append([
                            $('<div class="card-header bg-light d-flex z-index-2 align-items-center"></div>').append([
                                $('<h5 class="text-700 text-center flex-1"></h5>').append(
                                    $('<span>#</span>'),
                                    $('<a class="text-900" href="#"></a>').text(order.orderCode)
                                ),
                                ...actionElement
                            ]),
                            $('<div class="card-body row p-4 g-6"></div>').append([
                                $('<div class="col border-end"></div>').append([
                                    $('<div class="d-flex gap-3 mb-3"></div>').append([
                                        $('<div></div>').append(
                                            $('<span class="iconify-inline fs-2 search-box-icon" data-icon="wpf:name"></span>')
                                        ),
                                        $('<div></div>').append([
                                            $('<label class="form-label opacity-85 m-0">Họ và tên người nhận</label>'),
                                            $('<p class="fw-medium m-0 fs-1"></p>').text(order.customerName)
                                        ])
                                    ]),
                                    $('<div class="d-flex gap-3 mb-3"></div>').append([
                                        $('<div></div>').append(
                                            $('<span class="iconify-inline fs-2 search-box-icon" data-icon="ic:baseline-phone"></span>')
                                        ),
                                        $('<div></div>').append([
                                            $('<label class="form-label opacity-85 m-0">Số điện thoại</label>'),
                                            $('<p class="fw-medium m-0 fs-1"></p>').text(order.customerPhone)
                                        ])
                                    ]),
                                    $('<div class="d-flex gap-3 mb-3"></div>').append([
                                        $('<div></div>').append(
                                            $('<span class="iconify-inline fs-2 search-box-icon" data-icon="entypo:address"></span>')
                                        ),
                                        $('<div></div>').append([
                                            $('<label class="form-label opacity-85 m-0">Địa chỉ nhận</label>'),
                                            $('<p class="fw-medium m-0 fs-1"></p>').text(order.address)
                                        ])
                                    ]),
                                    $('<div class="d-flex gap-3 mb-3"></div>').append([
                                        $('<div></div>').append(
                                            $('<span class="iconify-inline fs-2 search-box-icon" data-icon="mdi:calendar-end"></span>')
                                        ),
                                        $('<div></div>').append([
                                            $('<label class="form-label opacity-85 m-0">Ngày gửi mong muốn</label>'),
                                            $('<p class="fw-medium m-0 fs-1"></p>').text(order.expectedOrderDate ? flatpickr.formatDate(new Date(order.expectedOrderDate), 'd/m/Y') : '...')
                                        ])
                                    ]),
                                    $('<div class="d-flex gap-3 mb-3"></div>').append([
                                        $('<div></div>').append(
                                            $('<span class="iconify-inline fs-2 search-box-icon" data-icon="mdi:calendar-start"></span>')
                                        ),
                                        $('<div></div>').append([
                                            $('<label class="form-label opacity-85 m-0">Ngày nhận mong muốn</label>'),
                                            $('<p class="fw-medium m-0 fs-1"></p>').text(order.expectedReceiveDate ? flatpickr.formatDate(new Date(order.expectedReceiveDate), 'd/m/Y') : '...')
                                        ])
                                    ]),
                                    $('<div class="d-flex gap-3 mb-3"></div>').append([
                                        $('<div></div>').append(
                                            $('<span class="iconify-inline fs-2 search-box-icon" data-icon="mdi:calendar-end-outlined"></span>')
                                        ),
                                        $('<div></div>').append([
                                            $('<label class="form-label opacity-85 m-0">Thời gian gửi hàng</label>'),
                                            $('<p class="fw-medium m-0 fs-1"></p>').text(order.orderDate ? flatpickr.formatDate(new Date(order.orderDate), 'h:i:S d/m/Y') : '...')
                                        ])
                                    ]),
                                    $('<div class="d-flex gap-3 mb-3"></div>').append([
                                        $('<div></div>').append(
                                            $('<span class="iconify-inline fs-2 search-box-icon" data-icon="mdi:calendar-start-outline"></span>')
                                        ),
                                        $('<div></div>').append([
                                            $('<label class="form-label opacity-85 m-0">Thời gian nhận hàng</label>'),
                                            $('<p class="fw-medium m-0 fs-1"></p>').text(order.receiveDate ? flatpickr.formatDate(new Date(order.receiveDate), 'h:i:S d/m/Y') : '...')
                                        ])
                                    ])
                                ]),
                                $('<div class="col"></div>').append([
                                    $('<div class="d-flex flex-column gap-4"></div>').append(
                                        order.orderDetails.map(orderDetail => {
                                            const firstImg = JSON.parse(orderDetail.images)[0]
                                            const img = firstImg ? `http://localhost:5000/${firstImg}` : '/images/placeholder.png'
                                            return $('<div class="d-flex gap-3 pb-3 border-bottom"></div>').append([
                                                $('<img width="106" height="106" class="fit-cover rounded-1 image" alt="">').attr('src', img),
                                                $('<div class="flex-1 position-relative"></div>').append([
                                                    $('<h5 class="h5 text-900 ellipsis-2 productName"></h5>').text(orderDetail.productName),
                                                    $('<div class="ps-3 mt-2"></div>').append(
                                                        $('<table class="opacity-85 fs--1"></table>').append(
                                                            $('<tbody></tbody>').append([
                                                                $('<tr></tr>').append([
                                                                    $('<td class="pe-3">Mã sản phẩm:</td>'),
                                                                    $('<td class="fw-medium productCode"></td>').text(orderDetail.productCode)
                                                                ]),
                                                                $('<tr></tr>').append([
                                                                    $('<td class="pe-3">Số lượng:</td>'),
                                                                    $('<td class="fw-medium quantity"></td>').text(orderDetail.quantity)
                                                                ])
                                                            ])
                                                        )
                                                    ),
                                                    $('<span class="fs-2 text-warning fw-medium position-absolute end-0 bottom-0"></span>').append([
                                                        $('<span class="totalPrice"></span>').text(String(orderDetail.total).replace(/\B(?=(\d{3})+(?!\d))/g, ',')),
                                                        $('<span class="text-decoration-underline">đ</span>')
                                                    ])
                                                ])
                                            ])
                                        })
                                    ),
                                    $('<p class="text-end mt-3"></p>').append([
                                        $('<span class="fs-1 pe-3">Tổng thanh toán</span>'),
                                        $('<span class="fs-3 text-warning fw-semi-bold">').append([
                                            $('<span class="totalPrice"></span>').text(String(order.totalPay).replace(/\B(?=(\d{3})+(?!\d))/g, ',')),
                                            $('<span class="text-decoration-underline">đ</span>')
                                        ])
                                    ])
                                ])
                            ])
                        ]).on({
                            'shown.bs.collapse': () => {
                                opened.add(order.orderId)
                            },
                            'hidden.bs.collapse': () => {
                                opened.delete(order.orderId)
                                if (!onDelete) {
                                    return
                                }
                                loadOrder()
                            }
                        })
                    ])
                    if (firstTime && showOrderId === order.orderId) {
                        firstTime = false
                        scrollToElement = element
                    }
                    return element
                })
            )
            if (scrollToElement) {
                scrollToElement[0].scrollIntoView()
            }
        }
    })
}