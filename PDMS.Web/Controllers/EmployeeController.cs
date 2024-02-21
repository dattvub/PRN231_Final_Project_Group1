using Microsoft.AspNetCore.Mvc;

namespace PDMS.Web.Controllers;

[Route("[controller]")]
public class EmployeeController : Controller {
    public IActionResult Index() {
        return View();
    }

    [Route("Create")]
    public IActionResult Create() {
        return View();
    }
}