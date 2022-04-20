using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serviced;

namespace RoverCore.Boilerplate.Infrastructure;

public static class Startup
{
	// 
	/// <summary>
	/// Auto-register services implementing IScoped, ITransient, ISingleton (thanks to Georgi Stoyanov)
	/// Also add automapper and auto-register mapping profiles
	/// </summary>
	/// <param name="services"></param>
	/// <param name="assemblies"></param>
	public static void ConfigureServicesDiscovery(IServiceCollection services, params Assembly[] assemblies)
	{
		services.AddServiced(assemblies);
        services.AddAutoMapper(assemblies);
    }

	public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
	{
		// Add settings and configuration services
		Common.Settings.Startup.ConfigureServices(services, configuration);

		// Add Caching
		Common.Cache.Startup.ConfigureServices(services, configuration);

		// Add Templates
		Common.Templates.Startup.ConfigureServices(services, configuration);

		// Add Email services
		Common.Email.Startup.ConfigureServices(services, configuration);

		// Add Hangfire job services
		Common.Hangfire.Startup.ConfigureServices(services, configuration);

		// Add Persistence services (entity framework)
		Persistence.Startup.ConfigureServices(services, configuration);

        // Add Identity
        Identity.Startup.ConfigureServices(services, configuration);

		// Add Authentication
		Authentication.Startup.ConfigureServices(services, configuration);

		// Add Authorization
		Authorization.Startup.ConfigureServices(services, configuration);

	}

	/// <summary>
	/// Configure middleware - Be very careful - The order of middleware in the pipeline does matter.
	/// </summary>
	/// <param name="app"></param>
	/// <param name="configuration"></param>
	public static void Configure(IApplicationBuilder app, IConfiguration configuration)
	{
		// Configure Authentication
		Authentication.Startup.Configure(app, configuration);

		// Configure Authorization
		Authorization.Startup.Configure(app, configuration);

		// Configure Hangfire
		Common.Hangfire.Startup.Configure(app, configuration);

		// Configure Settings
		Common.Settings.Startup.Configure(app, configuration);

		// Configure Templates
		Common.Templates.Startup.Configure(app, configuration);

		// Configure Persistence services (entity framework)
		Persistence.Startup.Configure(app, configuration);
	}
}

