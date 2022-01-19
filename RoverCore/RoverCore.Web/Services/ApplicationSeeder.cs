using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using RoverCore.Infrastructure.Persistence.DbContexts;

namespace Rover.Web.Services;

public class ApplicationSeeder : ISeeder
{
    public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
    {
        if (dbContext == null)
        {
            throw new ArgumentNullException(nameof(dbContext));
        }

        if (serviceProvider == null)
        {
            throw new ArgumentNullException(nameof(serviceProvider));
        }

        var seeders = GetInstances<ISeeder>();

        foreach (var seeder in seeders)
        {
            await seeder.SeedAsync(dbContext, serviceProvider);
            await dbContext.SaveChangesAsync();
            Log.Information($"Seeder {seeder.GetType().Name} done.");
        }
    }

    private List<T> GetInstances<T>()
    {
        return (from t in Assembly.GetExecutingAssembly().GetTypes()
            where t.GetInterfaces().Contains(typeof(T)) && t.GetConstructor(Type.EmptyTypes) != null && t.Name != this.GetType().Name
            select (T)Activator.CreateInstance(t)).ToList();
    }
}