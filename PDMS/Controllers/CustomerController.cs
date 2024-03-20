using ImageMagick;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDMS.Domain.Entities;
using PDMS.Infrastructure.Persistence;
using PDMS.Models;
using PDMS.Services.Interface;
using PDMS.Shared.Constants;
using PDMS.Shared.DTO.Customer;
using PDMS.Shared.Exceptions;

namespace PDMS.Controllers;

[Route("[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly PdmsDbContext _context;
    private readonly IUserService _userService;
    private readonly UserManager<User> _userManager;


    public CustomerController(PdmsDbContext context, IUserService userService, UserManager<User> userManager)
    {
        _context = context;
        _userService = userService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<List<Customer>>> GetCustomers()
    {
        return await _context.Customers.ToListAsync();
    }

    [HttpPost("Create")]
    [Authorize(Roles = RolesConstants.DIRECTOR)]
    public async Task<ActionResult<Customer>> CreateCustomer(
        [FromForm] CreateCustomerDto dto
    )
    {
        string? newUserId = null;
        try
        {
            var creator = await _userManager.Users.Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Id == _userManager.GetUserId(User));
            if (creator?.Employee == null)
            {
                throw new BlameClient("Yêu cầu tạo từ một tài khoản không tồn tại");
            }
            var user = new User()
            {
                UserName = dto.Code.Trim(),
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = dto.Email.Trim(),
                PhoneNumber = dto.Phone?.Trim(),
            };
            var newUser = await _userService.CreateUser(user, dto.Password, dto.Role, creator);
            newUserId = newUser.Id;

            var newCus = new Customer()
            {
                UserId = newUser.Id,
                CustomerCode = dto.Code.Trim(),
                CustomerName = $"{dto.LastName} {dto.FirstName}".Trim(),
                Address = dto.Address?.Trim(),
                TaxCode = dto.TaxCode.Trim(),   
                CustomerTypeId = dto.CustomerTypeId,
                CustomerGroupId = dto.CustomerGroupId,
                EmpId = creator.Employee.EmpId
            };
            await _context.Customers.AddAsync(newCus);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateCustomer), routeValues: new { id = newCus.CustomerId }, newCus);
        }
        catch (Exception e)
        {
            if (newUserId != null)
            {
                await _userService.DeleteUser(newUserId);
            }

            return e is BlameClient
                ? ValidationError.BadRequest400(e.Message)
                : ValidationError.InternalServerError500(e.Message);
        }
    }
}
