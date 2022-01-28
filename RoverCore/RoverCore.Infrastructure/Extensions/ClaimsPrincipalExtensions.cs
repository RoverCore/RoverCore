using System.Security.Claims;

namespace RoverCore.Infrastructure.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Returns the principal user email claim value
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static string GetUserEmail(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.Email) ?? String.Empty;
        }

        /// <summary>
        /// Returns the principal user id identifier claim value
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? String.Empty;
        }

        /// <summary>
        /// Returns the principal user username
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static string GetUsername(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.Name) ?? String.Empty;
        }

        public static string FirstName(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.GivenName) ?? String.Empty;
        }


        public static string LastName(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.Surname) ?? String.Empty;
        }

        /// <summary>
        /// Checks to see if the user given by id matches the principal user id
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsCurrentUser(this ClaimsPrincipal principal, string id)
        {
            var currentUserId = GetUserId(principal);

            return string.Equals(currentUserId, id, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns the roles held by principal
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
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
