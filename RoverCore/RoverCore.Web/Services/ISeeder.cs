using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoverCore.Infrastructure.DbContexts;

namespace Rover.Web.Services;

public interface ISeeder
{
    Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider);
}