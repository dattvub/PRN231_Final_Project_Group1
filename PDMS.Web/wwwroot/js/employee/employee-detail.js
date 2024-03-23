fetchWithCredentials(`http://localhost:5000/Employee/${empId}`, {
    onSuccess: async r => {
        const data = await r.json()
        console.log(data)
        setBreadcrumb(
            {
                route: 'Trang chủ',
                href: '/'
            },
            {
                route: 'Danh sách nhân viên',
                href: '/Employee'
            },
            data.empName
        )
        if (data.imageUrl) {
            $('#user-image').attr('src', `http://localhost:5000/${data.imageUrl}`)
        }
        $('#emp-name').text(data.empName)
        $('.emp-code').text(data.empCode)
        $('#gender').text(data.gender ? 'Nam' : 'Nữ')
        $('#phone').text(data.phoneNumber)
        $('#email').text(data.email)
        $('#address').text(data.address)
        $('#position').text(data.position)
        $('#department').text(data.department)

        if (data.userName === 'director') {
            $('#delete-btn').remove()
        }
    }
})