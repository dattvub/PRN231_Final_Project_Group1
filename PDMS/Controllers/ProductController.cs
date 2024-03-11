using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDMS.Domain.Abstractions;
using PDMS.Domain.Entities;
using PDMS.Models;
using PDMS.Shared.DTO;
using PDMS.Shared.DTO.Brand;
using PDMS.Shared.DTO.Product;

namespace PDMS.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IPdmsDbContext _context;
        private readonly IMapper _mapper;

        public ProductController(IPdmsDbContext context, IMapper mapper)
        {
            this._context = context;
            _mapper = mapper;
        }

        [EnableCors("allowAll")]
        [HttpGet("list")]
        public async Task<ActionResult<PaginationDto<ProductDto>>> GetProducts([FromQuery] GetProductsDto getProductsDto)
        {
            var firstQuery = _context.Products.Where(x => x.Status);

            if(getProductsDto.Query != null) {
                if (getProductsDto.QueryByName)
                {
                    firstQuery = firstQuery.Where(x => x.ProductName.Contains(getProductsDto.Query));
                }
                else
                {
                    firstQuery = firstQuery.Where(x => x.ProductCode.Contains(getProductsDto.Query));
                }
            }
            var total = await firstQuery.CountAsync();
            var products = await firstQuery
            .OrderByDescending(x => x.ProductId)
            .Skip((getProductsDto.Page - 1) * getProductsDto.Quantity)
            .Take(getProductsDto.Quantity)
            .Select(x => _mapper.Map<ProductDto>(x))
            .ToListAsync();

            return new PaginationDto<ProductDto>(products, total)
            {
                Page = getProductsDto.Page,
                ItemsPerPage = getProductsDto.Quantity,
                Query = getProductsDto.Query
            };
        }

        [EnableCors("allowAll")]
        [HttpPost("create")]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            var product = _mapper.Map<Product>(createProductDto);
            await _context.Products.AddAsync(product);
            try
            {
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(CreateProduct),
                    new { id = product.ProductId },
                    _mapper.Map<ProductDto>(product)
                );
            }
            catch (Exception e)
            {
                return ValidationError.InternalServerError500(e.Message);
            }
        }

        [EnableCors("allowAll")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct([FromRoute] int id, [FromBody] UpdateProductDto updateProductDto)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == id && x.Status);

            if (product == null)
            {
                return ValidationError.BadRequest400($"Product with id {id} is not exist");
            }

            var newProduct = _mapper.Map<Product>(updateProductDto);
            //product.ProductCode = newProduct.ProductCode;
            product.ProductName = newProduct.ProductName;
            product.ImportPrice = newProduct.ImportPrice;
            product.Price = newProduct.Price;
            product.Quantity = newProduct.Quantity;
            product.BarCode = newProduct.BarCode;
            product.LastModifiedById = newProduct.LastModifiedById;
            product.LastModifiedTime = newProduct.LastModifiedTime;
            product.Image = newProduct.Image;
            product.BrandId = newProduct.BrandId;
            product.SuppilerId = newProduct.SuppilerId;
            product.MajorId = newProduct.MajorId;

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return ValidationError.BadRequest400("Error occur while update product");
                }
            }
            catch (Exception e)
            {
                return ValidationError.InternalServerError500(e.Message);
            }

            return Ok(_mapper.Map<ProductDto>(product));
        }

        [EnableCors("allowAll")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ProductDto>> DeleteProduct([FromRoute] int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == id && x.Status);
            if (product == null)
            {
                return ValidationError.BadRequest400($"Product with id {id} is not exist");
            }

            product.Status = false;
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return ValidationError.InternalServerError500("Error occur while delete product");
            }

            return Ok(_mapper.Map<ProductDto>(product));
        }

        [EnableCors("allowAll")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> DetailProduct([FromRoute] int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == id && x.Status);
            if (product == null)
            {
                return ValidationError.BadRequest400($"Product with id {id} is not exist");
            }
            return Ok(_mapper.Map<ProductDto>(product));
        }
    }
}
