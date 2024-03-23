fetchWithCredentials(`http://localhost:5000/Customer/${cusId}`, {
    onSuccess: async r => {
        const data = await r.json()
        console.log(data)
        setBreadcrumb(
            {
                route: 'Trang chủ',
                href: '/'
            },
            {
                route: 'Danh sách khách hàng',
                href: '/Customer'
            },
            data.customerName
        )
        
        $('#customerName').text(data.customerName)
        $('#email').text(data.email)
        $('#customerCode').text(data.customerCode)
        $('#phoneNumber').text(data.phoneNumber)
        $('#address').text(data.address)
        $('#customerTypeId').text(data.customerTypeId)
        $('#customerGroupId').text(data.customerGroupId)
        $('#taxCode').text(data.taxCode)

    }
})