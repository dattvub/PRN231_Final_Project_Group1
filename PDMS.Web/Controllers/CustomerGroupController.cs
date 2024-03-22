using Microsoft.AspNetCore.Mvc;

namespace PDMS.Web.Controllers;

public class CustomerGroupController : Controller {
    [Route("Category/[controller]")]
    public IActionResult Index()
    {
        return View();
    }
}