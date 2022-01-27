using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace RoverCore.Infrastructure.Services.Seeder;

public class ApplicationSeederService : ISeeder
{
    private readonly ILogger _logger;

    public ApplicationSeederService(ILogger<ApplicationSeederService> logger)
    {
        _logger = logger;
    }

    public async Task SeedAsync(IServiceProvider serviceProvider)
    {
        /*
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

        if (dbContext == null)
        {
            throw new ArgumentNullException(nameof(dbContext));
        }
        */
        if (serviceProvider == null)
        {
            throw new ArgumentNullException(nameof(serviceProvider));
        }

        _logger.LogInformation($"ApplicationSeeder beginning execution");

        var seeders = GetInstances<ISeeder>();

        foreach (var seeder in seeders)
        {
            await seeder.SeedAsync(serviceProvider);
            _logger.LogInformation($"Seeder {seeder.GetType().Name} done.");
        }

        _logger.LogInformation($"ApplicationSeeder completed");

    }

    private List<T> GetInstances<T>()
    {
        return (from t in Assembly.GetExecutingAssembly().GetTypes()
            where t.GetInterfaces().Contains(typeof(T)) && t.GetConstructor(Type.EmptyTypes) != null && t.Name != this.GetType().Name
            select (T)Activator.CreateInstance(t)).ToList();
    }
    

}