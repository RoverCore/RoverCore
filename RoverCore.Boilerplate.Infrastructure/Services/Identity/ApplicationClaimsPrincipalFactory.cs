using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RoverCore.Boilerplate.Domain.Entities.Identity;
using System.Security.Claims;

namespace RoverCore.Boilerplate.Infrastructure.Services.Identity;

public class ApplicationClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
{
    public ApplicationClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager
        , RoleManager<ApplicationRole> roleManager
        , IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    {
    }

    public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
    {
        var principal = await base.CreateAsync(user);
        if (principal.Identity != null && !string.IsNullOrWhiteSpace(user.FirstName))
            ((ClaimsIdentity)principal.Identity).AddClaims(new[]
            {
                new Claim(ClaimTypes.GivenName, user.FirstName)
            });

        if (principal.Identity != null && !string.IsNullOrWhiteSpace(user.LastName))
            ((ClaimsIdentity)principal.Identity).AddClaims(new[]
            {
                new Claim(ClaimTypes.Surname, user.LastName)
            });
        return principal;
    }
}