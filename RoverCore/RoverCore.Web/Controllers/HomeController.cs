using Microsoft.AspNetCore.Mvc;

namespace Rover.Web.Controllers;

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