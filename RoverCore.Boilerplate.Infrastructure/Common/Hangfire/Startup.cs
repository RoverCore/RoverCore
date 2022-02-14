using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoverCore.Boilerplate.Infrastructure.Common.Hangfire.Filters;

namespace RoverCore.Boilerplate.Infrastructure.Common.Hangfire;

public static class Startup
{
	public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
	{
		services.AddHangfire(config => config
			.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
			.UseSimpleAssemblyNameTypeSerializer()
			.UseRecommendedSerializerSettings()
			.UseSqlServerStorage(configuration.GetConnectionString("AppContext"), new SqlServerStorageOptions
			{
				CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
				SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
				QueuePollInterval = TimeSpan.Zero,
				UseRecommendedIsolationLevel = true,
				DisableGlobalLocks = true
			}));

		services.AddHangfireServer();
	}

	public static void Configure(IApplicationBuilder app, IConfiguration configuration)
	{
		// Set up hangfire capabilities
		var options = new DashboardOptions
		{
			Authorization = new[] { new HangfireAuthorizationFilter() },
			AppPath = "#",
			DashboardTitle = "",
			DisplayStorageConnectionString = false
		};

		app.UseHangfireDashboard("/admin/job/hangfire", options);
	}
}

