using Serviced;

namespace RoverCore.Boilerplate.Infrastructure.Common.Seeder.Services;

public interface ISeeder : IScoped
{
    Task SeedAsync();
}

public interface ISeeder<T> : ISeeder
{
}
