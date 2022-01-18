using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace Rover.Web;

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


        try
        {
            Log.Information("Starting web host");
            BuildWebHost(args).Run();
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

    public static IWebHost BuildWebHost(string[] args)
    {
        var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddEnvironmentVariables().Build();

        return WebHost.CreateDefaultBuilder(args)
            .UseConfiguration(configuration)
            .UseStartup<Startup>()
            .Build();
    }
}