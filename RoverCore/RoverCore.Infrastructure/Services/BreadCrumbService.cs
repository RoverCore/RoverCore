using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using UrlHelper = Microsoft.AspNetCore.Mvc.Routing.UrlHelper;

namespace RoverCore.Infrastructure.Services;

public class BreadCrumbService : IBreadCrumbService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LinkGenerator _link;

    public List<BreadCrumb> BreadCrumbs { get; set; }

    public BreadCrumbService(IHttpContextAccessor httpContextAccessor, LinkGenerator link)
    {
        BreadCrumbs = new List<BreadCrumb>();

        _httpContextAccessor = httpContextAccessor;
        _link = link;

        Add("Home", null);
    }

    public void Default(string title) => Default(title, "");

    public void Default(string title, string url)
    {
        BreadCrumbs.Clear();
        Add(title, url);
    }

    public void DefaultAction(string title, string action = default,
        string controller = default,
        object values = default,
        PathString? pathBase = default,
        FragmentString fragment = default,
        LinkOptions options = default)
    {
        BreadCrumbs.Clear();
        AddAction(title, action, controller, values, pathBase, fragment, options);
    }

    public void DefaultPage(string title, string? page = default,
        string? handler = default,
        object? values = default,
        PathString? pathBase = default,
        FragmentString fragment = default,
        LinkOptions? options = default)
    {
        BreadCrumbs.Clear();
        AddPage(title, page, handler, values, pathBase, fragment, options);
    }

    public void Add(string title)
    {
        Add(title, "");
    }

    public void Add (string title, string url)
    {
        BreadCrumbs.Add(new BreadCrumb
        {
            Title = title,
            Url = String.IsNullOrEmpty(url) ? null : url
        });
    }

    public void AddAction(string title, string action = default,
        string controller = default,
        object values = default,
        PathString? pathBase = default,
        FragmentString fragment = default,
        LinkOptions options = default)
    {
        string url = "";
        HttpContext? httpContext = _httpContextAccessor.HttpContext;

        if (httpContext != null)
        {
            url = _link.GetPathByAction(httpContext, action, controller, values, pathBase, fragment, options) ?? "";
        }
        
        Add(title, url);
    }

    public void AddPage(string title, string? page = default,
        string? handler = default,
        object? values = default,
        PathString? pathBase = default,
        FragmentString fragment = default,
        LinkOptions? options = default)
    {
        string url = "";
        HttpContext? httpContext = _httpContextAccessor.HttpContext;

        if (httpContext != null)
        {
            url = _link.GetPathByPage(httpContext, page, handler, values, pathBase, fragment, options) ?? "";
        }

        Add(title, url);
    }


}