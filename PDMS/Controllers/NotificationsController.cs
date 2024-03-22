using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDMS.Domain.Abstractions;
using PDMS.Domain.Entities;
using PDMS.Models;
using PDMS.Shared.Constants;
using PDMS.Shared.DTO.Brand;
using PDMS.Shared.DTO.Notifications;
using PDMS.Shared.DTO.Product;
using PDMS.Shared.Exceptions;

namespace PDMS.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotificationsController : Controller
    {
        private readonly IPdmsDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public NotificationsController(IPdmsDbContext context, IMapper mapper, UserManager<User> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet("list")]
        [Authorize(Roles = $"{RolesConstants.DIRECTOR},{RolesConstants.SUPERVISOR},{RolesConstants.ACCOUNTANT},{RolesConstants.SALEMAN},{RolesConstants.CUSTOMER}")]
        public async Task<ActionResult<List<NotificationDto>>> GetNotifications()
        {
            List<Notification> notifications = await _context.Notifications
                .Where(x => x.Status)
                .OrderByDescending(x => x.NotiId)
                .ToListAsync();
            return _mapper.Map<List<NotificationDto>>(notifications);

        }

        [HttpPost("create")]
        [Authorize(Roles = $"{RolesConstants.DIRECTOR},{RolesConstants.SUPERVISOR},{RolesConstants.ACCOUNTANT},{RolesConstants.SALEMAN},{RolesConstants.CUSTOMER}")]
        public async Task<ActionResult<NotificationDto>> CreateNotification([FromForm] CreateNotificationDto createNotification)
        {
            var userId = _userManager.GetUserId(User);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId.Equals(userId));
            if (customer == null)
            {
                return ValidationError.BadRequest400("Không có Customer hợp lệ");
            }

            var notification = _mapper.Map<Notification>(createNotification);
            notification.CustomerCreateId = customer.CustomerId;
            notification.EmployeeId = customer.EmpId;
            await _context.Notifications.AddAsync(notification);
            try
            {
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(CreateNotification),
                    new { id = notification.NotiId },
                    _mapper.Map<NotificationDto>(notification));
            }
            catch (BlameClient e)
            {
                return ValidationError.BadRequest400(e.Message);
            }
            catch (Exception e)
            {
                return ValidationError.InternalServerError500(e.Message);
            }
        }

        [HttpPut("update/{id:int}")]
        [Authorize(Roles = $"{RolesConstants.DIRECTOR},{RolesConstants.SUPERVISOR},{RolesConstants.ACCOUNTANT},{RolesConstants.SALEMAN},{RolesConstants.CUSTOMER}")]
        public async Task<ActionResult<NotificationDto>> UpdateNotification(
            [FromRoute] int id,
            [FromForm] CreateNotificationDto createNotification)
        {
            try
            {
                //var userId = _userManager.GetUserId(User);
                //var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId.Equals(userId));
                //if (customer == null)
                //{
                //    return ValidationError.BadRequest400("Không có Customer hợp lệ");
                //}
                var notif = await _context.Notifications.FirstOrDefaultAsync(x => x.Status && x.NotiId == id);
                if (notif == null)
                {
                    return ValidationError.BadRequest400($"Notification with id {id} is not exist");
                }

                var newNotification = _mapper.Map<Notification>(createNotification);

                notif.OrderId = newNotification.OrderId;
                notif.EmployeeId = newNotification.EmployeeId;
                notif.Title = newNotification.Title;
                notif.Content = newNotification.Content;
                notif.CustomerCreateId = newNotification.CustomerCreateId;

                await _context.SaveChangesAsync();
                return _mapper.Map<NotificationDto>(notif);
            }
            catch (BlameClient e)
            {
                return ValidationError.BadRequest400(e.Message);
            }
            catch (Exception e)
            {
                return ValidationError.InternalServerError500(e.Message);
            }


        }

        [HttpDelete("delete/{id:int}")]
        [Authorize(Roles = $"{RolesConstants.DIRECTOR},{RolesConstants.SUPERVISOR},{RolesConstants.ACCOUNTANT},{RolesConstants.SALEMAN},{RolesConstants.CUSTOMER}")]
        public async Task<ActionResult<NotificationDto>> DeleteNotification([FromRoute] int id)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(x => x.NotiId == id && x.Status);
            if (notification == null)
            {
                return ValidationError.BadRequest400($"Notification with id {id} is not exist");
            }
            notification.Status = false;
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return ValidationError.InternalServerError500("Error occur while delete notification");
            }

            return Ok(_mapper.Map<NotificationDto>(notification));
        }

    }
}
