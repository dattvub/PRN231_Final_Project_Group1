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
using PDMS.Shared.DTO.CustomerType;

namespace PDMS.Controllers;

[Route("[controller]")]
[ApiController]
public class CustomerTypeController : ControllerBase
{
    private readonly IPdmsDbContext _context;
    private readonly IMapper _mapper;

    public CustomerTypeController(IPdmsDbContext context, IMapper mapper)
    {
        this._context = context;
        _mapper = mapper;
    }

    [EnableCors("allowAll")]
    [HttpGet("list")]
    public async Task<ActionResult<PaginationDto<CustomerTypeDto>>> GetcustomerTypes([FromQuery] GetCustomerTypesDto getCustomerTypesDto)
    {
        var query = _context.CustomerTypes.Where(x => x.Status);

        if (getCustomerTypesDto.Query != null)
        {
            if (getCustomerTypesDto.QueryByName)
            {
                query = query.Where(x => x.CustomerTypeName.Contains(getCustomerTypesDto.Query));
            }
            else
            {
                query = query.Where(x => x.CustomerTypeCode.Contains(getCustomerTypesDto.Query));
            }
        }

        var total = await query.CountAsync();

        var customerType = await query
            .OrderByDescending(x => x.CustomerTypeId)
            .Skip((getCustomerTypesDto.Page - 1) * getCustomerTypesDto.Quantity)
            .Take(getCustomerTypesDto.Quantity)
            .Select(x => _mapper.Map<CustomerTypeDto>(x))
            .ToListAsync();

        return new PaginationDto<CustomerTypeDto>(customerType, total)
        {
            Page = getCustomerTypesDto.Page,
            ItemsPerPage = getCustomerTypesDto.Quantity,
            Query = getCustomerTypesDto.Query
        };
    }
    
    [EnableCors("allowAll")]
    [HttpPost("create")]
    public async Task<ActionResult<CustomerTypeDto>> CreateCustomerType([FromBody] CreateCustomerTypeDto createCustomerTypeDto)
    {
        var customerType = _mapper.Map<CustomerType>(createCustomerTypeDto);
        await _context.CustomerTypes.AddAsync(customerType);

        try
        {
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(CreateCustomerType),
                new { id = customerType.CustomerTypeId },
                _mapper.Map<CustomerTypeDto>(customerType)
            );
        }
        catch (DbUpdateException)
        {
            if (!await CodeExisted(customerType.CustomerTypeCode)) throw;

            return ValidationError.BadRequest400("Dupplicate customerType code");
        }
        catch (Exception e)
        {
            return ValidationError.InternalServerError500(e.Message);
        }
    }

    [EnableCors("allowAll")]
    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<CustomerTypeDto>> UpdateCustomerType(
        [FromRoute] int id,
        [FromBody] CreateCustomerTypeDto createCustomerTypeDto
    )
    {
        var customerType = await _context.CustomerTypes.FirstOrDefaultAsync(x => x.CustomerTypeId == id && x.Status);
        if (customerType == null)
        {
            return ValidationError.BadRequest400($"CustomerType with id {id} is not exist");
        }

        var newInfoCustomerType = _mapper.Map<CustomerType>(createCustomerTypeDto);
        customerType.CustomerTypeCode = newInfoCustomerType.CustomerTypeCode;
        customerType.CustomerTypeName = newInfoCustomerType.CustomerTypeName;
        try
        {
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return ValidationError.BadRequest400("Error occur while update customerType");
            }
        }
        catch (DbUpdateException)
        {
            if (!await CodeExisted(customerType.CustomerTypeCode)) throw;

            return ValidationError.BadRequest400("Dupplicate customerType code");
        }
        catch (Exception e)
        {
            return ValidationError.InternalServerError500(e.Message);
        }

        return Ok(_mapper.Map<CustomerTypeDto>(customerType));
    }

    [EnableCors("allowAll")]
    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult<CustomerTypeDto>> DeleteCustomerType([FromRoute] int id)
    {
        var customerType = await _context.CustomerTypes.FirstOrDefaultAsync(x => x.CustomerTypeId == id && x.Status);
        if (customerType == null)
        {
            return ValidationError.BadRequest400($"CustomerType with id {id} is not exist");
        }

        customerType.Status = false;
        var result = await _context.SaveChangesAsync();
        if (result == 0)
        {
            return ValidationError.InternalServerError500("Error occur while delete customerType");
        }

        return Ok(_mapper.Map<CustomerTypeDto>(customerType));
    }

    private async Task<bool> CodeExisted(string code)
    {
        return await _context.CustomerTypes.AnyAsync(x => x.CustomerTypeCode == code);
    }
}