using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RoverCore.Infrastructure.Models.AuthenticationModels;
using RoverCore.Infrastructure.Persistence.DbContexts;
using RoverCore.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using RoverCore.Domain.Entities;
using RoverCore.Domain.Entities.Identity;
using RoverCore.Infrastructure.Services.Identity;
using RoverCore.Infrastructure.Services.Seeder;

namespace RoverCore.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSeeders(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var modules = assembly.Modules.ToList();

            var seeders = from t in assembly.GetTypes()
                where t.GetInterfaces().Contains(typeof(ISeeder)) 
                select t;

            foreach (var seeder in seeders)
            {
                services.AddScoped(seeder);
            }

            return services;
        }

        public static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection("Settings").Get<ApplicationSettings>();
            services.AddSingleton(settings);

            return services;
        }

        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("AppContext"), 
                    x => x.MigrationsAssembly("RoverCore.Infrastructure"));
            });


            return services;
        }

        public static IServiceCollection AddApplicationIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddClaimsPrincipalFactory<ApplicationClaimsPrincipalFactory>()
                .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection AddCaching(this IServiceCollection services)
        {
            // caching
            services.AddMemoryCache();
            services.AddTransient<CacheService>();

            return services;
        }
        public static IServiceCollection AddAuthenticationScheme(this IServiceCollection services, IConfiguration configuration)
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
}
