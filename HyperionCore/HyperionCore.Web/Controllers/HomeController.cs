using Microsoft.AspNetCore.Mvc;

namespace Hyperion.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard", new { Area = "Admin"});
            }

            return View();
        }
    }
}
