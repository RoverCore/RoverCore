using System;
using System.Collections.Generic;
using System.Linq;
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
                Settings = _configuration.GetSection("Settings").Get<Models.Settings>();
                Settings.NavMenu.NavMenuItems ??= new List<NavMenuItem>();

                ResolveUrls();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to parse Settings from AppSettings.json");

                throw new Exception();
            }
        }

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
                        var url = _link.GetPathByAction("Index", "Dashboard", new { Area = "Admin" }, "", FragmentString.Empty, null);
                        item.Url = _link.GetPathByAction(item.Action ?? "Index", item.Controller, item.Values);
                    }
                    else if (!String.IsNullOrEmpty(item.Page))
                    {
                        item.Url = _link.GetPathByPage(item.Page, item.Handler, item.Values);
                    }
                }
            }
        }
    }
}
