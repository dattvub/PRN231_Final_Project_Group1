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

namespace PDMS.Controllers;

[Route("[controller]")]
[ApiController]
public class BrandController : ControllerBase {
    private readonly IPdmsDbContext _context;
    private readonly IMapper _mapper;

    public BrandController(IPdmsDbContext context, IMapper mapper) {
        this._context = context;
        _mapper = mapper;
    }

    [EnableCors("allowAll")]
    [HttpGet("list")]
    public async Task<ActionResult<PaginationDto<BrandDto>>> GetBrands([FromQuery] GetBrandsDto getBrandsDto) {
        var query = _context.Brands.Where(x => x.Status);

        if (getBrandsDto.Query != null) {
            if (getBrandsDto.QueryByName) {
                query = query.Where(x => x.BrandName.Contains(getBrandsDto.Query));
            } else {
                query = query.Where(x => x.BrandCode.Contains(getBrandsDto.Query));
            }
        }

        var total = await query.CountAsync();

        var brands = await query
            .OrderByDescending(x => x.BrandId)
            .Skip((getBrandsDto.Page - 1) * getBrandsDto.Quantity)
            .Take(getBrandsDto.Quantity)
            .Select(x => _mapper.Map<BrandDto>(x))
            .ToListAsync();

        return new PaginationDto<BrandDto>(brands, total) {
            Page = getBrandsDto.Page,
            ItemsPerPage = getBrandsDto.Quantity,
            Query = getBrandsDto.Query
        };
    }

    [EnableCors("allowAll")]
    [HttpPost("create")]
    public async Task<ActionResult<BrandDto>> CreateBrand([FromBody] CreateBrandDto createBrandDto) {
        var brand = _mapper.Map<Brand>(createBrandDto);
        await _context.Brands.AddAsync(brand);

        try {
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(CreateBrand),
                new { id = brand.BrandId },
                _mapper.Map<BrandDto>(brand)
            );
        } catch (DbUpdateException) {
            if (!await CodeExisted(brand.BrandCode)) throw;

            return ValidationError.BadRequest400("Dupplicate brand code");
        } catch (Exception e) {
            return ValidationError.InternalServerError500(e.Message);
        }
    }

    [EnableCors("allowAll")]
    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<BrandDto>> UpdateBrand(
        [FromRoute] int id,
        [FromBody] CreateBrandDto createBrandDto
    ) {
        var brand = await _context.Brands.FirstOrDefaultAsync(x => x.BrandId == id && x.Status);
        if (brand == null) {
            return ValidationError.BadRequest400($"Brand with id {id} is not exist");
        }

        var newInfoBrand = _mapper.Map<Brand>(createBrandDto);
        brand.BrandCode = newInfoBrand.BrandCode;
        brand.BrandName = newInfoBrand.BrandName;
        try {
            var result = await _context.SaveChangesAsync();
            if (result == 0) {
                return ValidationError.BadRequest400("Error occur while update brand");
            }
        } catch (DbUpdateException) {
            if (!await CodeExisted(brand.BrandCode)) throw;

            return ValidationError.BadRequest400("Dupplicate brand code");
        } catch (Exception e) {
            return ValidationError.InternalServerError500(e.Message);
        }

        return Ok(_mapper.Map<BrandDto>(brand));
    }

    [EnableCors("allowAll")]
    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult<BrandDto>> DeleteBrand([FromRoute] int id) {
        var brand = await _context.Brands.FirstOrDefaultAsync(x => x.BrandId == id && x.Status);
        if (brand == null) {
            return ValidationError.BadRequest400($"Brand with id {id} is not exist");
        }

        brand.Status = false;
        var result = await _context.SaveChangesAsync();
        if (result == 0) {
            return ValidationError.InternalServerError500("Error occur while delete brand");
        }

        return Ok(_mapper.Map<BrandDto>(brand));
    }

    private async Task<bool> CodeExisted(string code) {
        return await _context.Brands.AnyAsync(x => x.BrandCode == code);
    }
}