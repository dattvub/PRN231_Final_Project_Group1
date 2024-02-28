const mainForm = $('#mainForm')
const avatarInput = $('#avatar')
const [entranceDateInput, exitDateInput] = $('.datetimepicker').flatpickr({
    disableMobile: true,
    dateFormat: 'd-m-Y',
    locale: 'vn'
})
const entranceDateSwitch = $('#entranceDateSwitch')
const exitDateSwitch = $('#exitDateSwitch')

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

$('#btnReset').on('click', () => {
    mainForm[0].reset()
    entranceDateSwitch[0].disabled = exitDateSwitch[0].checked
    exitDateSwitch[0].disabled = !entranceDateSwitch[0].checked
})

$(entranceDateInput.element).hide()
$(exitDateInput.element).hide()
exitDateSwitch[0].disabled = !entranceDateSwitch[0].checked

exitDateSwitch.on('change', () => {
    if (!exitDateSwitch[0].checked) {
        exitDateInput.clear()
        exitDateInput.close()
        if (exitDateInput.element.style.display !== 'none') {
            $(exitDateInput.element).slideToggle(160)
        }
        entranceDateSwitch[0].disabled = false
    } else {
        if (exitDateInput.element.style.display === 'none') {
            $(exitDateInput.element).slideToggle(160)
        }
        entranceDateSwitch[0].disabled = true
    }
})

entranceDateSwitch.on('change', () => {
    if (!entranceDateSwitch[0].checked) {
        console.log(123)
        entranceDateInput.clear()
        entranceDateInput.close()
        if (entranceDateInput.element.style.display !== 'none') {
            $(entranceDateInput.element).slideToggle(160)
        }
        exitDateSwitch[0].disabled = true
    } else {
        if (entranceDateInput.element.style.display === 'none') {
            $(entranceDateInput.element).slideToggle(160)
        }
        exitDateSwitch[0].disabled = false
    }
})