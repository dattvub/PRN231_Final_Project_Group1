const options = {
    valueNames: [
        'customerCode',
        'customerName',
        'email',
        'phoneNumber',
        'address',
        {
            data: ['customerId'],
        }
    ],
    page: 10
}
const customerList = new List('customerList', options)
let currentPage = 1

setBreadcrumb(
    {
        route: 'Trang chủ',
        href: '/'
    },
    'Danh sách khách hàng'
)
customerList.clear()

function loadCusList() {
    const searchParams = new URLSearchParams()
    searchParams.set('Page', currentPage)
    fetchWithCredentials(`http://localhost:5000/Customer?${searchParams}`, {
        onSuccess: async r => {
            const data = await r.json()
            console.log(data)
            $('#total-item-counter').text(data.total)
            customerList.clear()
            customerList.add(data.items, onLoadCustomerList)
            console.log(data.item)
        }
    })
}

loadCusList();


function onLoadCustomerList(items) {
    items.forEach(item => {
        const [editAction, deleteAction] = $(item.elm).find('.customerCode, .customerName').each((_, anchor) => {
            anchor.href += `/${item.values().customerId}`
        }).end().find('.delete-btn, .edit-btn')
        editAction.href = `/Customer/${item.values().customerId}/Edit`
    })
}


//==========================================

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

    //======================================== SEARCH
