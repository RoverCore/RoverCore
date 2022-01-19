using Microsoft.AspNetCore.Http;
using UrlHelper = Microsoft.AspNetCore.Mvc.Routing.UrlHelper;

namespace RoverCore.Infrastructure.Services;

public class BreadCrumbService : IBreadCrumbService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public List<BreadCrumb> BreadCrumbs { get; set; }

    public BreadCrumbService(IHttpContextAccessor httpContextAccessor)
    {
        BreadCrumbs = new List<BreadCrumb>();
        _httpContextAccessor = httpContextAccessor;

        StartAt("Home", null);
    }

    public IBreadCrumbService StartAt(string title, string url = null)
    {
        BreadCrumbs.Clear();

        return Then(title, url);
    }

    public IBreadCrumbService Then (string title, string url = null)
    {
        BreadCrumbs.Add(new BreadCrumb
        {
            Title = title,
            Url = url
        });

        return this;
    }
}