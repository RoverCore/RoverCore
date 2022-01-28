using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RoverCore.Navigation.Models;

namespace RoverCore.Navigation.Services;

public class NavigationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;
    private readonly LinkGenerator _link;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private NavigationConfiguration NavigationConfiguration { get; set; }

    public NavigationService(IConfiguration configuration, ILogger<NavigationService> logger, LinkGenerator link, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _logger = logger;
        _link = link;
        _httpContextAccessor = httpContextAccessor;

        try
        {
            NavigationConfiguration = new NavigationConfiguration
            {
                Menus = _configuration.GetSection("Navigation").Get<List<NavMenu>>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to parse Navigation settings from AppSettings.json");

            throw new Exception();
        }
    }

    public NavMenu Menu(string? menuId = null)
    {
        var roles = GetRoles(_httpContextAccessor.HttpContext?.User).ToList();
        NavMenu? nav;

        if (menuId is null)
        {
            nav = NavigationConfiguration.Menus.FirstOrDefault();
        }
        else
        {
            nav = NavigationConfiguration.Menus.FirstOrDefault(x => x.MenuId == menuId) ??
                  NavigationConfiguration.Menus.FirstOrDefault();
        }

        nav ??= new NavMenu
        {
            NavMenuItems = new List<NavMenuItem>()
        };

        ResolveUrls(nav.NavMenuItems);

        nav.NavMenuItems = nav.NavMenuItems
            .Where(x => x.Roles is null || x.Roles.Count == 0 || x.Roles.Any(role => roles.Contains(role)))
            .ToList();

        return nav;
    }

    /// <summary>
    /// Converts the routing parts specified in the configuration into actual urls
    /// </summary>
    private void ResolveUrls(List<NavMenuItem> items)
    {
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