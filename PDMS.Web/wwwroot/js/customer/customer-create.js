let isCodeChanged = true
let isCodeValid = false
const mainForm = $('#mainForm')
 function editSaleId() {
    if (window.user.role === 'DIRECTOR') {
        // Hiển thị phần tử HTML
        document.querySelector('.fixSale').style.display = 'block';
    } else {
        document.querySelector('.fixSale').style.display = 'none';
    }
}
function populateCustomerTP() {
    editSaleId();
    fetchWithCredentials('http://localhost:5000/CustomerType/list')
        .then(response => response.json())
        .then(data => {
            const selectElement = document.getElementById('CustomerTypeId');
            populateSelectOptions(selectElement, data.items, 'customerTypeId', 'customerTypeName', 'Loại khách hàng');
        })
        .catch(error => {
            console.error('Lỗi khi lấy dữ liệu Customer Type:', error);
        });

    // Lấy dữ liệu từ API và chèn vào thẻ select cho Customer Group
    fetchWithCredentials('http://localhost:5000/CustomerGroup/list')
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

$('#cus-code').on('input', e => {
    isCodeChanged = true
    const val = $('#cus-code').val().trim()
    if (val.length < 3) {
        isCodeValid = false
        return
    }
})

$.validator.addMethod('validateCode', () => isCodeValid, 'Code đã tồn tại')
mainForm.validate({
    rules: {
        lastname: {
            required: true,
        },
        firstname: {
            required: true,
        },
        'cus-code': {
            required: true,
            minlength: 3,
            maxlength: 50,
            validateCode: true
        },
        email: {
            required: true,
            email: true
        },
        password: {
            required: !window.cusId,
            minlength: 3,
            maxlength: 50,
        },
        phone: {
            required: true,
        },
    }
})

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
    form.set('CustomerTypeId', $('#CustomerTypeId').val())
    form.set('CustomerGroupId', $('#CustomerGroupId').val())

    var salemanIdValue = document.getElementById("salemanId").value.trim();
    if (salemanIdValue === null || salemanIdValue === "") {
        form.set('empId', window.user.associationId)
    } else {
        form.set('empId', $('#salemanId').val())
    }

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

$('#btnReset').on('click', () => {
    mainForm[0].reset()
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
            href: `/Customer/${cus.customerId}`
        },
        'Cập nhật thông tin'
    )
    editSaleId();


    $('#lastname').val(cus.lastName)
    $('#firstname').val(cus.firstName)
    $('#cus-code').val(cus.customerCode)
    $('#email').val(cus.email)
    $('#taxCode').val(cus.taxCode)
    $('#phone').val(cus.phoneNumber)
    $('#address').val(cus.address)
     setTimeout(() => {
    $('#CustomerTypeId').val(cus.customerTypeId)
    $('#CustomerGroupId').val(cus.customerGroupId)
    $('#salemanId').val(cus.empId)
    }, 1000)

    // Kiểm tra nếu địa chỉ URL kết thúc bằng "/create"


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













