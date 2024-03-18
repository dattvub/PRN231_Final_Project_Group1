const mainForm = $('#mainForm')


mainForm.on('submit', async e => {
    e.preventDefault()
    e.stopPropagation()

    if (mainForm.find(':invalid').length > 0) {
        mainForm.addClass('was-validated')
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
    
    const r = await fetchWithCredentials('http://localhost:5000/Customer/Create', {
        method: 'POST',
        body: form
    })
    if (r.ok) {
        window.location.href = '/Customer'
    }
})



$('#btnReset').on('click', () => {
    mainForm[0].reset()
})

