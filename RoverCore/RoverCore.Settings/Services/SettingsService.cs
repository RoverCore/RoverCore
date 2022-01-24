using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RoverCore.Settings.Models;

namespace RoverCore.Settings.Services
{
    public class SettingsService : SettingsService<Models.Settings>
    {
        public SettingsService(IConfiguration configuration, 
            ILogger<SettingsService<Models.Settings>> logger, 
            LinkGenerator link,
            IHttpContextAccessor httpContextAccessor) : base(configuration, logger, link, httpContextAccessor)
        {
        }
    }

    public class SettingsService<T> where T : Models.Settings, new()
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly LinkGenerator _link;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public T Settings { get; set; }

        public SettingsService(IConfiguration configuration, ILogger<SettingsService<T>> logger, LinkGenerator link, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _logger = logger;
            _link = link;
            _httpContextAccessor = httpContextAccessor;

            try
            {
                var roles = GetRoles(_httpContextAccessor.HttpContext?.User).ToList();

                Settings = _configuration.GetSection("Settings").Get<T>();
                Settings.NavMenu.NavMenuItems ??= new List<NavMenuItem>();

                // Convert any links constructed from routing dictionary values in the configuration
                // to their path counterparts
                ResolveUrls();

                // Filter out any links the current user can't access
                Settings.NavMenu.NavMenuItems = Settings.NavMenu.NavMenuItems
                    .Where(x => x.Roles is null || x.Roles.Count == 0 || x.Roles.Any(role => roles.Contains(role)))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to parse Settings from AppSettings.json");

                throw new Exception();
            }
        }

        /// <summary>
        /// Converts the routing parts specified in the configuration into actual urls
        /// </summary>
        private void ResolveUrls()
        {
            var items = Settings?.NavMenu?.NavMenuItems;
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext is null)
            {
                throw new Exception("HTTP Context is null when attempting to resolve urls");
            }

            if (items?.Count > 0)
            {
                foreach (var item in items)
                {
                    // If the url is already set then we will use that as the link
                    if (!String.IsNullOrEmpty(item.Url)) continue;

                    // Check to see if this is an MVC link or a Page
                    if (!String.IsNullOrEmpty(item.Controller))
                    {
                        item.Url = _link.GetPathByAction(httpContext, item.Action ?? "Index", item.Controller, item.Values);
                    }
                    else if (!String.IsNullOrEmpty(item.Page)) 
                    {
                        item.Url = _link.GetPathByPage(httpContext, item.Page, item.Handler, item.Values);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a list of roles a ClaimsPrincipal has
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public IEnumerable<string> GetRoles(ClaimsPrincipal? principal)
        {
            if (principal == null)
                return Enumerable.Empty<string>();

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
