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
using PDMS.Shared.DTO.Supplier;

namespace PDMS.Controllers;

[Route("[controller]")]
[ApiController]
public class SupplierController : ControllerBase
{
    private readonly IPdmsDbContext _context;
    private readonly IMapper _mapper;

    public SupplierController(IPdmsDbContext context, IMapper mapper)
    {
        this._context = context;
        _mapper = mapper;
    }

    [EnableCors("allowAll")]
    [HttpGet("list")]
    public async Task<ActionResult<PaginationDto<SupplierDto>>> GetSuppliers([FromQuery] GetSuplliersDto getSuppliersDto)
    {
        var query = _context.Suppliers.Where(x => x.Status);

        if (getSuppliersDto.Query != null)
        {
            if (getSuppliersDto.QueryByName)
            {
                query = query.Where(x => x.SupplierName.Contains(getSuppliersDto.Query));
            }
            else
            {
                query = query.Where(x => x.SupplierCode.Contains(getSuppliersDto.Query));
            }
        }

        var total = await query.CountAsync();

        var supplier = await query
            .OrderByDescending(x => x.SupplierId)
            .Skip((getSuppliersDto.Page - 1) * getSuppliersDto.Quantity)
            .Take(getSuppliersDto.Quantity)
            .Select(x => _mapper.Map<SupplierDto>(x))
            .ToListAsync();

        return new PaginationDto<SupplierDto>(supplier, total)
        {
            Page = getSuppliersDto.Page,
            ItemsPerPage = getSuppliersDto.Quantity,
            Query = getSuppliersDto.Query
        };
    }

    [EnableCors("allowAll")]
    [HttpPost("create")]
    public async Task<ActionResult<SupplierDto>> CreateSupplier([FromBody] CreateSupplierDto createSupplierDto)
    {
        var supplier = _mapper.Map<Supplier>(createSupplierDto);
        await _context.Suppliers.AddAsync(supplier);

        try
        {
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(CreateSupplier),
                new { id = supplier.SupplierId },
                _mapper.Map<SupplierDto>(supplier)
            );
        }
        catch (DbUpdateException)
        {
            if (!await CodeExisted(supplier.SupplierCode)) throw;

            return ValidationError.BadRequest400("Dupplicate supplier code");
        }
        catch (Exception e)
        {
            return ValidationError.InternalServerError500(e.Message);
        }
    }

    [EnableCors("allowAll")]
    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<SupplierDto>> UpdateSupplier(
        [FromRoute] int id,
        [FromBody] CreateSupplierDto createSupplierDto
    )
    {
        var supplier = await _context.Suppliers.FirstOrDefaultAsync(x => x.SupplierId == id && x.Status);
        if (supplier == null)
        {
            return ValidationError.BadRequest400($"Supplier with id {id} is not exist");
        }

        var newInfoSupplier = _mapper.Map<Supplier>(createSupplierDto);
        supplier.SupplierCode = newInfoSupplier.SupplierCode;
        supplier.SupplierName = newInfoSupplier.SupplierName;
        try
        {
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return ValidationError.BadRequest400("Error occur while update suppier");
            }
        }
        catch (DbUpdateException)
        {
            if (!await CodeExisted(supplier.SupplierCode)) throw;

            return ValidationError.BadRequest400("Dupplicate supplier code");
        }
        catch (Exception e)
        {
            return ValidationError.InternalServerError500(e.Message);
        }

        return Ok(_mapper.Map<SupplierDto>(supplier));
    }

    [EnableCors("allowAll")]
    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult<SupplierDto>> DeleteSupplier([FromRoute] int id)
    {
        var supplier = await _context.Suppliers.FirstOrDefaultAsync(x => x.SupplierId == id && x.Status);
        if (supplier == null)
        {
            return ValidationError.BadRequest400($"Supplier with id {id} is not exist");
        }

        supplier.Status = false;
        var result = await _context.SaveChangesAsync();
        if (result == 0)
        {
            return ValidationError.InternalServerError500("Error occur while delete supplier");
        }

        return Ok(_mapper.Map<SupplierDto>(supplier));
    }

    private async Task<bool> CodeExisted(string code)
    {
        return await _context.Suppliers.AnyAsync(x => x.SupplierCode == code);
    }
}