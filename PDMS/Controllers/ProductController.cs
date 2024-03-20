using AutoMapper;
using ImageMagick;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDMS.Domain.Abstractions;
using PDMS.Domain.Entities;
using PDMS.Models;
using PDMS.Shared.Constants;
using PDMS.Shared.DTO;
using PDMS.Shared.DTO.Brand;
using PDMS.Shared.DTO.Product;
using System.Text.Json;

namespace PDMS.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IPdmsDbContext _context;
        private static readonly string StaticRootDir;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        static ProductController()
        {
            StaticRootDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "PDMS");
        }
        public ProductController(IPdmsDbContext context, IMapper mapper, UserManager<User> userManager)
        {
            this._context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        [EnableCors("allowAll")]
        [HttpGet("list")]
        public async Task<ActionResult<PaginationDto<ProductDto>>> GetProducts([FromQuery] GetProductsDto getProductsDto)
        {
            var firstQuery = _context.Products.AsQueryable();

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
            if(getProductsDto.OnlyIn?.Length > 0)
            {
                var onlyInProducts = await firstQuery
                    .Where(o => getProductsDto.OnlyIn.Contains(o.ProductId))
                    .Select(x => _mapper.Map<ProductDto>(x))
                    .ToListAsync();
                return new PaginationDto<ProductDto>(onlyInProducts);
            }
            else
            {
                var total = await firstQuery.CountAsync();
                var products = await firstQuery
                .Where(x => x.Status)
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
        }


        [EnableCors("allowAll")]
        [HttpPost("create")]
        [Authorize(Roles = RolesConstants.DIRECTOR)]
        public async Task<ActionResult<ProductDto>> CreateProduct(
            [FromForm] CreateProductDto createProductDto,
            List<IFormFile> images
            )
        {
            string? fullPath = null;
            string? relImagePath = null;
            List<string> itemsPath = new List<string>();
            string? itemsFullPath = null;
            var userId = _userManager.GetUserId(User);
            var emp = await _context.Employees.FirstOrDefaultAsync(c => c.UserId.Equals(userId));
            if (emp == null)
            {
                return ValidationError.BadRequest400("Không có Employee hợp lệ");
            }
            var product = _mapper.Map<Product>(createProductDto);
            if (images.Count > 0)
            {
                foreach (var image in images)
                {
                    var prgImg = new MagickImage(image.OpenReadStream());
                    prgImg.Format = MagickFormat.Jpg;
                    prgImg.ColorSpace = ColorSpace.sRGB;
                    var fileName = $"{Guid.NewGuid():D}.jpg";
                    fullPath = Path.Combine(StaticRootDir, "images", fileName);
                    await prgImg.WriteAsync(fullPath, MagickFormat.Jpg);
                    if (System.IO.File.Exists(fullPath))
                    {
                        relImagePath = Path
                            .Combine("files", "images", fileName)
                            .Replace(Path.DirectorySeparatorChar, '/');
                        itemsPath.Add(relImagePath);
                    }
                }
                itemsFullPath = JsonSerializer.Serialize(itemsPath);
            }
            product.Image = itemsFullPath;
            product.CreatedById = emp.EmpId;
            product.LastModifiedById = emp.EmpId;

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
