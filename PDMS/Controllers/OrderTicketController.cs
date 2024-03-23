using System.Security.Claims;
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
using PDMS.Shared.DTO;
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

    [HttpPatch("{id:int:min(1)}")]
    [Authorize(Roles = $"{RolesConstants.CUSTOMER},{RolesConstants.SALEMAN},{RolesConstants.DIRECTOR}")]
    public async Task<ActionResult<OrderTicketStatus>> UpdateStatus([FromRoute] int id, [FromQuery] string s) {
        var userRole = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value ?? string.Empty;
        if (!Enum.TryParse<OrderTicketStatus>(s, out var status)) {
            return ValidationError.BadRequest400("????");
        }

        if (userRole == RolesConstants.CUSTOMER
            && status != OrderTicketStatus.Cancel
            && status != OrderTicketStatus.Received) {
            return ValidationError.BadRequest400("????");
        }

        if (userRole != RolesConstants.CUSTOMER
            && status != OrderTicketStatus.Approved
            && status != OrderTicketStatus.Rejected) {
            return ValidationError.BadRequest400("????");
        }

        var order = await _context.OrderTickets.FindAsync(id);
        if (order == null) {
            return ValidationError.BadRequest400("????");
        }

        order.Status = status;
        _context.OrderTickets.Update(order);
        await _context.SaveChangesAsync();
        return order.Status;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<PaginationDto<OrderTicketDto>>> GetOrderTickets([FromQuery] GetOrderTicketDto dto) {
        var user = await _userManager.GetUserAsync(User);
        var userRole = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value ?? string.Empty;
        if (user == null || string.IsNullOrWhiteSpace(userRole)) {
            return Unauthorized();
        }

        var query = _context.OrderTickets
            .Include(x => x.OrderDetails)
            .ThenInclude(x => x.Product)
            .AsQueryable();

        if (userRole == RolesConstants.CUSTOMER) {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (customer == null) {
                return Unauthorized();
            }

            query = query.Where(x => x.CustomerId == customer.CustomerId);
        } else if (userRole == RolesConstants.SALEMAN) {
            var emp = await _context.Employees.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (emp == null) {
                return Unauthorized();
            }

            var customerIdList = await _context.Customers
                .Where(x => x.EmpId == emp.EmpId)
                .Select(x => x.CustomerId)
                .ToListAsync();

            query = query.Where(x => customerIdList.Contains(x.CustomerId));
        } else if (userRole == RolesConstants.SUPERVISOR) {
            var supervisor = await _context.Employees.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (supervisor?.GroupId == null) {
                return Unauthorized();
            }

            var empIdList = await _context.Employees
                .Where(x => x.GroupId == supervisor.GroupId)
                .Select(x => x.EmpId)
                .ToListAsync();

            var customerIdList = await _context.Customers
                .Where(x => x.EmpId != null && empIdList.Contains((int)x.EmpId))
                .Select(x => x.CustomerId)
                .ToListAsync();

            query = query.Where(x => customerIdList.Contains(x.CustomerId));
        }

        var total = await query.CountAsync();
        var orderTickets = await query
            .Skip((dto.Page - 1) * dto.Quantity)
            .Take(dto.Quantity)
            .OrderByDescending(x => x.CreatedDate)
            .Select(x => _mapper.Map<OrderTicketDto>(x))
            .ToListAsync();

        return new PaginationDto<OrderTicketDto>(orderTickets, total) {
            Page = dto.Page,
            ItemsPerPage = dto.Quantity,
            Query = dto.Query
        };
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