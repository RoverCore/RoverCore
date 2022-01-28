using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoverCore.Infrastructure.Persistence.DbContexts;
using RoverCore.Infrastructure.Services.Seeder;

namespace RoverCore.Infrastructure.Extensions
{
    public static class HostExtensions
    {
        public static IWebHost RunSeeders(this IWebHost host)
        {
            using (var serviceScope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var seeder = serviceScope.ServiceProvider.GetService<ApplicationSeederService>();

                seeder?.SeedAsync().GetAwaiter().GetResult();
            }

            return host;
        }

        public static IWebHost RunMigrations(this IWebHost host)
        {
            using (var serviceScope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                context?.Database.Migrate();
            }

            return host;
        }
    }
}
