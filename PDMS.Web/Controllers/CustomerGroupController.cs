using Microsoft.AspNetCore.Mvc;

namespace PDMS.Web.Controllers;

public class CustomerGroupController : Controller {
    public IActionResult Index()
    {
        return View();
    }
}