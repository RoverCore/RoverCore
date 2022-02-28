﻿using Microsoft.AspNetCore.Identity;
using RoverCore.Boilerplate.Domain.Entities.Identity;
using RoverCore.Boilerplate.Infrastructure.Common.Seeder.Services;

namespace RoverCore.Boilerplate.Infrastructure.Identity.Seeding;

public class DefaultTenantSeed : ISeeder
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public DefaultTenantSeed(RoleManager<ApplicationRole> roleManager)
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