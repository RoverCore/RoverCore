using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoverCore.BreadCrumbs.Services;
using RoverCore.Boilerplate.Domain.Entities.Identity;
using RoverCore.Boilerplate.Web.Areas.Dashboard.Models.HomeViewModels;
using RoverCore.Boilerplate.Web.Controllers;
using System.Threading.Tasks;

namespace RoverCore.Boilerplate.Web.Areas.Dashboard.Controllers;

[Area("Dashboard")]
[Authorize]
[ApiExplorerSettings(IgnoreApi = true)]
public class HomeController : BaseController
{
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
            .Then("Home");

        var viewModel = new HomeViewModel
        {
            User = await _userManager.GetUserAsync(User)
        };

        _toast.Success($"Welcome back {viewModel.User.FirstName}!");

        return View(viewModel);
    }
}