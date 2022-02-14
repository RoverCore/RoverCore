using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RoverCore.Serviced;

namespace RoverCore.Boilerplate.Infrastructure.Common.Seeder.Services;

/// <summary>
/// This is the primary service responsible for calling all classes that implement the ISeeder
/// interface.  You'll note that it itself implements ISeeder, which allows it to be automatically
/// registered as a service.  
/// </summary>
public class ApplicationSeederService : ISeeder
{
    private readonly ILogger _logger;
    private readonly ServicedRegistryService _servicedRegistry;
    private readonly IServiceProvider _serviceProvider;

    public ApplicationSeederService(IServiceProvider serviceProvider, ILogger<ApplicationSeederService> logger,
        ServicedRegistryService servicedRegistry)
    {
        _logger = logger;
        _servicedRegistry = servicedRegistry;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Perform the seeding function by calling any other registered seeders.
    /// </summary>
    /// <returns></returns>
    public async Task SeedAsync()
    {
        // The service registry contains a list of all the automatically-registered
        // services using the Serviced service.  This code creates a list of all of
        // the other registered ISeeders in the assemblies specified in Startup.
        // (see services.AddServiced line)
        List<Type> seederTypes = _servicedRegistry.FilterServiceTypes<ISeeder>()
            .Where(t => t.Name != GetType().Name)
            .ToList();
        var test = _serviceProvider.GetServices<ISeeder>().ToList();

        _logger.LogInformation("ApplicationSeeder beginning execution");

        // Iterate through each of the seeders and call their seeding method
        foreach (var stype in seederTypes)
        {
            var seederService = _serviceProvider.GetService(stype);

            if (seederService != null)
            {
                var serviceName = seederService.GetType().Name;

                _logger.LogInformation($"Seeder {serviceName} started at {DateTime.UtcNow}.");

                await ((ISeeder)seederService).SeedAsync();

                _logger.LogInformation($"Seeder {serviceName} completed at {DateTime.UtcNow}.");
            }
        }

        _logger.LogInformation("ApplicationSeeder completed");
    }
}