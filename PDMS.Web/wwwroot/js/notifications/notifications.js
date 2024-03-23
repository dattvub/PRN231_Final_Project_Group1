setTimeout(() => {
    window.fullItems.forEach(async e => {
        if (user.associationId === e.employeeId ) {
            item = `<a class="border-bottom-0 rounded-0 border-x-0 border-300 align-items-center" id="notifId${e.notiId}" href="/OrderTicket?showOrder=${e.orderId}">` +
                `<div class="notification-avatar">` +
                `<div class="avatar avatar-xl me-3">` +
                `<img class="rounded-circle" src="/images/blank_avatar.jpg" alt="" />` +
                `</div>` +
                `</div>` +
                `<div class="notification-body">` +
                `<div class="notification-flush">` +
                `<p class="mb-1"><em>${e.title}</em></p>` +
                `</div>` +
                `<p class="mb-1"><strong></strong> ${e.content}</p>`

            let thing = ''

            const currentMils = new Date().getTime()
            const notifyMils = new Date(e.time).getTime()
            const secondsBetween = Math.floor(((currentMils - notifyMils) / 1000))
            if (secondsBetween < 60) {
                thing = 'Mới đây'
            } else if (secondsBetween >= 60 && secondsBetween < 3600) {
                thing = Math.floor((secondsBetween / 60)) + 'm'
            } else if (secondsBetween >= 3600 && secondsBetween < 86400) {
                thing = Math.floor((secondsBetween / 3600)) + 'h'
            } else {
                thing = Math.floor((secondsBetween / 86400)) + 'd'
            }

            item += `<span class="notification-time"><span class="me-2" role="img" aria-label="Emoji">📢</span>${thing}</span>`
            const newDiv = $('<div>').html(item)
            $('#list-notifications').append(newDiv)
            if (e.status === true) {
                $(`#notifId${e.notiId}`).addClass('notification')
            } else {
                $(`#notifId${e.notiId}`).addClass('notification notification-unread')
            }
            $(`#notifId${e.notiId}`).on('click', async () => {
                const r = await fetchWithCredentials(`http://localhost:5000/Notifications/delete/${e.notiId}`, {
                    method: 'DELETE'})
                if (!r.ok) {
                    const json = await r.json()
                    showToast('Notification', json.errors.join('. '))
                    return
                } else {
                    window.location.replace(`http://localhost:5238/Notifications`)
                }
            })
        }
        
    })
}, 1000)





