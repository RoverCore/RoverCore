using System.Text;
using FluentEmail.MailKitSmtp;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RoverCore.Boilerplate.Domain.DTOs.Authentication;
using RoverCore.Boilerplate.Domain.Entities.Settings;
using RoverCore.Boilerplate.Infrastructure.Common.Templates.Models;
using RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts;

namespace RoverCore.Boilerplate.Infrastructure.Common.Extensions;

public static class ServiceCollectionExtensions
{


    public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration configuration)
    {
	    // MOVED TO STARTUP
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

	    return services;
    }




}