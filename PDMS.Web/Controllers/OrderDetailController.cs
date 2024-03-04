using Microsoft.AspNetCore.Mvc;

namespace PDMS.Web.Controllers
{
    [Route("Ticket/OrderTicket/[controller]")]
    public class OrderDetailController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("Add")]
        [HttpGet]
        public IActionResult AddOrderDetail()
        {
            return View();
        }


    }
}
