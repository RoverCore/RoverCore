using Microsoft.AspNetCore.Mvc;

namespace HyperionCore.Web.Views.Shared.Components.Breadcrumbs
{
    public class BreadcrumbsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
