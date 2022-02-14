using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RoverCore.Boilerplate.Infrastructure.Common.Cache;

/// <summary>
///     Add any caching services that are needed by the system
/// </summary>
public static class Startup
{
	public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
	{
		services.AddMemoryCache();
	}

	public static void Configure(IApplicationBuilder app, IConfiguration configuration)
	{
	}
}

