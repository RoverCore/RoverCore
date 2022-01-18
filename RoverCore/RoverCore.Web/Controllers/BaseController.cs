using AspNetCoreHero.ToastNotification.Abstractions;
using Rover.Web.Services;
using RoverCore.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace RoverCore.Web.Controllers;

public class BaseController : Controller
{
    private IBreadCrumbService _breadCrumbInstance;
    private INotyfService _notifyInstance;

    protected INotyfService _notify => _notifyInstance ??= HttpContext.RequestServices.GetService<INotyfService>();

    protected IBreadCrumbService _breadCrumbService =>
        _breadCrumbInstance ??= HttpContext.RequestServices.GetService<IBreadCrumbService>();
}