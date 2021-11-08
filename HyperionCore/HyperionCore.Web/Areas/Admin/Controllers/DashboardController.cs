using System.Threading.Tasks;
using Hyperion.Web.Models.HomeViewModels;
using HyperionCore.Web.Areas.Identity.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HyperionCore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel
            {
                User = await _userManager.GetUserAsync(User)
            };

            return View(viewModel);
        }
    }
}
