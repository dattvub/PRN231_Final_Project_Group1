using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDMS.Domain.Abstractions;
using PDMS.Domain.Entities;
using PDMS.Models;
using PDMS.Shared;
using PDMS.Shared.Constants;
using PDMS.Shared.DTO.Group;

namespace PDMS.Controllers;

[ApiController]
[Route("[controller]")]
public class GroupController : ControllerBase {
    private readonly IPdmsDbContext _context;
    private readonly IMapper _mapper;

    public GroupController(IPdmsDbContext context, IMapper mapper) {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = RolesConstants.DIRECTOR)]
    public async Task<ActionResult<List<GroupDto>>> GetGroups([FromQuery] string? q) {
        List<Group> groups;
        if (string.IsNullOrWhiteSpace(q)) {
            groups = await _context.Groups
                .Where(x => x.Status)
                .OrderByDescending(x => x.GroupId)
                .ToListAsync();
        } else {
            groups = await _context.Groups
                .Where(x => (x.GroupName.Contains(q) || x.GroupCode.Contains(q)) && x.Status)
                .OrderByDescending(x => x.GroupId)
                .ToListAsync();
        }

        return _mapper.Map<List<GroupDto>>(groups);
    }

    [HttpPost("create")]
    [Authorize(Roles = RolesConstants.DIRECTOR)]
    public async Task<ActionResult<GroupDto>> CreateGroup([FromForm] CreateGroupDto dto) {
        try {
            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var randomString = Utils.RandomString(3, Utils.UppercaseChars, Utils.Numbers);
            var newGroup = new Group() {
                GroupName = dto.Name,
                Address = dto.Address,
                Status = true,
                GroupCode = $"{Utils.LongToBase32(currentTime)}{randomString}"
            };
            await _context.Groups.AddAsync(newGroup);
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(CreateGroup), routeValues: new { id = newGroup.GroupId }, _mapper.Map<GroupDto>(newGroup)
            );
        } catch (Exception e) {
            return ValidationError.InternalServerError500(e.Message);
        }
    }

    [HttpPatch("{id:int:min(1)}")]
    [Authorize(Roles = RolesConstants.DIRECTOR)]
    public async Task<ActionResult<GroupDto>> UpdateGroup([FromRoute] int id, [FromForm] UpdateGroupDto dto) {
        try {
            var group = await _context.Groups.FindAsync(id);
            if (group == null) {
                return ValidationError.BadRequest400("Nhóm với không tồn tại");
            }

            var isChanged = false;
            if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != group.GroupName) {
                group.GroupName = dto.Name;
                isChanged = true;
            }

            if (!string.IsNullOrWhiteSpace(dto.Address) && dto.Address != group.Address) {
                group.Address = dto.Address;
                isChanged = true;
            }

            if (isChanged) {
                await _context.SaveChangesAsync();
            }

            return _mapper.Map<GroupDto>(group);
        } catch (Exception e) {
            return ValidationError.InternalServerError500(e.Message);
        }
    }

    [HttpDelete("{id:int:min(1)}")]
    [Authorize(Roles = RolesConstants.DIRECTOR)]
    public async Task<IActionResult> DeleteGroup([FromRoute] int id) {
        var group = await _context.Groups.Include(x => x.Employees).FirstOrDefaultAsync(x => x.GroupId == id);
        if (group == null) {
            return ValidationError.BadRequest400("Nhóm không tồn tại");
        }

        if (group.Employees.Count > 0) {
            // return ValidationError.BadRequest400("Không thể xoá nhóm có nhân viên");
            group.Status = false;
        } else {
            _context.Groups.Remove(group);
        }

        await _context.SaveChangesAsync();
        return Ok();
    }
}