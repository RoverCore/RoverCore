using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HyperionCore.Infrastructure.DbContexts;

namespace Hyperion.Web.Services;

public interface ISeeder
{
    Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider);
}