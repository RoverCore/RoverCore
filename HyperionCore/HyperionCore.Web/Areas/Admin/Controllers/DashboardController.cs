using System.Threading.Tasks;
using Hyperion.Web.Models.HomeViewModels;
using HyperionCore.Domain.Entities.Identity;
using HyperionCore.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HyperionCore.Web.Areas.Admin.Controllers;

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
        _breadCrumbService.Add("Home");

        var viewModel = new HomeViewModel
        {
            User = await _userManager.GetUserAsync(User)
        };

        _notify.Success($"Welcome back {viewModel.User.FirstName}!");

        return View(viewModel);
    }
}