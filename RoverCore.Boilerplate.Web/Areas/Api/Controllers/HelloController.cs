using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoverCore.Boilerplate.Domain.Entities.Identity;
using System.Threading.Tasks;
using RoverCore.Boilerplate.Domain.Entities.Settings;
using RoverCore.Boilerplate.Infrastructure.Common.Cache;
using RoverCore.Boilerplate.Infrastructure.Common.Cache.Services;
using RoverCore.Boilerplate.Infrastructure.Common.Settings.Services;

namespace RoverCore.Boilerplate.Web.Areas.Api.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Produces("application/json")]
[Area("Api")]
public class HelloController : Controller
{
    private readonly SettingsService<ApplicationSettings> _settingsService;
    private readonly CacheService _cache;
    private readonly UserManager<ApplicationUser> _userManager;

    public HelloController(SettingsService<ApplicationSettings> settingsService, CacheService cache, UserManager<ApplicationUser> userManager)
    {
        _settingsService = settingsService;
        _cache = cache;
        _userManager = userManager;
    }

    // GET: api/hello/notprotected
    [AllowAnonymous]  // Don't require JWT authentication to access this method
    public object NotProtected()
    {
        return new
        {
            Result = "Hello random stranger!"
        };
    }

    // GET: api/hello/protected
    public async Task<object> Protected()
    {
        var user = await _userManager.GetUserAsync(User);

        return new
        {
            Result = $"Hello authenticated user {user.UserName}!"
        };
    }

}