using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RoverCore.Boilerplate.Infrastructure;

public static class Startup
{
	public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
	{
		// Add Persistence services
		// RoverCore.Boilerplate.Infrastructure.Persistence.Startup.ConfigureServices(services, configuration);
	}

	public static void Configure(IApplicationBuilder app, IConfiguration configuration)
	{
	}
}

