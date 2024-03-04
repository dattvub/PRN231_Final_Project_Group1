using Microsoft.AspNetCore.Mvc;

namespace PDMS.Web.Controllers;

public class CustomerController : Controller {
    [Route("/[controller]")]
    public IActionResult Index()
    {
        return View();
    }
    [Route("/[controller]/Add")]
    public IActionResult Add()
    {
        return View();
    }
    [Route("/[controller]/{id}")]
    public IActionResult Delete()
    {
        return View();
    }

    [Route("/[controller]/{id}/Update")]
    public IActionResult Update()
    {
        return View();
    }
}