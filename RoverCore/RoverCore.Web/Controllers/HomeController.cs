using Microsoft.AspNetCore.Mvc;
using RoverCore.Web.Controllers;

namespace Rover.Web.Controllers;

public class HomeController : BaseController
{
    public IActionResult Index()
    {
        return View();
    }
}