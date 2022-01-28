using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RoverCore.Infrastructure.Persistence.DbContexts;
using RoverCore.Serviced;
using Serviced;

namespace RoverCore.Infrastructure.Services.Seeder;

public class ApplicationSeederService : ISeeder
{
    private readonly ILogger _logger;
    private readonly ServicedRegistryService _servicedRegistry;
    private readonly IServiceProvider _serviceProvider;

    public ApplicationSeederService(IServiceProvider serviceProvider, ILogger<ApplicationSeederService> logger, ServicedRegistryService servicedRegistry)
    {
        _logger = logger;
        _servicedRegistry = servicedRegistry;
        _serviceProvider = serviceProvider;
    }

    public async Task SeedAsync()
    {
        List<Type> seederTypes = _servicedRegistry.FilterServiceTypes<ISeeder>()
            .Where(t => t.Name != this.GetType().Name)
            .ToList();
        var test = _serviceProvider.GetServices<ISeeder>().ToList();
        _logger.LogInformation($"ApplicationSeeder beginning execution");

        foreach (var stype in seederTypes)
        {
            var seederService = _serviceProvider.GetService(stype);

            if (seederService != null)
            {
                await ((ISeeder)seederService).SeedAsync();

                _logger.LogInformation($"Seeder {seederService.GetType().Name} done.");
            }

        }

        _logger.LogInformation($"ApplicationSeeder completed");
    }

}