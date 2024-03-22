using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using PDMS.Domain.Abstractions;
using PDMS.Domain.Entities;
using PDMS.Models;
using PDMS.Shared.DTO;
using PDMS.Shared.DTO.Brand;
using PDMS.Shared.DTO.CustomerGroup;

namespace PDMS.Controllers;

[Route("[controller]")]
[ApiController]
public class CustomerGroupController : ControllerBase
{
    private readonly IPdmsDbContext _context;
    private readonly IMapper _mapper;

    public CustomerGroupController(IPdmsDbContext context, IMapper mapper)
    {
        this._context = context;
        _mapper = mapper;
    }

    [EnableCors("allowAll")]
    [HttpGet("list")]
    public async Task<ActionResult<PaginationDto<CustomerGroupDto>>> GetCustomerGroups([FromQuery] GetCustomerGroupsDto getCustomerGroupsDto)
    {
        var query = _context.CustomerGroups.Where(x => x.Status);

        if (getCustomerGroupsDto.Query!= null)
        {
            if (getCustomerGroupsDto.QueryByName)
            {
                query = query.Where(x => x.CustomerGroupName.Contains(getCustomerGroupsDto.Query));
            }
            else
            {
                query = query.Where(x => x.CustomerGroupCode.Contains(getCustomerGroupsDto.Query));
            }
        }

        var total = await query.CountAsync();

        var customerGroup = await query
            .OrderByDescending(x => x.CustomerGroupId)
            .Skip((getCustomerGroupsDto.Page - 1) * getCustomerGroupsDto.Quantity)
            .Take(getCustomerGroupsDto.Quantity)
            .Select(x => _mapper.Map<CustomerGroupDto>(x))
            .ToListAsync();

        return new PaginationDto<CustomerGroupDto>(customerGroup, total)
        {
            Page = getCustomerGroupsDto.Page,
            ItemsPerPage = getCustomerGroupsDto.Quantity,
            Query = getCustomerGroupsDto.Query
        };
    }

    [EnableCors("allowAll")]
    [HttpPost("create")]
    public async Task<ActionResult<CustomerGroupDto>> CreateCustomerGroup([FromBody] CreateCustomerGroupsDto createCustomerGroupDto)
    {
        var customerGroup = _mapper.Map<CustomerGroup>(createCustomerGroupDto);
        await _context.CustomerGroups.AddAsync(customerGroup);

        try
        {
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(CreateCustomerGroup),
                new { id = customerGroup.CustomerGroupId },
                _mapper.Map<CustomerGroupDto>(customerGroup)
            );
        }
        catch (DbUpdateException)
        {
            if (!await CodeExisted(customerGroup.CustomerGroupCode)) throw;

            return ValidationError.BadRequest400("Dupplicate customerGroup code");
        }
        catch (Exception e)
        {
            return ValidationError.InternalServerError500(e.Message);
        }
    }

    [EnableCors("allowAll")]
    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<CustomerGroupDto>> UpdateCustomerGroup(
        [FromRoute] int id,
        [FromBody] CreateCustomerGroupsDto createCustomerGroupDto
    )
    {
        var customerGroup = await _context.CustomerGroups.FirstOrDefaultAsync(x => x.CustomerGroupId == id && x.Status);
        if (customerGroup == null)
        {
            return ValidationError.BadRequest400($"CustomerGroup with id {id} is not exist");
        }

        var newInfoCustomerGroup = _mapper.Map<CustomerGroup>(createCustomerGroupDto);
        customerGroup.CustomerGroupCode = newInfoCustomerGroup.CustomerGroupCode;
        customerGroup.CustomerGroupName = newInfoCustomerGroup.CustomerGroupName;
        try
        {
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return ValidationError.BadRequest400("Error occur while update customerGroup");
            }
        }
        catch (DbUpdateException)
        {
            if (!await CodeExisted(customerGroup.CustomerGroupCode)) throw;

            return ValidationError.BadRequest400("Dupplicate customerGroup code");
        }
        catch (Exception e)
        {
            return ValidationError.InternalServerError500(e.Message);
        }

        return Ok(_mapper.Map<CustomerGroupDto>(customerGroup));
    }

    [EnableCors("allowAll")]
    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult<CustomerGroupDto>> DeleteCustomerGroup([FromRoute] int id)
    {
        var customerGroup = await _context.CustomerGroups.FirstOrDefaultAsync(x => x.CustomerGroupId == id && x.Status);
        if (customerGroup == null)
        {
            return ValidationError.BadRequest400($"CustomerGroup with id {id} is not exist");
        }

        customerGroup.Status = false;
        var result = await _context.SaveChangesAsync();
        if (result == 0)
        {
            return ValidationError.InternalServerError500("Error occur while delete customerGroup");
        }

        return Ok(_mapper.Map<CustomerGroupDto>(customerGroup));
    }

    private async Task<bool> CodeExisted(string code)
    {
        return await _context.CustomerGroups.AnyAsync(x => x.CustomerGroupCode == code);
    }
}