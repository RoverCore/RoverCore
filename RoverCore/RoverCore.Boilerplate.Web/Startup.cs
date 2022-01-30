using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RoverCore.BreadCrumbs.Services;
using RoverCore.Boilerplate.Domain.Entities.Identity;
using RoverCore.Boilerplate.Infrastructure.Extensions;
using RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts;
using RoverCore.Boilerplate.Infrastructure.Services;
using RoverCore.Boilerplate.Infrastructure.Services.Identity;
using RoverCore.Boilerplate.Infrastructure.Services.Seeder;
using RoverCore.Navigation.Services;
using RoverCore.ToastNotification;
using Serviced;

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
        services.AddServiced(typeof(Startup).Assembly,
            typeof(ApplicationSeederService).Assembly);

        // Settings and configuration services
        services.AddSettings(_configuration) // Add ApplicationsSettings service
            .AddOptions(); // Adds IOptions capabilities        

        // RoverCore infrastructure services - These extension methods can be adapted to set up additional services
        services.AddPersistence(_configuration) // Add services that persist data (ef core,etc)
            .AddAuthenticationScheme(_configuration)  // Adds authentication services
            .AddCaching();  // Adds caching services

        // Add custom identity user, roles, etc.
        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddClaimsPrincipalFactory<ApplicationClaimsPrincipalFactory>()
            .AddDefaultTokenProviders();

        services.AddRouting(options => options.LowercaseUrls = true) // Add routing with lowercase url configuration
            .AddCors() // Adds cross-origin sharing services
            .AddHttpContextAccessor();  // Add default HttpContextAccessor service

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
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "RoverCore.Boilerplate.Web API", Version = "v1" });

            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        // Configure email service
        services.AddTransient<IEmailSender, EmailSender>();

        // Add third-party application layer services
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
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
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
	    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1"); });

	    // Load configuration settings from database
	    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
	    {
		    var settingsService = serviceScope.ServiceProvider.GetRequiredService<SettingsService>();
		    settingsService.LoadPersistedSettings().GetAwaiter().GetResult();
	    }
    }
}