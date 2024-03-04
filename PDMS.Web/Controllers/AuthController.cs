using Microsoft.AspNetCore.Mvc;

namespace PDMS.Web.Controllers;

[Route("[controller]")]
public class AuthController : Controller {
    [Route("Login")]
    public IActionResult Login() {
        return View();
    }
}