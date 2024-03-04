using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDMS.Domain.Abstractions;
using PDMS.Domain.Entities;
using PDMS.Models;
using PDMS.Shared.DTO.Brand;
using PDMS.Shared.DTO.OrderDetail;

namespace PDMS.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrderDetailController : Controller
    {
        private readonly IPdmsDbContext _context;
        private readonly IMapper _mapper;

        public OrderDetailController(IPdmsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [EnableCors("allowAll")]
        [HttpPost("create")]
        public async Task<ActionResult<OrderDetailDto>> CreateOrderDetail([FromBody] CreateOrderDetailDto createOrderDetail)
        {
            var orderDetail = _mapper.Map<OrderDetail>(createOrderDetail);
            await _context.OrderDetails.AddAsync(orderDetail);
            try
            {
                await _context.SaveChangesAsync();
                return CreatedAtAction(
                    nameof(CreateOrderDetail), new { id = orderDetail.OrderDetailId }, _mapper.Map<OrderDetailDto>(orderDetail));
            }
            catch (Exception e)
            {
                return ValidationError.InternalServerError500(e.Message);
            }
        }

        [EnableCors("allowAll")]
        [HttpPut("{id}")]
        public async Task<ActionResult<OrderDetailDto>> UpdateOrderDetail(
            [FromRoute] int id,
            [FromBody] CreateOrderDetailDto orderDetailDto)
        {
            var orderDetail = await _context.OrderDetails.FirstOrDefaultAsync(x => x.OrderDetailId == id);
            if (orderDetail == null)
            {
                return ValidationError.BadRequest400($"Order detail with id {id} is not exist");
            }
            var newOrderDetail = _mapper.Map<OrderDetail>(orderDetailDto);
            orderDetail.Quantity = newOrderDetail.Quantity;
            orderDetail.Price = newOrderDetail.Price;
            orderDetail.Total = newOrderDetail.Total;
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
            return Ok(_mapper.Map<OrderDetailDto>(orderDetail));
        }
    }
}
