using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoverCore.Boilerplate.Infrastructure.Services;

namespace RoverCore.Boilerplate.Web.Areas.Api.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Produces("application/json")]
[Area("Api")]
public class HelloController : Controller
{
    private readonly RoverCore.Boilerplate.Infrastructure.Services.SettingsService _settingsService;
    private readonly CacheService _cache;

    public HelloController(RoverCore.Boilerplate.Infrastructure.Services.SettingsService settingsService, CacheService cache)
    {
        _settingsService = settingsService;
        _cache = cache;
    }

    // GET: api/v1/Sample
    [AllowAnonymous]  // Don't require JWT authentication to access this method
    [HttpGet("Sample")]
    public object Sample()
    {
        return new
        {
            Result = "Hello World"
        };
    }

}