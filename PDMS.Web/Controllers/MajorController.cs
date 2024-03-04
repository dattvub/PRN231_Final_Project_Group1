using Microsoft.AspNetCore.Mvc;

namespace PDMS.Web.Controllers;

[Route("Category/[controller]")]
public class MajorController : Controller {
    public IActionResult Index()
    {
        return View();
    }
}