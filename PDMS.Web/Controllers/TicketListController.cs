using Microsoft.AspNetCore.Mvc;

namespace PDMS.Web.Controllers;

[Route("/Category/[controller]")]
public class TicketListController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
