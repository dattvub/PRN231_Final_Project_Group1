using Microsoft.AspNetCore.Mvc;

namespace PDMS.Web.Controllers; 
public class OrderTicketController : Controller {
    public IActionResult Index()
    {
        return View();
    }

  

    public IActionResult Create()
    {
        return View();
    }
    public IActionResult Detail()
    {
        return View();
    }
    public IActionResult Update()
    {
        return View();
    }
}