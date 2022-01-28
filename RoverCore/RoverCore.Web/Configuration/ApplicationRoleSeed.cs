using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rover.Web.Services;
using RoverCore.Domain.Entities.Identity;
using RoverCore.Infrastructure.Persistence.DbContexts;
using RoverCore.Infrastructure.Services.Seeder;

namespace Rover.Web.Configuration;

public class ApplicationRoleSeed : ISeeder
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public ApplicationRoleSeed(RoleManager<ApplicationRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public void CreateRoles()
    {
        var roles = new List<string>
        {
            "Admin",
            "User"
        };

        foreach (var roleName in roles)
        {
            if (!_roleManager.RoleExistsAsync(roleName).Result)
            {
                var role = new ApplicationRole { Id = Guid.NewGuid().ToString(), Name = roleName };

                _roleManager.CreateAsync(role).Wait();
            }
        }
    }

    public Task SeedAsync()
    {
        CreateRoles();

        return Task.CompletedTask;
    }
}