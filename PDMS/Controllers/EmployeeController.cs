using System.Diagnostics;
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

    static EmployeeController() {
        StaticRootDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "PDMS");
    }

    public EmployeeController(PdmsDbContext context, IUserService userService, UserManager<User> userManager) {
        _context = context;
        _userService = userService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<List<Employee>>> GetEmployees() {
        return await _context.Employees.ToListAsync();
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
                UserId = newUser.Id,
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
                CreatedById = creator.Employee.EmpId
            };
            await _context.Employees.AddAsync(newEmp);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateEmployee), routeValues: new { id = newEmp.EmpId }, newEmp);
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