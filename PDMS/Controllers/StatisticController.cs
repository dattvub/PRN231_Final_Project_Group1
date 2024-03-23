using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PDMS.Domain.Abstractions;
using PDMS.Shared.DTO.Statistic;
using PDMS.Shared.Enums;

namespace PDMS.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StatisticController : Controller
    {
        private readonly IPdmsDbContext _context;
        public StatisticController(IPdmsDbContext context)
        {
            _context = context;
        }
       [HttpGet]
        public IActionResult Index()
        {
            var customerCount = _context.Customers.ToList().Count;
            var employeeCount = _context.Employees.ToList().Count;
            var orderPendingCount = _context.OrderTickets.Where(x => x.Status == OrderTicketStatus.Pending).ToList().Count;
            var orderCompleteCount = _context.OrderTickets.Where(x => x.Status == OrderTicketStatus.Received).ToList().Count;
            var productCount = _context.Products.Where(x => x.Quantity > 0).ToList().Count;

            var summary = new StatisticDto()
            {
                CustomerCount = customerCount,
                EmployeeCount = employeeCount,
                OrderPendingCount = orderPendingCount,
                OrderCompleteCount = orderCompleteCount,
                ProductCount = productCount
            };

            return Ok(summary);
        }
    }
}
