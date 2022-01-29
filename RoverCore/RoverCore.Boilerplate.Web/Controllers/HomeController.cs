using Microsoft.AspNetCore.Mvc;

namespace RoverCore.Boilerplate.Web.Controllers;

public class HomeController : BaseController
{
    public IActionResult Index()
    {
        return View();
    }

    // Template actions
    public IActionResult About() => View();
}