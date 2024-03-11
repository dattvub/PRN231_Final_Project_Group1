using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDMS.Domain.Abstractions;
using PDMS.Domain.Entities;
using PDMS.Shared.DTO;
using PDMS.Shared.DTO.Major;
using PDMS.Shared.DTO.OrderDetail;
using System.Net.WebSockets;

namespace PDMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IPdmsDbContext _context;

        public OrderController(IPdmsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<PaginationDto<OrderTicket>> SearchAll(SearchOrderDto request)
        {
            var userId = 1;
            var role = "";
            var query = _context.OrderTickets.AsQueryable()
                .OrderByDescending(i => i.OrderId)
            .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize);
            var listOrder = new List<OrderTicket>();
            //Lấy order theo customerId
            if (role == "Customer")
                listOrder = await query.Where(i => i.CustomerId == userId).ToListAsync();
            //Lấy order theo listCustomerId
            else if (role == "Sale")
            {
                var listCustomerId = await _context.Customers.Where(i => i.Employee == userId).Select(i => i.CustomerId).ToListAsync();
                listOrder = await query.Where(i => listCustomerId.Contains(i.CustomerId)).ToListAsync();
            }
            //Quản lý thấy order của customer thuộc các employee trong nhóm
            else if (role == "Supervisor")
            {
                var groupId = await GetGroupIdByEmployeeId(userId);
                var listEmployeId = await _context.EmpGroups
                    .Where(i => i.GroupId == groupId).Select(i => i.EmpId).ToListAsync();
                var listCusId = await _context.Customers.Where(i => listEmployeId.Contains(i.CustomerId))
                    .Select(i => i.CustomerId)
                    .ToListAsync();
                listOrder = await query.Where(i => listCusId.Contains(i.CustomerId)).ToListAsync();
            }
            var total = await query.CountAsync();
            return new PaginationDto<OrderTicket>(listOrder, total)
            {
                Page = request.Page,
                ItemsPerPage = request.PageSize,
            };
        }

        private async Task<int> GetGroupIdByEmployeeId(int employeeId)
        {
            var gid = await _context.EmpGroups.Where(i => i.EmpId == employeeId)
                .Select(i => i.GroupId).FirstOrDefaultAsync();
            return gid;
        }
    }
}
