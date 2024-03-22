let isCodeChanged = true
let isCodeValid = false

function populateCustomerTP() {
    // Lấy dữ liệu từ API và chèn vào thẻ select cho Customer Type
    fetch('http://localhost:5000/CustomerType/list')
        .then(response => response.json())
        .then(data => {
            const selectElement = document.getElementById('CustomerTypeId');
            populateSelectOptions(selectElement, data.items, 'customerTypeId', 'customerTypeName', 'Loại khách hàng');
        })
        .catch(error => {
            console.error('Lỗi khi lấy dữ liệu Customer Type:', error);
        });

    // Lấy dữ liệu từ API và chèn vào thẻ select cho Customer Group
    fetch('http://localhost:5000/CustomerGroup/list')
        .then(response => response.json())
        .then(data => {
            const selectElement = document.getElementById('CustomerGroupId');
            populateSelectOptions(selectElement, data.items, 'customerGroupId', 'customerGroupName', 'Nhóm khách hàng');
        })
        .catch(error => {
            console.error('Lỗi khi lấy dữ liệu Customer Group:', error);
        });
}

function populateSelectOptions(selectElement, items, valueKey, textKey, defaultText) {
    // Xóa tất cả các option hiện có trong thẻ select
    selectElement.innerHTML = '';

    // Tạo option mặc định
    const defaultOption = document.createElement('option');
    defaultOption.value = '';
    defaultOption.textContent = defaultText;
    defaultOption.disabled = true;
    defaultOption.selected = true;
    selectElement.appendChild(defaultOption);

    // Chèn dữ liệu từ API vào thẻ select
    items.forEach(item => {
        const option = document.createElement('option');
        option.value = item[valueKey];
        option.textContent = item[textKey];
        selectElement.appendChild(option);
    });
}

// Gọi hàm populateCustomerTP khi trang được tải
window.onload = populateCustomerTP;

if (window.cusId) {
    initEditPage()
} else {
    setBreadcrumb(
        {
            route: 'Trang chủ',
            href: '/'
        },
        {
            route: 'Danh sách khách hàng',
            href: '/Customer'
        },
        'Thêm khách hàng mới'
    )
}

async function initEditPage() {
    const r = await fetchWithCredentials(`http://localhost:5000/Customer/${window.cusId}`)
    const cus = await r.json()
    window.cus = cus
    console.log(cus)
    setBreadcrumb(
        {
            route: 'Trang chủ',
            href: '/'
        },
        {
            route: 'Danh sách khách hàng',
            href: '/Customer'
        },
        {
            route: cus.customerName,
            href: `/Customer/${cus.cusId}`
        },
        'Cập nhật thông tin'
    )


    $('#lastname').val(cus.lastName)
    $('#firstname').val(cus.firstName)
    $('#cus-code').val(cus.Code)
    $('#email').val(cus.Email)
    $('#password').val(cus.Password)
    $('#taxCode').val(cus.TaxCode)
    $('#CustomerTypeId').val(cus.CustomerTypeId)
    $('#CustomerGroupId').val(cus.CustomerGroupId)
}

setBreadcrumb(
    {
        route: 'Trang chủ',
        href: '/'
    },
    {
        route: 'Danh sách khách hàng',
        href: '/Customer'
    },
    'Thêm khách hàng mới'
)

const mainForm = $('#mainForm')
mainForm.on('submit', async e => {
    e.preventDefault()
    e.stopPropagation()

    const code = $('#cus-code').val().trim()
    if (isCodeChanged) {
        await checkDuppCode(code)
    }
    if (!mainForm.valid()) {
        return
    }

    const form = new FormData()
    form.set('firstName', $('#firstname').val().trim())
    form.set('lastName', $('#lastname').val().trim())
    form.set('code', $('#cus-code').val().trim())
    form.set('email', $('#email').val().trim())
    form.set('password', $('#password').val())
    form.set('taxCode', $('#taxCode').val().trim())
    form.set('CustomerTypeId', $('#CustomerTypeId').val().trim())
    form.set('CustomerGroupId', $('#CustomerGroupId').val().trim())
    const phone = $('#phone').val()?.trim()
    if (phone) {
        form.set('phone', phone)
    }
    const address = $('#address').val()?.trim()
    if (address) {
        form.set('address', address)
    }

    if (!window.cusId) {
        const r = await fetchWithCredentials('http://localhost:5000/Customer/Create', {
            method: 'POST',
            body: form
        })
        if (r.ok) {
            window.location.href = '/Customer'
        } else {
            if (!r.ok) {
                const json = await r.json()
                showToast('Thêm khách hàng', json.errors.join('. '))
            }
        }
    } else {
        console.log(...form)
        const r = await fetchWithCredentials(`http://localhost:5000/Customer/${window.cusId}`, {
            method: 'PUT',
            body: form,
        })
        if (!r.ok) {
            const json = await r.json()
            showToast('Cập nhật thông tin khách hàng', json.errors.join('. '))
        } else {
            window.location.href = `/Customer/${window.cusId}`
        }
    }
})

async function checkDuppCode(val) {
    const form = new FormData()
    form.set('code', val.trim())
    if (window.cusId) {
        form.set('id', window.cusId)
    }
    const r = await fetchWithCredentials('http://localhost:5000/Customer/CheckCode', {
        method: 'POST',
        body: form
    })
    isCodeValid = (await r.text()) === 'true'
    isCodeChanged = false
}



$('#btnReset').on('click', () => {
    mainForm[0].reset()
})





