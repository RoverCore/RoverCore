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
    public class SettingsService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly LinkGenerator _link;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Models.Settings Settings { get; set; }

        public SettingsService(IConfiguration configuration, ILogger<SettingsService> logger, LinkGenerator link, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _logger = logger;
            _link = link;
            _httpContextAccessor = httpContextAccessor;

            try
            {
                var roles = GetRoles(_httpContextAccessor.HttpContext.User).ToList();

                Settings = _configuration.GetSection("Settings").Get<Models.Settings>();
                Settings.NavMenu.NavMenuItems ??= new List<NavMenuItem>();

                ResolveUrls();

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

            if (items?.Count > 0)
            {
                foreach (var item in items)
                {
                    // If the url is already set then we will use that as the link
                    if (!String.IsNullOrEmpty(item.Url)) continue;

                    if (!String.IsNullOrEmpty(item.Controller))
                    {
                        item.Url = _link.GetPathByAction(_httpContextAccessor.HttpContext, item.Action ?? "Index", item.Controller, item.Values);
                    }
                    else if (!String.IsNullOrEmpty(item.Page))
                    {
                        item.Url = _link.GetPathByPage(_httpContextAccessor.HttpContext, item.Page, item.Handler, item.Values);
                    }
                }
            }
        }

        public IEnumerable<string> GetRoles(ClaimsPrincipal principal)
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
