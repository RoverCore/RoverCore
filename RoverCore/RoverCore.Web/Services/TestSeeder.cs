using Rover.Web.Services;
using System;
using System.Threading.Tasks;
using RoverCore.Infrastructure.Persistence.DbContexts;
using RoverCore.Infrastructure.Services.Seeder;

namespace RoverCore.Web.Services
{
    public class TestSeeder : ISeeder
    {
        private readonly ApplicationDbContext _context;

        public TestSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task SeedAsync(IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }
    }
}
