using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RoverCore.Boilerplate.Domain.Entities;
using RoverCore.Boilerplate.Infrastructure.Models.AuthenticationModels;
using RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts;

namespace RoverCore.Boilerplate.Infrastructure.Extensions;

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


        return services;
    }

    /// <summary>
    ///     Add any caching services that are needed by the system
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCaching(this IServiceCollection services)
    {
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
        // configure strongly typed settings objects
        var appSettingsSection = configuration.GetSection("JWTSettings");
        services.Configure<JWTSettings>(appSettingsSection);

        // configure jwt authentication
        var appSettings = appSettingsSection.Get<JWTSettings>();
        var key = Encoding.ASCII.GetBytes(appSettings.TokenSecret);
        services.AddAuthentication()
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