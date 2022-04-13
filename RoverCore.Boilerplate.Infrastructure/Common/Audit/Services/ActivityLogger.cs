using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using RoverCore.Boilerplate.Domain.Entities.Audit;
using RoverCore.Boilerplate.Infrastructure.Identity.Extensions;
using RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts;

namespace RoverCore.Boilerplate.Infrastructure.Common.Audit.Services
{
    public class ActivityLogger
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ActivityLogger(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task AddAction(string state = "success")
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                var controllerActionDescriptor = httpContext
                    .GetEndpoint()?
                    .Metadata
                    .GetMetadata<ControllerActionDescriptor>();

                var controllerName = controllerActionDescriptor.ControllerName;

                var actionName = controllerActionDescriptor.ActionName;
                var userId = httpContext.User.Identity.IsAuthenticated ? httpContext.User.GetUserId() : string.Empty;

                var rd = httpContext.GetRouteData();

                try
                {
                    _context!.ActivityLog.Add(new ActivityLog
                    {
                        Service = controllerName,
                        Action = actionName,
                        UserId = userId,
                        State = state,
                        Metadata = JsonSerializer.Serialize(new
                        {
                            RouteData = rd
                        })
                    });
                    await _context.SaveChangesAsync();

                }
                catch (Exception)
                {
                    // Ignore
                }
            }
        }
    }
}
