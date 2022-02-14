using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoverCore.Boilerplate.Domain.Entities.Settings;
using RoverCore.Boilerplate.Infrastructure.Common.Settings.Services;

namespace RoverCore.Boilerplate.Infrastructure.Common.Settings;

public static class Startup
{
	/// <summary>
	///     Adds a singleton service of the typed ApplicationSettings stored in appsettings.json
	/// </summary>
	public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
	{
		// Add ApplicationsSettings service
		var settings = configuration.GetSection("Settings").Get<ApplicationSettings>();
		services.AddSingleton(settings);

		// Adds IOptions capabilities
		services.AddOptions();
	}

	public static void Configure(IApplicationBuilder app, IConfiguration configuration)
	{
		using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
		{
			var settingsService = serviceScope.ServiceProvider.GetRequiredService<SettingsService>();

			// Load persisted settings
			settingsService.LoadPersistedSettings().GetAwaiter().GetResult();
		}
	}
}

