using Rover.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RoverCore.BreadCrumbs;
using RoverCore.BreadCrumbs.Services;
using RoverCore.Domain.Entities;
using RoverCore.Domain.Entities.Identity;
using RoverCore.Infrastructure.Models.AuthenticationModels;
using RoverCore.Infrastructure.Persistence.DbContexts;
using RoverCore.Infrastructure.Services;
using RoverCore.Infrastructure.Services.Identity;
using RoverCore.Navigation.Services;
using RoverCore.ToastNotification;
using RoverCore.Infrastructure.Extensions;

namespace Rover.Web;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        // Adds cross-origin sharing services
        services.AddCors();

        services.AddPersistence(Configuration) // Add database access and identity
                .AddApplicationIdentity()  // Add custom identity user for application
                .AddHttpContextAccessor()  // Add default HttpContextAccessor service
                .AddOptions();  // Adds IOptions capabilities

        // Add routing with lowercase url configuration
        services.AddRouting(options => options.LowercaseUrls = true);

#if DEBUG
        // For development only - Display exceptions on page if there is an error
        services.AddDatabaseDeveloperPageExceptionFilter();
#endif

        // Add Mvc services
        services.AddMvc()
                .AddRazorRuntimeCompilation();

        // Add Swagger documentation
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "RoverCore.Web API", Version = "v1" });

            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        // RoverCore infrastructure services
        services.AddAuthenticationScheme(Configuration)
                .AddSettings(Configuration)
                .AddCaching(); // Adds CacheService

        // Add JWT user service
        services.AddScoped<IUserService, UserService>();

        // Configure email service
        services.AddTransient<IEmailSender, EmailSender>();

        // Add application layer services
        services.AddScoped<IBreadCrumbService, BreadCrumbService>();
        services.AddScoped<NavigationService>();
        services.AddNotyf(config => { config.DurationInSeconds = 10; config.IsDismissable = true; config.Position = NotyfPosition.BottomRight; });
        services.AddTransient<RoverCore.Infrastructure.Services.Configuration>();

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        UpdateDatabase(app);

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
        }

        app.UseRouting();
        app.UseStaticFiles();


        // global cors policy
        app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            );

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

        });

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        });

        using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            new ApplicationSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();

        }
    }

    // Applies any new migrations automatically
    private static void UpdateDatabase(IApplicationBuilder app)
    {
        try
        {
            using (var serviceScope = app.ApplicationServices
                       .GetRequiredService<IServiceScopeFactory>()
                       .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
                {
                    context?.Database.Migrate();
                }
            }
        }
        catch (Exception e)
        {
            // Log error
        }
    }
}