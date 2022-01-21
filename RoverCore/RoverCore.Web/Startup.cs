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
using RoverCore.Domain.Entities.Identity;
using RoverCore.Infrastructure.Models.AuthenticationModels;
using RoverCore.Infrastructure.Models.SettingsModels;
using RoverCore.Infrastructure.Persistence.DbContexts;
using RoverCore.Infrastructure.Services;
using RoverCore.Settings.Services;
using RoverCore.ToastNotification;

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
        services.AddCors();
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(Configuration.GetConnectionString("AppContext"), x => x.MigrationsAssembly("RoverCore.Infrastructure"));
        });

        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // Add application services.
        services.AddTransient<IEmailSender, EmailSender>();

        // caching
        services.AddMemoryCache();
        services.AddTransient<Services.Cache>();

        services.AddTransient<RoverCore.Infrastructure.Services.Configuration>();
        services.AddRouting(options => options.LowercaseUrls = true);

#if DEBUG
        services.AddRazorPages().AddRazorRuntimeCompilation();
#endif
        services.AddMvc();

        // Add functionality to inject IOptions<T>
        services.AddOptions();



        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "RoverCore.Web API", Version = "v1" });

            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        // Add strongly-typed Settings object to services
        services.Configure<Settings>(Configuration.GetSection("Settings"));

        // configure strongly typed settings object
        // configure strongly typed settings objects
        var appSettingsSection = Configuration.GetSection("JWTSettings");
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
        // configure DI for application services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBreadCrumbService, BreadCrumbService>();
        services.AddScoped<SettingsService>();
        services.AddHttpContextAccessor();
        services.AddNotyf(config => { config.DurationInSeconds = 10; config.IsDismissable = true; config.Position = NotyfPosition.BottomRight; });

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