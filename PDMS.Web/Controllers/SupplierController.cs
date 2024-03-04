using Microsoft.AspNetCore.Mvc;

namespace PDMS.Web.Controllers;

public class SupplierController : Controller {
    [Route("Category/[controller]")]
    public IActionResult Index()
    {
        return View();
    }
}