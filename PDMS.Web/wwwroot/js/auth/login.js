const loginForm = $('#login-form')

loginForm.on('submit', e => {
    $('.alert-danger').remove()
    e.preventDefault()
    const email = $('#email').val()
    const password = $('#password').val()
    const rememberMe = $('#remember-me')[0].checked

    if (loginForm.find(':invalid').length > 0) {
        loginForm.addClass('was-validated')
        return
    }

    const formData = new FormData();
    formData.set('email', email);
    formData.set('password', password);
    formData.set('rememberMe', rememberMe)
    // fetch('http://localhost:5000/auth/login', {
    //     method: 'POST',
    //     body: formData,
    //     credentials: 'include',
    //     headers: {}
    // })
    $.ajax({
        url: 'http://localhost:5000/auth/login',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        xhrFields: {
            withCredentials: true,
        },
        success: function () {
            window.location.replace("/")
        },
        error: function (xhr) {
            $($('#error-list')[0].content.cloneNode(true))
                .find(".err-list")
                .append((xhr.responseJSON?.errors || ['Có lỗi xuất hiện trong quá trình đăng nhập']).map(x => $('<li></li>').text(x)))
                .end()
                .insertBefore($("#submit-btn").parent())
        }
    })
})