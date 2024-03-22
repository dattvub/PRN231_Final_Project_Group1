using Microsoft.AspNetCore.Mvc;

namespace PDMS.Web.Controllers
{
    public class NotificationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
