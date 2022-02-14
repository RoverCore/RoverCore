using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RoverCore.Boilerplate.Infrastructure.Authorization;

public static class Startup
{
	/// <summary>
	///     Add authorization services required for authentication
	/// </summary>
	public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
	{

	}

	public static void Configure(IApplicationBuilder app, IConfiguration configuration)
	{
		app.UseAuthorization();
	}
}