using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Identity;

namespace RoverCore.Boilerplate.Domain.Entities.Identity;

public class ApplicationRole : IdentityRole<string>
{
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; }

}