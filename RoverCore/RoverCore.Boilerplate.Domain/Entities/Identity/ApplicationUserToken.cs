using Microsoft.AspNetCore.Identity;

namespace RoverCore.Boilerplate.Domain.Entities.Identity
{
    public class ApplicationUserToken : IdentityUserToken<string>
    {
        public virtual ApplicationUser User { get; set; }
    }
}
