using System.Threading.Tasks;
using Rover.Web.Models.HomeViewModels;
using RoverCore.Domain.Entities.Identity;
using RoverCore.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoverCore.Web.Extensions;

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
        _breadCrumbService.StartAt("Dashboard", Url.Action("Index", "Dashboard", new { Area = "Admin"}) ?? "/admin/dashboard" )
            .Then("Home");

        var viewModel = new HomeViewModel
        {
            User = await _userManager.GetUserAsync(User)
        };

        _toast.Success($"Welcome back {viewModel.User.FirstName}!");

        return View(viewModel);
    }
}