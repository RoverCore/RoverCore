using System.Threading.Tasks;
using RoverCore.Infrastructure.Services;
using RoverCore.Web.Areas.Admin.Models.ConfigurationViewModels;
using RoverCore.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoverCore.Infrastructure.Persistence.DbContexts;

namespace RoverCore.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ConfigurationController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly Configuration Configuration;

    public ConfigurationController(ApplicationDbContext context, Configuration configuration)
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