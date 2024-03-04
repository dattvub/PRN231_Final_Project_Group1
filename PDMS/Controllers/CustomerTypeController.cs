using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDMS.Domain.Abstractions;
using PDMS.Domain.Entities;
using PDMS.Models;
using PDMS.Shared.DTO;
using PDMS.Shared.DTO.Brand;
using PDMS.Shared.DTO.CustomerType;
using PDMS.Shared.DTO.OrderDetail;

namespace PDMS.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomerTypeController : Controller
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
        public async Task<ActionResult<PaginationDto<CustomerTypeDto>>> GetCustomerTypes([FromQuery] GetCustomerTypesDto getCustomerTypesDto)
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

            var customerTypes = await query
                .OrderByDescending(x => x.CustomerTypeId)
                .Skip((getCustomerTypesDto.Page - 1) * getCustomerTypesDto.Quantity)
                .Take(getCustomerTypesDto.Quantity)
                .Select(x => _mapper.Map<CustomerTypeDto>(x))
                .ToListAsync();
            return new PaginationDto<CustomerTypeDto>(customerTypes, total)
            {
                Page = getCustomerTypesDto.Page,
                ItemsPerPage = getCustomerTypesDto.Quantity,
                Query = getCustomerTypesDto.Query
            };
        }

        [EnableCors("allowAll")]
        [HttpPost("create")]
        public async Task<ActionResult<CustomerTypeDto>> CreateCustomerType([FromBody] CreateCustomerTypeDto createCustomer)
        {
            var customerType = _mapper.Map<CustomerType>(createCustomer);
            await _context.CustomerTypes.AddAsync(customerType);
            try
            {
                await _context.SaveChangesAsync();
                return CreatedAtAction(
                    nameof(CreateCustomerType),
                    new { id = customerType.CustomerTypeId },
                    _mapper.Map<CustomerTypeDto>(customerType));
            }catch (Exception ex)
            {
                return ValidationError.InternalServerError500(ex.Message);
            }
        }

        [EnableCors("allowAll")]
        [HttpPost("{id}")]
        public async Task<ActionResult<CustomerTypeDto>> UpdateCustomerType(
            [FromRoute] int id,
            [FromBody] CreateCustomerTypeDto customerTypeDto)
        {
            var customerT = await _context.CustomerTypes.FirstOrDefaultAsync(x => x.CustomerTypeId == id);
            if (customerT == null)
            {
                return ValidationError.BadRequest400($"Order detail with id {id} is not exist");
            }
            var newCustomerType = _mapper.Map<CustomerType>(customerTypeDto);
            customerT.CustomerTypeName = newCustomerType.CustomerTypeName;
            customerT.CustomerTypeCode = newCustomerType.CustomerTypeCode;
            try
            {
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return ValidationError.BadRequest400("Error occur while update order detail");
                }
            }
            catch (Exception e)
            {
                return ValidationError.InternalServerError500(e.Message);
            }
            return Ok(_mapper.Map<CustomerTypeDto>(customerT));
        }

        [EnableCors("allowAll")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<CustomerTypeDto>> DeleteCustomerType([FromRoute] int id)
        {
            var customerType = await _context.CustomerTypes.FirstOrDefaultAsync(x => x.CustomerTypeId == id );
            if (customerType == null)
            {
                return ValidationError.BadRequest400($"Customer type with id {id} is not exist");
            }

            customerType.Status = false;
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return ValidationError.InternalServerError500("Error occur while delete customer type");
            }

            return Ok(_mapper.Map<CustomerTypeDto>(customerType));
        }
    }
}
