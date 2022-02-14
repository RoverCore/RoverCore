using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts;

namespace RoverCore.Boilerplate.Infrastructure.Persistence;

public static class Startup
{
	/// <summary>
	///     Adds all Entity Framework database contexts to the service collection
	/// </summary>
	public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<ApplicationDbContext>(options =>
		{
			options.UseSqlServer(configuration.GetConnectionString("AppContext"),
				x => x.MigrationsAssembly("RoverCore.Boilerplate.Infrastructure"));
		});

		services.AddDbContextFactory<ApplicationDbContext>(options =>
		{
			options.UseSqlServer(configuration.GetConnectionString("AppContext"),
				x => x.MigrationsAssembly("RoverCore.Boilerplate.Infrastructure"));
		});
    }

	public static void Configure(IApplicationBuilder app, IConfiguration configuration)
	{
	}
}

