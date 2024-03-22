using AutoMapper;
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
using PDMS.Shared.DTO;
using PDMS.Shared.DTO.Customer;
using PDMS.Shared.DTO.Employee;
using PDMS.Shared.Exceptions;
using System.Security.Claims;

namespace PDMS.Controllers;

[Route("[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly PdmsDbContext _context;
    private readonly IUserService _userService;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public CustomerController(
        PdmsDbContext context,
        IUserService userService,
        UserManager<User> userManager,
        IMapper mapper
    )
    {
        _context = context;
        _userService = userService;
        _userManager = userManager;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = $"{RolesConstants.DIRECTOR},{RolesConstants.SUPERVISOR},{RolesConstants.SALEMAN},{RolesConstants.ACCOUNTANT}")]
    public async Task<ActionResult<PaginationDto<CustomerDto>>> GetCustomers([FromQuery] GetCustomersDto dto)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return ValidationError.BadRequest400("Token lỗi");
        }

        var user = await _userManager.Users
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x => x.Id == userId);
        if (user?.Employee == null)
        {
            return ValidationError.BadRequest400("Khách hàng không tồn tại");
        }
        var roleTemp = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value ?? string.Empty;
        IQueryable<Customer> query;
        if (roleTemp == RolesConstants.DIRECTOR)
        {
            query = _context.Customers
                .Include(x => x.User)
                .AsQueryable();
        }
        else
        {
            var tempCus = await _context.Employees.FirstOrDefaultAsync(x => x.UserId == userId);
            query = _context.Customers
                .Include(x => x.User)
                .Where(x => tempCus.EmpId == x.EmpId)
                .AsQueryable();
        }


        var total = await query.CountAsync();
        var customers = await query
            .Skip((dto.Page - 1) * dto.Quantity)
            .Take(dto.Quantity)
            //.Select(x => _mapper.Map<CustomerDto>(x))
            .ToListAsync();
        var dtoOutput = _mapper.Map<List<CustomerDto>>(customers);
        return new PaginationDto<CustomerDto>(dtoOutput, total)
        {
            Page = dto.Page,
            ItemsPerPage = dto.Quantity,
            Query = dto.Query
        };
    }


    [HttpGet("{id:int:min(1)}")]
    [Authorize()]
    public async Task<ActionResult<CustomerDto>> GetCustomer([FromRoute] int id)
    {
        var customer = await _context.Customers
            .Include(x => x.CustomerType)
            .Include(x => x.CustomerGroup)
            .Include(x => x.User)
            .ThenInclude(x => x.UserRoles)
            .FirstOrDefaultAsync(x => x.CustomerId == id);

        if (customer == null)
        {
            return NotFound();
        }

        var requesterRole = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value ?? string.Empty;
        var requesterId = _userManager.GetUserId(User);

        if (string.IsNullOrWhiteSpace(requesterId) || string.IsNullOrWhiteSpace(requesterRole))
        {
            return Unauthorized();
        }

        if (requesterRole != RolesConstants.CUSTOMER && requesterRole != RolesConstants.DIRECTOR)
        {
            var managerId = await _context.Employees
                .Where(x => x.UserId == requesterId)
                .Select(x => x.EmpId)
                .FirstOrDefaultAsync();
            if (managerId == null || customer.EmpId != managerId)
            {
                return Unauthorized();
            }
        }
        else if (requesterRole == RolesConstants.CUSTOMER && customer.UserId != requesterId)
        {
            return Unauthorized();
        }

        var customerRole = await _userManager.GetRolesAsync(customer.User);
        var dto = _mapper.Map<CustomerDto>(customer);
        dto.Role = customerRole[0];

        return dto;
    }

    [HttpPost("CheckCode")]
    [Authorize(
    Roles =
        $"{RolesConstants.DIRECTOR},{RolesConstants.SUPERVISOR},{RolesConstants.SALEMAN},{RolesConstants.ACCOUNTANT}"
)]
    public async Task<ActionResult<bool>> CheckCode([FromForm] CheckCusCodeDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Code) || dto.Code.Length < 3)
        {
            return false;
        }

        var cus = await _context.Customers.FirstOrDefaultAsync(x => x.CustomerCode == dto.Code);
        return cus == null || cus.CustomerId == dto.Id;
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
            var newUser = await _userService.CreateUser(user, dto.Password, RolesConstants.CUSTOMER, creator);
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

    [HttpPut("{id:int:min(1)}")]
    [Authorize(Roles = $"{RolesConstants.DIRECTOR},{RolesConstants.SALEMAN}")]
    public async Task<ActionResult<CustomerDto>> EditCustomer(
        [FromRoute] int id,
        [FromForm] CreateCustomerDto dto
    )
    {
        var cus = await _context.Customers.FindAsync(id);
        if (cus == null)
        {
            return NotFound($"Khách hàng với id {id} không tồn tại");
        }

        var user = await _userManager.FindByIdAsync(cus.UserId);
        if (user == null)
        {
            return NotFound($"Tài khoản với id {cus.UserId} không tồn tại");
        }
        try
        {
            var editor = await _userManager.Users.Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Id == _userManager.GetUserId(User));
            if (editor?.Employee == null)
            {
                throw new BlameClient("Yêu cầu tạo từ một tài khoản không tồn tại");
            }
            user.UserName = dto.Code.Trim();
            user.FirstName = dto.FirstName.Trim();
            user.LastName = dto.LastName.Trim();
            user.Email = dto.Email.Trim();
            user.PhoneNumber = dto.Phone?.Trim();
            await _userManager.UpdateAsync(user);

            cus.CustomerCode = dto.Code;
            cus.CustomerName = $"{dto.LastName} {dto.FirstName}".Trim();
            cus.Address = dto.Address?.Trim();
            cus.TaxCode = dto.TaxCode?.Trim();
            cus.CustomerGroupId = dto.CustomerGroupId;
            cus.CustomerTypeId = dto.CustomerTypeId;
            cus.EmpId = editor.Employee.EmpId;
            _context.Customers.Update(cus);

            await _context.SaveChangesAsync();
            return _mapper.Map<CustomerDto>(cus);

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
}
