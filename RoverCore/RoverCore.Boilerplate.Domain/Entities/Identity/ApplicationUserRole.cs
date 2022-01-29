using Microsoft.AspNetCore.Identity;

namespace RoverCore.Boilerplate.Domain.Entities.Identity
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationRole Role { get; set; }
    }
}
