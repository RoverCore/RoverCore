using Rover.Web.Services;
using RoverCore.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace RoverCore.Web.Controllers;

public class BaseController : Controller
{
    private IBreadCrumbService? _breadCrumbInstance;
    private INotyfService _toastInstance;

    protected INotyfService _toast => _toastInstance ??= HttpContext.RequestServices.GetService<INotyfService>();


    protected IBreadCrumbService _breadCrumbService =>
        _breadCrumbInstance ??= HttpContext.RequestServices.GetService<IBreadCrumbService>();
}