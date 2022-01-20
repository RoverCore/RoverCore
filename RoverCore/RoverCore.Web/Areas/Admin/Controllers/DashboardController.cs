using System.Threading.Tasks;
using Rover.Web.Models.HomeViewModels;
using RoverCore.Domain.Entities.Identity;
using RoverCore.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoverCore.Infrastructure.Services;

namespace RoverCore.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[ApiExplorerSettings(IgnoreApi = true)]
public class DashboardController : BaseController
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        _breadcrumbs.StartAtAction("Dashboard", "Index", "Dashboard", new { Area = "Admin"})
            .Then("Home");

        var viewModel = new HomeViewModel
        {
            User = await _userManager.GetUserAsync(User)
        };

        _toast.Success($"Welcome back {viewModel.User.FirstName}!");

        return View(viewModel);
    }
}