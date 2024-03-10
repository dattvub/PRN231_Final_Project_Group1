const mainForm = $('#mainForm')
const avatarInput = $('#avatar')
const [entranceDateInput, exitDateInput] = $('.datetimepicker').flatpickr({
    disableMobile: true,
    dateFormat: 'd-m-Y',
    locale: 'vn'
})
const entranceDateSwitch = $('#entranceDateSwitch')
const exitDateSwitch = $('#exitDateSwitch')
const cropImgElement = $('#img-crop')[0]
let cropper
let prevCropData
let selectedImage

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
    form.set('code', $('#emp-code').val().trim())
    form.set('gender', $('[name=gender]:checked').val())
    form.set('email', $('#email').val().trim())
    form.set('password', $('#password').val())
    const phone = $('#phone').val()?.trim()
    if (phone) {
        form.set('phone', phone)
    }
    const address = $('#address').val()?.trim()
    if (address) {
        form.set('address', address)
    }
    const position = $('#position').val()?.trim()
    if (position) {
        form.set('position', position)
    }
    const department = $('#department').val()?.trim()
    if (department) {
        form.set('department', department)
    }
    if (selectedImage) {
        form.set('image', selectedImage)
    }
    const selectedEntranceDate = entranceDateInput.selectedDates[0]
    if (entranceDateSwitch[0].checked && selectedEntranceDate) {
        const selectedDate = new Date(selectedEntranceDate)
        selectedDate.setHours(selectedDate.getHours() + 7)
        form.set('entranceDate', selectedDate.toJSON().split('T')[0])
    }
    const selectedExitDate = exitDateInput.selectedDates[0]
    if (exitDateSwitch[0].checked && selectedExitDate) {
        const selectedDate = new Date(selectedExitDate)
        selectedDate.setHours(selectedDate.getHours() + 7)
        form.set('exitDate', selectedDate.toJSON().split('T')[0])
    }
    const r = await fetchWithCredentials('http://localhost:5000/Employee/Create', {
        method: 'POST',
        body: form
    })
    if (r.ok) {
        window.location.href = '/Employee'
    }
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

$(window).on({
    dropzoneInit: () => {
        Dropzone.forElement('#img-dropzone')
            .on('addedfile', file => {
                selectedImage = file
            })
            .on('thumbnail', (file, dataUrl) => {
                if (file.name !== '/.png') {
                    cropImgElement.src = dataUrl
                    prevCropData = undefined
                }
            })
            .on('removedfile', (file) => {
                selectedImage = undefined
            })
    }
})

$('#crop-image-modal').on({
    'shown.bs.modal': () => {
        cropper = new Cropper(cropImgElement, {
            viewMode: 2,
            responsive: true,
            aspectRatio: 1,
            dragMode: 'move',
            ready: () => {
                const imgData = cropper.getImageData()
                const maxDimension = Math.min(imgData.naturalHeight, imgData.naturalWidth)
                if (prevCropData) {
                    cropper?.setData(prevCropData)
                    return
                }
                cropper?.setData({
                    x: (imgData.naturalWidth / 2) - (maxDimension / 2),
                    y: (imgData.naturalHeight / 2) - (maxDimension / 2),
                })
                cropper?.setData({
                    width: maxDimension
                })
            }
        })
    },
    'hidden.bs.modal': () => {
        if (cropper) {
            cropper.destroy()
        }
    }
})

$('#confirm-crop-btn').on('click', () => {
    if (!cropper?.cropped) {
        return
    }
    prevCropData = cropper.getData()
    cropper.getCroppedCanvas().toBlob(setFileToDropzone)
})

function setFileToDropzone(img) {
    if (!img) {
        return
    }
    const file = new File([img], '/.png', {type: 'image/png'})
    Dropzone.forElement('#img-dropzone')?.removeAllFiles()
    Dropzone.forElement('#img-dropzone')?.addFile(file)
    bootstrap.Modal.getInstance($('#crop-image-modal'))?.hide()
}