const options = {
    valueNames: [
        'empName',
        'empCode',
        'email',
        'position',
        'department',
        {
            data: ['empId'],
        }
    ],
    page: 10
}
const employeeList = new List('employeeList', options)
const groupSearchInput = $('#search-group-input')
let groupSearchTimer
let oldGroupSearchText = ''
let selectedGroupId = 0
let currentPage = 1

setBreadcrumb(
    {
        route: 'Trang chủ',
        href: '/'
    },
    'Danh sách nhân viên'
)
employeeList.clear()

$('#all-emp').on({
    click() {
        onGroupSeleted({
            groupId: -1,
            groupName: 'Tất cả nhân viên'
        }, this)
    }
})
$('#no-group-emp').on({
    click() {
        onGroupSeleted({
            groupId: -2,
            groupName: 'Nhân viên chưa vào group'
        }, this)
    }
})
$(document).on('click', (e) => {
    const groupAction = e.target.closest('.group-action.show')
    if (groupAction) {
        return
    }
    $('.group-action.show').each((_, e) => {
        const collapse = bootstrap.Collapse.getInstance(e)
        if (collapse) {
            collapse.hide()
        }
    })
})
$('#group-create').on({
    'show.bs.collapse': () => {
        $('#btn-group-add')[0].disabled = true
    },
    'hide.bs.collapse': () => {
        $('#btn-group-add')[0].disabled = false
    },
    'hidden.bs.collapse': () => {
        $('#group-create')[0].reset()
    },
    submit: async e => {
        e.stopPropagation()
        e.preventDefault()
        const fieldset = $('#group-create').find('fieldset')[0]
        fieldset.disabled = true
        const form = new FormData()
        form.set('name', $('#group-name').val().trim())
        form.set('address', $('#group-address').val().trim())
        const r = await fetchWithCredentials('http://localhost:5000/Group/create', {
            method: 'POST',
            body: form
        })
        fieldset.disabled = false
        if (r.ok) {
            $('#cancel-add-group').click()
            loadGroups()
        }
    }
})
$('#group-search-box').on({
    'hidden.bs.collapse': () => {
        $('#clear-group-search-btn').click()
    },
})
$('#clear-group-search-btn').on({
    click: () => {
        oldGroupSearchText = ''
        loadGroups()
        setTimeout(() => {
            $('#clear-group-search-btn')[0].disabled = true
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
            loadGroups()
        }, 400)
    },
})
$('#cancel-add-group').on('click', () => {
    bootstrap.Collapse.getInstance($('#group-create'))?.hide()
})

$('#table-title').text('Tất cả nhân viên')
onGroupSeleted({
    groupId: -1,
    groupName: 'Tất cả nhân viên'
}, $('#all-emp')[0])
loadGroups()

function loadEmpList() {
    const searchParams = new URLSearchParams()
    searchParams.set('InGroup', selectedGroupId)
    searchParams.set('Page', currentPage)
    fetchWithCredentials(`http://localhost:5000/Employee?${searchParams}`, {
        onSuccess: async r => {
            const data = await r.json()
            console.log(data)
            $('#total-item-counter').text(data.total)
            employeeList.clear()
            employeeList.add(data.items, onLoadEmployeeList)
        }
    })
}

async function loadGroups() {
    const searchParams = new URLSearchParams()
    searchParams.set('q', oldGroupSearchText)
    const r = await fetchWithCredentials(`http://localhost:5000/Group?${searchParams}`)
    if (!r.ok) {
        return
    }
    const groups = await r.json()
    $('#group-search-not-found').toggleClass('d-none', groups.length !== 0)
    renderGroupsSidebar(groups)
    $('#group-container').find('[data-bs-toggle=tooltip]').each((_, e) => {
        bootstrap.Tooltip.getOrCreateInstance(e)
    })
}

function onGroupSeleted(group, elm) {
    if (selectedGroupId === group.groupId) {
        return
    }
    selectedGroupId = group.groupId
    currentPage = 1
    loadEmpList()
    $('#table-title').text(group.groupName)
    if (elm) {
        $('#navbarVerticalNav .nav-link.active').removeClass('active')
        elm.classList.add('active')
    }
}

