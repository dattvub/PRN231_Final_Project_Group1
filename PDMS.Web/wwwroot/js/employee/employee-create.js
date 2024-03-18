const mainForm = $('#mainForm')
const avatarInput = $('#avatar')
const [entranceDateInput, exitDateInput] = $('.datetimepicker').flatpickr({
    disableMobile: true,
    dateFormat: 'd-m-Y',
    locale: 'vn'
})
const roleSelect = new Choices('#role-select', {
    placeholder: true,
    allowHTML: false,
    searchEnabled: false,
    searchChoices: false,
})
const groupSelect = new Choices('#group-select', {
    placeholder: true,
    allowHTML: false,
    itemSelectText: 'Nhấn để chọn',
    noResultsText: 'Không có kết quả',
    noChoicesText: 'Không có lựa chọn'
})
const entranceDateSwitch = $('#entranceDateSwitch')
const exitDateSwitch = $('#exitDateSwitch')
const cropImgElement = $('#img-crop')[0]
let cropper
let prevCropData
let selectedImage
let isCodeValid = false
let checkCodeTimer
let isCodeChanged = true

if (window.empId) {
    initEditPage()
} else {
    setBreadcrumb(
        {
            route: 'Trang chủ',
            href: '/'
        },
        {
            route: 'Danh sách nhân viên',
            href: '/Employee'
        },
        'Thêm nhân viên mới'
    )
}

$(roleSelect.passedElement.element).on({
    choice: e => {
        $('#group-select-box').toggleClass('d-none', e.detail.choice.value === 'DIRECTOR')
    }
})
groupSelect.setChoices(async () => {
    try {
        const r = await fetchWithCredentials('http://localhost:5000/Group')
        const items = await r.json()
        return items.map(x => ({
            value: x.groupId,
            label: x.groupName
        }))
    } catch (err) {
        console.error(err);
    }
});
$('#emp-code').on('input', e => {
    isCodeChanged = true
    const val = $('#emp-code').val().trim()
    if (val.length < 3) {
        isCodeValid = false
        return
    }
    if (checkCodeTimer) {
        clearTimeout(checkCodeTimer)
    }
    checkCodeTimer = setTimeout(async () => {
        await checkDuppCode(val)
        $('#emp-code').valid()
    }, 400)
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
        'emp-code': {
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
            required: !window.empId,
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

    const code = $('#emp-code').val().trim()
    if (isCodeChanged) {
        await checkDuppCode(code)
    }
    if (!mainForm.valid()) {
        return
    }

    const form = new FormData()
    form.set('firstName', $('#firstname').val().trim())
    form.set('lastName', $('#lastname').val().trim())
    form.set('code', code)
    form.set('gender', $('[name=gender]:checked').val())
    form.set('email', $('#email').val().trim())
    form.set('password', $('#password').val() || '_')
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
    if (selectedImage && !selectedImage.name.startsWith('*.')) {
        form.set('image', selectedImage)
    } else if (!selectedImage && window.emp.imageUrl) {
        form.set('image', new File([], 'image.png', {type: 'image/png'}))
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
    const role = roleSelect.getValue().value
    if (role) {
        form.set('role', role)
    }
    const group = groupSelect.getValue().value
    if (group) {
        form.set('groupId', group)
    }

    if (!window.empId) {
        const r = await fetchWithCredentials('http://localhost:5000/Employee/Create', {
            method: 'POST',
            body: form
        })
        if (r.ok) {
            window.location.href = '/Employee'
        } else {
            if (!r.ok) {
                const json = await r.json()
                showToast('Thêm nhân viên nhân viên', json.errors.join('. '))
            }
        }
    } else {
        console.log(...form)
        const r = await fetchWithCredentials(`http://localhost:5000/Employee/${window.empId}`, {
            method: 'PUT',
            body: form,
        })
        if (!r.ok) {
            const json = await r.json()
            showToast('Cập nhật thông tin nhân viên', json.errors.join('. '))
        } else {
            window.location.href = `/Employee/${window.empId}`
        }
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


async function checkDuppCode(val) {
    const form = new FormData()
    form.set('code', val.trim())
    if (window.empId) {
        form.set('id', window.empId)
    }
    const r = await fetchWithCredentials('http://localhost:5000/Employee/CheckCode', {
        method: 'POST',
        body: form
    })
    isCodeValid = (await r.text()) === 'true'
    isCodeChanged = false
}

async function initEditPage() {
    const r = await fetchWithCredentials(`http://localhost:5000/Employee/${window.empId}`)
    const emp = await r.json()
    window.emp = emp
    console.log(emp)
    setBreadcrumb(
        {
            route: 'Trang chủ',
            href: '/'
        },
        {
            route: 'Danh sách nhân viên',
            href: '/Employee'
        },
        {
            route: emp.empName,
            href: `/Employee/${emp.empId}`
        },
        'Cập nhật thông tin'
    )

    if (emp.imageUrl) {
        const url = `http://localhost:5000/${emp.imageUrl}`
        fetch(url)
            .then(r => r.blob())
            .then(blob => {
                const file = new File([blob], `*.${emp.imageUrl.split('/').pop()}`, {type: blob.type})
                Dropzone.forElement('#img-dropzone')?.addFile(file)
            })
    }
    $('#lastname').val(emp.lastName)
    $('#firstname').val(emp.firstName)
    $('#emp-code').val(emp.userName)
    $(`#gender-${emp.gender ? 'male' : 'female'}`)[0].checked = true
    $('#email').val(emp.email)
    $('#phone').val(emp.phoneNumber)
    $('#address').val(emp.address)
    $('#position').val(emp.position)
    $('#department').val(emp.department)
    roleSelect.setChoiceByValue(emp.role)
    if (emp.groupId) {
        groupSelect.setChoiceByValue(emp.groupId)
    }
    if (emp.role === 'DIRECTOR') {
        $('#group-select-box').addClass('d-none')
    } else if (emp.groupId) {
        groupSelect.setChoiceByValue(emp.groupId)
    }
    if (emp.entranceDate) {
        entranceDateSwitch[0].checked = true
        entranceDateSwitch.trigger('change')
        entranceDateInput.setDate(new Date(emp.entranceDate))
    }
    if (emp.exitDate) {
        exitDateSwitch[0].checked = true
        exitDateSwitch.trigger('change')
        exitDateInput.setDate(new Date(emp.exitDate))
    }
}

function setFileToDropzone(img) {
    if (!img) {
        return
    }
    const file = new File([img], '/.png', {type: 'image/png'})
    Dropzone.forElement('#img-dropzone')?.removeAllFiles()
    Dropzone.forElement('#img-dropzone')?.addFile(file)
    bootstrap.Modal.getInstance($('#crop-image-modal'))?.hide()
}