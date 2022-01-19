using Rover.Web.Services;
using RoverCore.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NToastNotify;

namespace RoverCore.Web.Controllers;

public class BaseController : Controller
{
    private IBreadCrumbService? _breadCrumbInstance;
    private IToastNotification? _toastInstance;

    protected IToastNotification _toast => _toastInstance ??= HttpContext.RequestServices.GetService<IToastNotification>();

    protected IBreadCrumbService _breadCrumbService =>
        _breadCrumbInstance ??= HttpContext.RequestServices.GetService<IBreadCrumbService>();
}