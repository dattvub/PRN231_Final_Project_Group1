using Microsoft.AspNetCore.Mvc;

namespace PDMS.Web.Controllers;

[Route("/Category/[controller]")]
public class ProductController : Controller {
    public IActionResult Index()
    {
        return View();
    }

    [Route("create")]
    public IActionResult CreateProduct()
    {
        return View();
    }

    [Route("{id}")]
    public IActionResult DetailProduct()
    {
        return View();
    }

    [Route("director/edit/{id}")]
    public IActionResult EditProduct()
    {
        return View();
    }

    [Route("director/view")]
    public IActionResult ViewProduct()
    {
        return View();
    }
}