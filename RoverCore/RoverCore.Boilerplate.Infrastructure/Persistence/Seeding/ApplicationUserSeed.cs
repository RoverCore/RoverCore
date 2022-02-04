using Microsoft.AspNetCore.Identity;
using RoverCore.Boilerplate.Domain.Entities.Identity;
using RoverCore.Boilerplate.Infrastructure.Services.Seeder;

namespace RoverCore.Boilerplate.Infrastructure.Persistence.Seeding;

public class ApplicationUserSeed : ISeeder
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ApplicationUserSeed(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public void CreateAdminUser()
    {
        if (_userManager.FindByNameAsync("admin").Result != null)
        {
            return;
        }

        var adminUser = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "admin",
            FirstName = "Admin"
        };

        IdentityResult result;
        try
        {
            result = _userManager.CreateAsync(adminUser, "Password123!").Result;
        }
        catch (Exception e)
        {
            throw new Exception("An error occurred while creating the admin user: " + e.InnerException);
        }

        if (!result.Succeeded)
        {
            throw new Exception("The following error(s) occurred while creating the admin user: " + string.Join(" ", result.Errors.Select(e => e.Description)));
        }

        _userManager.AddToRoleAsync(adminUser, "Admin").Wait();
    }

    public Task SeedAsync()
    {
        CreateAdminUser();

        return Task.CompletedTask;
    }
}