using Hyperion.Web.Data;
using Hyperion.Web.Models.ConfigurationViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Hyperion.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ConfigurationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Services.Configuration Configuration;

        public ConfigurationController(ApplicationDbContext context, Services.Configuration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            var viewModel = new ConfigurationViewModel
            {
                RSSFeedUrl = Configuration.Get("RSSFeedUrl"),
                PrivacyPolicyUrl = Configuration.Get("PrivacyPolicyUrl"),
                AzureHubListenConnectionString = Configuration.Get("AzureHubListenConnectionString"),
                AzureHubFullConnectionString = Configuration.Get("AzureHubFullConnectionString"),
                AzureHubName = Configuration.Get("AzureHubName")
            };

            return View(viewModel);
        }

        // POST: Configuration/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ConfigurationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Configuration.Set("RSSFeedUrl", viewModel.RSSFeedUrl);
                Configuration.Set("PrivacyPolicyUrl", viewModel.PrivacyPolicyUrl);
                Configuration.Set("AzureHubListenConnectionString", viewModel.AzureHubListenConnectionString);
                Configuration.Set("AzureHubFullConnectionString", viewModel.AzureHubFullConnectionString);
                Configuration.Set("AzureHubName", viewModel.AzureHubName);

                await Configuration.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View("Index", viewModel);
        }
    }
}