using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RoverCore.Infrastructure.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static IEnumerable<string> GetRoles(this ClaimsPrincipal principal)
        {
            return principal.Identities.SelectMany(i =>
            {
                return i.Claims
                    .Where(c => c.Type == i.RoleClaimType)
                    .Select(c => c.Value)
                    .ToList();
            });
        }
    }
}
