using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDMS.Domain.Abstractions;
using PDMS.Domain.Entities;
using PDMS.Models;
using PDMS.Shared;
using PDMS.Shared.Constants;
using PDMS.Shared.DTO.OrderDetail;
using PDMS.Shared.DTO.OrderTicket;
using PDMS.Shared.Enums;

namespace PDMS.Controllers;

[Route("[controller]")]
[ApiController]
public class OrderTicketController : Controller {
    private readonly IPdmsDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public OrderTicketController(IPdmsDbContext context, UserManager<User> userManager, IMapper mapper) {
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
    }

    [HttpPost("Create")]
    [Authorize(Roles = RolesConstants.CUSTOMER)]
    public async Task<ActionResult<OrderTicketDto>> CreateOrderTicket([FromForm] CreateOrderTicketDto dto) {
        try {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound();
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (customer == null) {
                return NotFound();
            }

            var orderDetails = new List<OrderDetail>();
            double totalPay = 0;
            foreach (var cartItem in dto.CartItems) {
                if (cartItem.Quantity <= 0) {
                    return ValidationError.BadRequest400($"Sản phẩm có số lượng không hợp lệ");
                }

                var product = await _context.Products.FindAsync(cartItem.ProductId);
                if (product == null) {
                    return ValidationError.BadRequest400($"Sản phẩm có id {cartItem.ProductId} không tồn tại");
                }

                if (product.Quantity <= 0) {
                    return ValidationError.BadRequest400($"Sản phẩm '{product.ProductName}' đã hết hàng");
                }

                orderDetails.Add(
                    new OrderDetail() {
                        ProductId = product.ProductId,
                        Quantity = cartItem.Quantity,
                        Price = product.Price,
                        Total = product.Price * cartItem.Quantity,
                        Product = product
                    }
                );
                totalPay += product.Price * cartItem.Quantity;
            }

            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var randomString = Utils.RandomString(3, Utils.UppercaseChars, Utils.Numbers);
            var orderTicket = new OrderTicket() {
                OrderCode = $"O{Utils.LongToBase32(currentTime)}{randomString}",
                CustomerId = customer.CustomerId,
                CustomerName = $"{user.LastName} {user.FirstName}".Trim(),
                CustomerPhone = user.PhoneNumber,
                ExpectedOrderDate = dto.ExpectedOrderDate,
                ExpectedReceiveDate = dto.ExpectedReceiveDate,
                CreatedDate = DateTime.Now,
                Status = OrderTicketStatus.Pending,
                TotalPay = totalPay,
                Address = dto.Address.Trim(),
                OrderDetails = orderDetails,
                Note = dto.Note
            };

            var notification = new Notification() {
                OrderTicket = orderTicket,
                EmployeeId = customer.EmpId,
                Title = "Khách hàng đã tạo phiếu đặt hàng",
                Content = $"{orderTicket.CustomerName} đã tạo phiếu đặt hàng mới",
                Status = false,
                CustomerCreateId = customer.CustomerId,
                Time = orderTicket.CreatedDate
            };

            await _context.OrderTickets.AddAsync(orderTicket);
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            return _mapper.Map<OrderTicketDto>(orderTicket);
        } catch (Exception e) {
            return ValidationError.InternalServerError500(e.Message);
        }
    }
}