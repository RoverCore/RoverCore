using Rover.Web.Models.ApiModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using RoverCore.Infrastructure.Services;

namespace Rover.Web.Controllers;

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