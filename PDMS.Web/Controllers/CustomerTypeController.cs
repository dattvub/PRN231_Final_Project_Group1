using Microsoft.AspNetCore.Mvc;

namespace PDMS.Web.Controllers;

public class CustomerTypeController : Controller
{
    [Route("Category/[controller]")]
    public IActionResult Index()
    {
        return View();
    }
}