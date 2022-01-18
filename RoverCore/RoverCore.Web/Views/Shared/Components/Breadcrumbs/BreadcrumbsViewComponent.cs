using Microsoft.AspNetCore.Mvc;

namespace RoverCore.Web.Views.Shared.Components.Breadcrumbs;

public class BreadcrumbsViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}