using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Linq;
using RoverCore.Boilerplate.Infrastructure.Common.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace RoverCore.Boilerplate.Web;

public class Program
{
    public static void Main(string[] args)
    {
        string logPath = "Logs" + Path.DirectorySeparatorChar;
        Directory.CreateDirectory(logPath);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Debug()
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day,
                flushToDiskInterval: TimeSpan.FromSeconds(1),
                shared: true)
            .CreateLogger();

        bool overrideMigration = false, overrideSeed = false;

        // Process command-line switches
        if (args.Contains("--migrate")) overrideMigration = true;
        if (args.Contains("--seed")) overrideSeed = true;

        if (overrideSeed || overrideMigration)
        {
            Log.Information("Starting seeding/migration process");
            BuildWebApplication(args)
                .RunMigrations(overrideMigration) // Apply any new EF migrations
                .RunSeeders(overrideSeed); // Run any auto-registered seeders

            return;
        }

        try
        {
            Log.Information("Starting web host");
            BuildWebApplication(args)
                .RunMigrations()  // Apply any new EF migrations (requires appsetting)
                .RunSeeders()     // Run any auto-registered seeders (classes that implement ISeeder) (requires appsetting)
                .Run();           // Start the web host
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }

    }

    public static WebApplication BuildWebApplication(string[] args)
    {
        var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables()
            .Build();

        var builder = WebApplication.CreateBuilder();

        builder.Configuration.AddEnvironmentVariables();
        builder.Host.UseSerilog(Log.Logger);

        var startup = new Startup(builder.Configuration);
        startup.ConfigureServices(builder.Services);

        var app = builder.Build();
        startup.Configure(app);

        return app;
    }

}

