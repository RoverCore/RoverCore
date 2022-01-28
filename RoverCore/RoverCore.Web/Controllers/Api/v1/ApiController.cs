using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoverCore.Infrastructure.Services;

namespace RoverCore.Web.Controllers.Api.v1;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Produces("application/json")]
[Route("api/v1")]
public class ApiController : Controller
{
    private readonly RoverCore.Infrastructure.Services.Configuration Configuration;
    private readonly CacheService _cache;

    public ApiController(RoverCore.Infrastructure.Services.Configuration configuration, CacheService cache)
    {
        Configuration = configuration;
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