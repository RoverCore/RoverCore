namespace RoverCore.Infrastructure.Services.Seeder;

public interface ISeeder
{
    Task SeedAsync(IServiceProvider serviceProvider);
}