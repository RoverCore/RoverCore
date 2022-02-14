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
    /// <summary>
    ///     Adds a singleton service of the typed ApplicationSettings stored in appsettings.json
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        // MOVED TO STARTUP
        var settings = configuration.GetSection("Settings").Get<ApplicationSettings>();
        services.AddSingleton(settings);

        return services;
    }

    /// <summary>
    ///     Adds all Entity Framework database contexts to the service collection
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
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


        return services;
    }

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

    public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
    {
	    var settings = configuration.GetSection("Settings").Get<ApplicationSettings>();

	    var provider = new VirtualFileProvider();

	    services.AddSingleton<VirtualFileProvider>(provider);
	    services.AddFluentEmail(settings.Email.DefaultSenderAddress, settings.Email.DefaultSenderName)
		    .AddLiquidRenderer(config =>
		    {
			    config.FileProvider = provider;
		    })
		    .AddMailKitSender(new SmtpClientOptions 
		    {
                Server = settings.Email.Server,
                Port = settings.Email.Port,
                User = settings.Email.User,
                Password = settings.Email.Password,
                UseSsl = settings.Email.UseSsl,
                RequiresAuthentication = settings.Email.RequiresAuthentication,
                PreferredEncoding = settings.Email.PreferredEncoding,
                UsePickupDirectory = settings.Email.UsePickupDirectory,
                MailPickupDirectory = settings.Email.MailPickupDirectory
		    });

        return services;
    }

    /// <summary>
    ///     Add any caching services that are needed by the system
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCaching(this IServiceCollection services)
    {
	    // MOVED TO STARTUP
        // caching
        services.AddMemoryCache();

        return services;
    }

    /// <summary>
    ///     Add authentication services required for authentication
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddAuthenticationScheme(this IServiceCollection services,
        IConfiguration configuration)
    {
	    // MOVED TO STARTUP

        // configure strongly typed settings objects
        var appSettingsSection = configuration.GetSection("JWTSettings");
        services.Configure<JWTSettings>(appSettingsSection);

        // configure jwt authentication
        var appSettings = appSettingsSection.Get<JWTSettings>();
        var key = Encoding.ASCII.GetBytes(appSettings.TokenSecret);
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie()
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        return services;
    }
}