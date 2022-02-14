using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoverCore.Boilerplate.Domain.Entities.Settings;

namespace RoverCore.Boilerplate.Infrastructure.Common.Settings;

public static class Startup
{
	/// <summary>
	///     Adds a singleton service of the typed ApplicationSettings stored in appsettings.json
	/// </summary>
	public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
	{
		var settings = configuration.GetSection("Settings").Get<ApplicationSettings>();
		services.AddSingleton(settings);
	}

	public static void Configure(IApplicationBuilder app, IConfiguration configuration)
	{
	}
}

