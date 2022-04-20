using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RoverCore.BreadCrumbs.Services;
using RoverCore.ToastNotification.Abstractions;

namespace RoverCore.Boilerplate.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class BaseController<T> : Controller where T : Controller
{
    private IBreadCrumbService _breadCrumbInstance;
    private INotyfService _toastInstance;
    private ILogger _loggerInstance;

    protected INotyfService _toast => _toastInstance ??= HttpContext.RequestServices.GetService<INotyfService>();
    protected IBreadCrumbService _breadcrumbs =>
        _breadCrumbInstance ??= HttpContext.RequestServices.GetService<IBreadCrumbService>();
    protected ILogger _logger => _loggerInstance ??= HttpContext.RequestServices.GetService<ILogger<T>>();
}