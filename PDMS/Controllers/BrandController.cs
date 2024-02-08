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
        var query = _context.Brands.AsQueryable();

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

            var error = new ValidationError() {
                StatusCode = StatusCodes.Status400BadRequest,
                ReasonPhrase = ReasonPhrases.GetReasonPhrase(StatusCodes.Status400BadRequest),
                Errors = new List<string>() {
                    "Dupplicate brand code"
                },
                Timestamp = DateTime.Now
            };

            return ValidationError.BadRequest400("Dupplicate brand code");
        } catch (Exception e) {
            return ValidationError.InternalServerError500(e.Message);
        }
    }

    private async Task<bool> CodeExisted(string code) {
        return await _context.Brands.AnyAsync(x => x.BrandCode == code);
    }
}