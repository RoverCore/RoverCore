﻿using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Identity;

namespace RoverCore.Boilerplate.Domain.Entities.Identity
{
	[MultiTenant]
	public class ApplicationRoleClaim : IdentityRoleClaim<string>
    {
        public virtual ApplicationRole Role { get; set; }
    }
}
