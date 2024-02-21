const mainForm = $('#mainForm')
const avatarInput = $('#avatar')

mainForm.on('submit', e => {
    e.preventDefault()
    e.stopPropagation()

    if (mainForm.find(':invalid').length > 0) {
        mainForm.addClass('was-validated')
        return
    }

    console.log(123)
})

avatarInput.on('input', e => {
    const file = e.delegateTarget.files[0];
    if (file) {
        $('#preview-avatar').removeClass('d-none')[0].src = URL.createObjectURL(file)
    } else {
        $('#preview-avatar').addClass('d-none')[0].src = ''
    }
})