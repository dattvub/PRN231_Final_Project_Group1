using Microsoft.AspNetCore.Mvc;

namespace PDMS.Web.Controllers;

public class SupplierController : Controller {
    [Route("Category/[controller]")]
    public IActionResult Index()
    {
        return View();
    }
    [Route("Category/[controller]/Add")]
    public IActionResult Add()
    {
        return View();
    }
    [Route("Category/[controller]/{id}")]
    public IActionResult Delete()
    {
        return View();
    }

    [Route("Category/[controller]/{id}/Update")]
    public IActionResult Update()
    {
        return View();
    }
}