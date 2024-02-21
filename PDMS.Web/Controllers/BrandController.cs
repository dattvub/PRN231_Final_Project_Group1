using Microsoft.AspNetCore.Mvc;

namespace PDMS.Web.Controllers;

[Route("Category/[controller]")]
public class BrandController : Controller {
    public IActionResult Index() {
        return View();
    }
}