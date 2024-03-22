using System.Diagnostics;
using System.Security.Claims;
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
using PDMS.Shared.DTO.Employee;
using PDMS.Shared.Exceptions;

namespace PDMS.Controllers;

[Route("[controller]")]
[ApiController]
public class EmployeeController : ControllerBase {
    private static readonly string StaticRootDir;
    private const int MaxImageSquareSize = 800;
    private readonly PdmsDbContext _context;
    private readonly IUserService _userService;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    static EmployeeController() {
        StaticRootDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "PDMS");
    }

    public EmployeeController(
        PdmsDbContext context,
        IUserService userService,
        UserManager<User> userManager,
        IMapper mapper
    ) {
        _context = context;
        _userService = userService;
        _userManager = userManager;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = $"{RolesConstants.DIRECTOR},{RolesConstants.SUPERVISOR}")]
    public async Task<ActionResult<PaginationDto<EmployeeDto>>> GetEmployees([FromQuery] GetEmployeesDto dto) {
        if (dto.InGroup < -2) {
            return ValidationError.BadRequest400("Nhóm có id không không hợp lệ");
        }

        var userId = _userManager.GetUserId(User);
        if (userId == null) {
            return ValidationError.BadRequest400("Token lỗi");
        }

        var user = await _userManager.Users
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x => x.Id == userId);
        if (user?.Employee == null) {
            return ValidationError.BadRequest400("Nhân viên không tồn tại");
        }

        if (await _userManager.IsInRoleAsync(user, RolesConstants.SUPERVISOR)) {
            dto.InGroup = user.Employee.GroupId ?? 0;
        }

        if (dto.InGroup == 0) {
            return new PaginationDto<EmployeeDto>(new List<EmployeeDto>());
        }

        var query = _context.Employees
            .Include(x => x.User)
            .AsQueryable();

        switch (dto.InGroup) {
            case -2:
                query = query.Where(x => x.GroupId == null);
                break;
            case > 0:
                query = query.Where(x => x.GroupId == dto.InGroup);
                break;
        }

        if (!string.IsNullOrWhiteSpace(dto.Query)) {
            query = query.Where(x => x.EmpName.Contains(dto.Query.Trim()));
        }

        var total = await query.CountAsync();
        var employees = await query
            .Skip((dto.Page - 1) * dto.Quantity)
            .Take(dto.Quantity)
            .Select(x => _mapper.Map<EmployeeDto>(x))
            .ToListAsync();

        return new PaginationDto<EmployeeDto>(employees, total) {
            Page = dto.Page,
            ItemsPerPage = dto.Quantity,
            Query = dto.Query
        };
    }

    [HttpGet("ByCustomer")]
    [Authorize(Roles = RolesConstants.CUSTOMER)]
    public async Task<ActionResult<EmployeeDto>> GetEmployeeByCustomer() {
        var cusId = _userManager.GetUserId(User);
        var customer = await _context.Customers
            .Include(x => x.Employee.Group)
            .Include(x => x.Employee.User)
            .FirstOrDefaultAsync(x => x.UserId == cusId);
        if (customer == null) {
            return NotFound();
        }

        return _mapper.Map<EmployeeDto>(customer.Employee);
    }

    [HttpGet("{id:int:min(1)}")]
    [Authorize()]
    public async Task<ActionResult<EmployeeDto>> GetEmployee([FromRoute] int id) {
        var emp = await _context.Employees
            .Include(x => x.Group)
            .Include(x => x.User)
            .ThenInclude(x => x.UserRoles)
            .FirstOrDefaultAsync(x => x.EmpId == id);
        if (emp == null) {
            return NotFound();
        }

        var requesterRole = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value ?? string.Empty;
        var requesterId = _userManager.GetUserId(User);

        if (string.IsNullOrWhiteSpace(requesterId) || string.IsNullOrWhiteSpace(requesterRole)) {
            return Unauthorized();
        }

        if (requesterRole == RolesConstants.CUSTOMER) {
            var managerId = await _context.Customers
                .Where(x => x.UserId == requesterId)
                .Select(x => x.EmpId)
                .FirstOrDefaultAsync();
            if (managerId == null || emp.EmpId != managerId) {
                return Unauthorized();
            }
        } else if (requesterRole != RolesConstants.DIRECTOR && emp.UserId != requesterId) {
            return Unauthorized();
        }

        var empRole = await _userManager.GetRolesAsync(emp.User);
        var dto = _mapper.Map<EmployeeDto>(emp);
        dto.Role = empRole[0];
        return dto;
    }

    [HttpPost("CheckCode")]
    [Authorize(Roles = RolesConstants.DIRECTOR)]
    public async Task<ActionResult<bool>> CheckCode([FromForm] CheckCodeDto dto) {
        if (string.IsNullOrWhiteSpace(dto.Code) || dto.Code.Length < 3) {
            return false;
        }

        var emp = await _context.Employees.FirstOrDefaultAsync(x => x.EmpCode == dto.Code);
        return emp == null || emp.EmpId == dto.Id;
    }

    [HttpPost("Create")]
    [Authorize(Roles = RolesConstants.DIRECTOR)]
    public async Task<ActionResult<Employee>> CreateEmployee(
        [FromForm] CreateEmployeeDto dto,
        IFormFile? image
    ) {
        string? fullPath = null;
        string? newUserId = null;
        try {
            string? relImagePath = null;
            var creator = await _userManager.Users.Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Id == _userManager.GetUserId(User));
            if (creator?.Employee == null) {
                throw new BlameClient("Yêu cầu tạo từ một tài khoản không tồn tại");
            }

            DateTime? entranceDate = null;
            DateTime? exitDate = null;
            if (dto.EntranceDate != null) {
                entranceDate = ParseDateString(dto.EntranceDate);
            }

            if (dto.ExitDate != null) {
                exitDate = ParseDateString(dto.ExitDate);
            }

            if (image != null) {
                if (!image.ContentType.StartsWith("image/")) {
                    return ValidationError.BadRequest400("Image không phải ảnh");
                }

                var processImg = new MagickImage(image.OpenReadStream());
                processImg.Format = MagickFormat.Jpg;
                processImg.ColorSpace = ColorSpace.sRGB;
                if (processImg.Width != processImg.Height) {
                    var maxDimension = Math.Min(processImg.Width, processImg.Height);
                    processImg.Crop(maxDimension, maxDimension, Gravity.Center);
                }

                if (processImg.Width > MaxImageSquareSize) {
                    processImg.Resize(MaxImageSquareSize, MaxImageSquareSize);
                }

                var fileName = $"{Guid.NewGuid():D}.jpg";
                fullPath = Path.Combine(StaticRootDir, "images", fileName);
                await processImg.WriteAsync(fullPath, MagickFormat.Jpg);
                if (System.IO.File.Exists(fullPath)) {
                    relImagePath = Path
                        .Combine("files", "images", fileName)
                        .Replace(Path.DirectorySeparatorChar, '/');
                }
            }

            var user = new User() {
                UserName = dto.Code.Trim(),
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = dto.Email.Trim(),
                PhoneNumber = dto.Phone?.Trim(),
                ImageUrl = relImagePath
            };
            var newUser = await _userService.CreateUser(user, dto.Password, dto.Role, creator);
            newUserId = newUser.Id;

            var newEmp = new Employee() {
                User = newUser,
                EmpCode = dto.Code,
                EmpName = $"{dto.LastName} {dto.FirstName}".Trim(),
                Gender = dto.Gender,
                Status = true,
                Position = string.IsNullOrWhiteSpace(dto.Position) ? null : dto.Position,
                Department = string.IsNullOrWhiteSpace(dto.Department) ? null : dto.Department,
                Address = dto.Address?.Trim(),
                EntranceDate = entranceDate,
                ExitDate = exitDate,
                CreateDate = newUser.CreatedDate ?? DateTime.Now,
                CreatedById = creator.Employee.EmpId,
                GroupId = dto.GroupId
            };
            await _context.Employees.AddAsync(newEmp);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(CreateEmployee), routeValues: new { id = newEmp.EmpId }, _mapper.Map<EmployeeDto>(newEmp)
            );
        } catch (Exception e) {
            if (fullPath != null) {
                System.IO.File.Delete(fullPath);
            }

            if (newUserId != null) {
                await _userService.DeleteUser(newUserId);
            }

            return e is BlameClient
                ? ValidationError.BadRequest400(e.Message)
                : ValidationError.InternalServerError500(e.Message);
        }
    }

    [HttpPut("{id:int:min(1)}")]
    [Authorize(Roles = $"{RolesConstants.DIRECTOR}")]
    public async Task<ActionResult<EmployeeDto>> EditEmployee(
        [FromRoute] int id,
        [FromForm] CreateEmployeeDto dto,
        IFormFile? image
    ) {
        var emp = await _context.Employees.FindAsync(id);
        if (emp == null) {
            return NotFound($"Nhân viên với id {id} không tồn tại");
        }

        var user = await _userManager.FindByIdAsync(emp.UserId);
        if (user == null) {
            return NotFound($"Tài khoản với id {emp.UserId} không tồn tại");
        }

        string? fullPath = null;
        var relImagePath = "";
        try {
            DateTime? entranceDate = null;
            DateTime? exitDate = null;
            if (dto.EntranceDate != null) {
                entranceDate = ParseDateString(dto.EntranceDate);
            }

            if (dto.ExitDate != null) {
                exitDate = ParseDateString(dto.ExitDate);
            }

            if (image != null) {
                if (image.Length == 0) {
                    relImagePath = null;
                } else {
                    if (!image.ContentType.StartsWith("image/")) {
                        return ValidationError.BadRequest400("Image không phải ảnh");
                    }

                    var processImg = new MagickImage(image.OpenReadStream());
                    processImg.Format = MagickFormat.Jpg;
                    processImg.ColorSpace = ColorSpace.sRGB;
                    if (processImg.Width != processImg.Height) {
                        var maxDimension = Math.Min(processImg.Width, processImg.Height);
                        processImg.Crop(maxDimension, maxDimension, Gravity.Center);
                    }

                    if (processImg.Width > MaxImageSquareSize) {
                        processImg.Resize(MaxImageSquareSize, MaxImageSquareSize);
                    }

                    var fileName = $"{Guid.NewGuid():D}.jpg";
                    fullPath = Path.Combine(StaticRootDir, "images", fileName);
                    await processImg.WriteAsync(fullPath, MagickFormat.Jpg);
                    if (System.IO.File.Exists(fullPath)) {
                        relImagePath = Path
                            .Combine("files", "images", fileName)
                            .Replace(Path.DirectorySeparatorChar, '/');
                    }
                }
            }

            user.UserName = dto.Code.Trim();
            user.FirstName = dto.FirstName.Trim();
            user.LastName = dto.LastName.Trim();
            user.Email = dto.Email.Trim();
            user.PhoneNumber = dto.Phone?.Trim();
            user.ImageUrl = relImagePath switch {
                null => null,
                "" => user.ImageUrl,
                _ => relImagePath
            };
            user.LastModifiedDate = DateTime.Now;
            user.LastModifiedBy = _userManager.GetUserId(User);
            await _userManager.UpdateAsync(user);

            emp.EmpCode = dto.Code;
            emp.EmpName = $"{dto.LastName} {dto.FirstName}".Trim();
            emp.Gender = dto.Gender;
            emp.Position = string.IsNullOrWhiteSpace(dto.Position) ? null : dto.Position;
            emp.Department = string.IsNullOrWhiteSpace(dto.Department) ? null : dto.Department;
            emp.Address = dto.Address?.Trim();
            emp.EntranceDate = entranceDate;
            emp.ExitDate = exitDate;
            emp.GroupId = dto.GroupId;
            _context.Employees.Update(emp);

            await _context.SaveChangesAsync();
            return _mapper.Map<EmployeeDto>(emp);
        } catch (BlameClient e) {
            return ValidationError.BadRequest400(e.Message);
        } catch (Exception e) {
            return ValidationError.InternalServerError500(e.Message);
        }
    }

    private static DateTime ParseDateString(string date) {
        var parts = date.Split("-");
        if (parts.Length != 3) {
            throw new Exception("Thời gian không hợp lệ");
        }

        var day = int.Parse(parts[2]);
        var month = int.Parse(parts[1]);
        var year = int.Parse(parts[0]);
        return new DateTime(year, month, day);
    }
}