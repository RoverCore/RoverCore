using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Identity;

namespace RoverCore.Boilerplate.Domain.Entities.Identity
{
	[MultiTenant]
	public class ApplicationUserToken : IdentityUserToken<string>
    {
        public virtual ApplicationUser User { get; set; }
    }
}
