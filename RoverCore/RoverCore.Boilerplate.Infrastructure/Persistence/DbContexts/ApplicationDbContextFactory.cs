using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts
{
    /// <summary>
    /// This factory is used to create a dbContext for design-time tools (such as scaffolding).  Since they are only
    /// used for development only the development appSettings are needed.
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        ApplicationDbContext IDesignTimeDbContextFactory<ApplicationDbContext>.CreateDbContext(string[] args)
        {
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var path = AppContext.BaseDirectory;

            if (String.IsNullOrEmpty(envName))
                envName = "Development";

            Console.WriteLine($"ApplicationDbContextFactory: Using base path = {path}");

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile($"appsettings.json")
                .AddJsonFile($"appsettings.{envName}.json")
                .Build();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("AppContext");

            builder.UseSqlServer(connectionString);

            return new ApplicationDbContext(builder.Options);
        }
    }
}