function renderGroupsSidebar(groups) {
    let updated = false
    $('#group-container').html('').append(
        groups.map(group => {
            return [
                $('<div class="nav-link" role="button" aria-expanded="false"></div>').append(
                    $('<div class="d-flex align-items-center"></div>').append([
                        $('<span class="nav-link-icon lh-1"></span>').append(
                            $('<i class="iconify-inline fs-1" data-icon="mdi:user-box"></i>')
                        ),
                        $('<div class="d-flex flex-1"></div>').append([
                            $('<span class="nav-link-text ps-1 lh-1 m-0" data-bs-toggle="tooltip" data-bs-placement="left" data-bs-trigger="hover"></span>')
                                .attr({
                                    title: group.address
                                })
                                .text(group.groupName),
                            $(`<a draggable="false" class="nav-link-text nav-link py-0 p-1 lh-1 ms-auto toggle-group-action" href="#group-action-${group.groupId}" role="button" data-bs-toggle="collapse" aria-expanded="false"></a>`).append(
                                $('<span class="iconify-inline fs--1" data-icon="fa6-solid:ellipsis"></span>')
                            )
                        ])
                    ])
                ).on({
                    click() {
                        onGroupSeleted(group, this)
                    }
                }),
                $(`<div class="nav collapse group-action" id="group-action-${group.groupId}"></div>`).append(
                    $('<fieldset class="nav-item d-flex justify-content-center gap-3"></fieldset>').append([
                        $(`<a draggable="false" href="#update-form-${group.groupId}" class="btn btn-sm btn-primary" role="button" data-bs-toggle="collapse" aria-expanded="false">Sửa</a>`),
                        $('<button class="btn btn-sm btn-danger">Xoá</button>').on({
                            click: () => {
                                fetchWithCredentials(`http://localhost:5000/Group/${group.groupId}`, {
                                    method: 'DELETE',
                                    onSuccess: () => {
                                        updated = true
                                        showToast('Xoá nhóm', 'Xoá nhóm thành công')
                                        bootstrap.Collapse.getInstance($(`#group-action-${group.groupId}`)).hide()
                                    },
                                    onFail: async r => {
                                        const json = await r.json()
                                        showToast('Đã xảy ra lỗi', json?.errors?.join('. ') || 'Đã có lỗi xảy ra')
                                    }
                                })
                            }
                        })
                    ])
                ).on({
                    'hidden.bs.collapse': () => {
                        if (updated) {
                            loadGroups()
                        }
                    }
                }),
                $(`<form class="collapse" id="update-form-${group.groupId}"></form>`).on({
                    'show.bs.collapse': () => {
                        const groupActionElm = $(`#group-action-${group.groupId}`)
                        groupActionElm.find('fieldset')[0].disabled = true
                        $('.toggle-group-action').addClass('d-none')
                        const collapse = bootstrap.Collapse.getInstance(groupActionElm)
                        collapse?.hide()
                    },
                    'hide.bs.collapse': () => {
                        $(`#group-action-${group.groupId}`).find('fieldset')[0].disabled = false
                        $('.toggle-group-action').removeClass('d-none')
                    },
                    'hidden.bs.collapse': () => {
                        if (updated) {
                            loadGroups()
                        } else {
                            $(`#group-name-${group.groupId}`).val(group.groupName)
                            $(`#group-address-${group.groupId}`).val(group.address)
                        }
                    },
                    submit: async e => {
                        e.stopPropagation()
                        e.preventDefault()
                        const name = $(`#group-name-${group.groupId}`).val().trim()
                        const address = $(`#group-address-${group.groupId}`).val().trim()
                        const form = new FormData()
                        if (name && name !== group.groupName) {
                            form.set('name', name)
                        }
                        if (address && address !== group.address) {
                            form.set('address', address)
                        }
                        if ([...form].length === 0) {
                            return
                        }
                        const r = await fetchWithCredentials(`http://localhost:5000/Group/${group.groupId}`, {
                            method: 'PATCH',
                            body: form
                        })
                        if (!r.ok) {
                            const json = await r.json()
                            showToast('Cập nhật nhóm', json.errors.join('. '))
                            return
                        }
                        updated = true
                        showToast('Cập nhật nhóm', 'Cập nhật nhóm thành công')
                        bootstrap.Collapse.getInstance($(`#update-form-${group.groupId}`)).hide()
                    }
                }).append(
                    $('<fieldset class="fw-normal p-3 mb-3 bg-white rounded-3 shadow-sm border"></fieldset>').append([
                        $('<div class="mb-3"></div>').append([
                            $('<label class="form-label" for="group-name">Tên nhóm</label>'),
                            $(`<input class="form-control" id="group-name-${group.groupId}" name="group-name" type="text"/>`).val(group.groupName)
                        ]),
                        $('<div class="mb-3"></div>').append([
                            $('<label class="form-label" for="group-address">Địa chỉ</label>'),
                            $(`<input class="form-control" id="group-address-${group.groupId}" name="group-address" type="text"/>`).val(group.address)
                        ]),
                        $('<div class="d-flex gap-3"></div>').append([
                            $('<button class="btn btn-falcon-danger w-100 flex-1" type="button">Huỷ</button>').on({
                                click: () => {
                                    bootstrap.Collapse.getInstance($(`#update-form-${group.groupId}`))?.hide()
                                    bootstrap.Collapse.getInstance($(`#group-action-${group.groupId}`))?.show()
                                }
                            }),
                            $('<button class="btn btn-primary w-100 flex-1" type="submit">Cập nhật</button>')
                        ])
                    ])
                )
            ]
        }).reduce((prev, cur) => {
            prev.push(...cur)
            return prev
        }, [])
    )
}

function onLoadEmployeeList(items) {
    items.forEach(item => {
        const [editAction, deleteAction] = $(item.elm).find('.empCode, .empName').each((_, anchor) => {
            anchor.href += `/${item.values().empId}`
        }).end().find('.delete-btn, .edit-btn')
        if (item.values().userName === 'director') {
            deleteAction.remove()
        }
        editAction.href = `/Employee/${item.values().empId}/Edit`
    })
}