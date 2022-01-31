﻿using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoverCore.Boilerplate.Domain.Entities.Settings;
using RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts;
using RoverCore.Boilerplate.Web.Areas.Admin.Models.ConfigurationViewModels;
using RoverCore.Boilerplate.Web.Controllers;
using RoverCore.BreadCrumbs.Services;

namespace RoverCore.Boilerplate.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ConfigurationController : BaseController<ConfigurationController>
{
	private readonly ApplicationDbContext _context;
	private readonly Infrastructure.Services.SettingsService _settingsService;

	public ConfigurationController(ApplicationDbContext context,
		Infrastructure.Services.SettingsService settingsService)
	{
		_context = context;
		_settingsService = settingsService;
	}

	public IActionResult Index()
	{
		_breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
			.ThenAction("Configuration", "Index", "Configuration", new { Area = "Admin" })
			.Then("Manage");

		var settings = _settingsService.GetSettings();

		return View(settings);
	}

	// POST: Configuration/Edit
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(
		[Bind("SiteName,Company,ApplyMigrationsOnStartup,SeedDataOnStartup")] ApplicationSettings newSettings)
	{
		var _settings = _settingsService.GetSettings();

		if (ModelState.IsValid)
		{
			_settings.SiteName = newSettings.SiteName;
			_settings.Company = newSettings.Company;

			_settings.ApplyMigrationsOnStartup = newSettings.ApplyMigrationsOnStartup;

			_settings.SeedDataOnStartup = newSettings.SeedDataOnStartup;

			await _settingsService.SaveSettings();

			return RedirectToAction(nameof(Index));
		}

		return View("Index", _settings);
	}

}