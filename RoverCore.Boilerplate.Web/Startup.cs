using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RoverCore.Boilerplate.Domain.Entities.Identity;
using RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts;
using RoverCore.BreadCrumbs.Services;
using RoverCore.Navigation.Services;
using RoverCore.ToastNotification;
using Serviced;
using System;
using System.IO;
using System.Reflection;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using RoverCore.Boilerplate.Infrastructure.Common;
using RoverCore.Boilerplate.Infrastructure.Common.Email.Services;
using RoverCore.Boilerplate.Infrastructure.Common.Extensions;
using RoverCore.Boilerplate.Infrastructure.Common.Hangfire.Filters;
using RoverCore.Boilerplate.Infrastructure.Common.Seeder.Services;
using RoverCore.Boilerplate.Infrastructure.Common.Settings.Services;
using RoverCore.Boilerplate.Infrastructure.Common.Templates;
using RoverCore.Boilerplate.Infrastructure.Common.Templates.Services;
using RoverCore.Boilerplate.Infrastructure.Identity.Services;
using RoverCore.Boilerplate.Web.Jobs;

namespace RoverCore.Boilerplate.Web;

public class Startup
{
    public IConfiguration _configuration { get; }

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        // Auto-register services implementing IScoped, ITransient, ISingleton (thanks to Georgi Stoyanov)
        Infrastructure.Startup.ConfigureServicesDiscovery(services, typeof(Startup).Assembly, typeof(ApplicationSeederService).Assembly);

        // Configure infrastructure services (see Startup.cs of the Infrastructure project)
        Infrastructure.Startup.ConfigureServices(services, _configuration);

        services.AddRouting(options => options.LowercaseUrls = true) // Add routing with lowercase url configuration
            .AddCors() // Adds cross-origin sharing services
            .AddHttpContextAccessor();  // Add default HttpContextAccessor service

        // Add a cookie policy to the site
        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential 
            // cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            // requires using Microsoft.AspNetCore.Http;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

#if DEBUG
        // For development only - Display exceptions on page if there is an error
        services.AddDatabaseDeveloperPageExceptionFilter();
#endif

        // Add Mvc services
        services.AddMvc()
                .AddRazorRuntimeCompilation();

        services.AddRazorPages();

        // Add Swagger documentation
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "RoverCore.Boilerplate.Web API", Version = "v1" });

            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        // Add third-party presentation layer services
        services.AddScoped<IBreadCrumbService, BreadCrumbService>();
        services.AddScoped<NavigationService>();
        services.AddNotyf(config =>
        {
            config.DurationInSeconds = 10;
            config.IsDismissable = true;
            config.Position = NotyfPosition.BottomRight;
        });

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app)
    {
#if DEBUG
        app.UseDeveloperExceptionPage();
#else
		app.UseExceptionHandler("/error/500");
        app.UseHsts();
#endif

        app.UseStatusCodePagesWithReExecute("/error/{0}");

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCookiePolicy();
        app.UseRouting();

        // global cors policy
        app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        // Configure infrastructure middleware
        Infrastructure.Startup.Configure(app, _configuration);

        // Note that at least one controller route has to be named "default"
        // This route will be used to determine the base url for the site by the EmailSender
        // service
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();

            endpoints.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller}/{action=Index}/{id?}"
            );

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

        });

        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1"); });

        // Schedule Hangfire jobs -- Add your jobs in this method
        ConfigureJobs.Schedule();
    }
}