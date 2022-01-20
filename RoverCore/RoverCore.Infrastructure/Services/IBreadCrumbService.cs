using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoverCore.Infrastructure.Services;

public interface IBreadCrumbService
{
    List<BreadCrumb> BreadCrumbs { get; set; }

    void Default(string title);
    void Default(string title, string url);

    void DefaultAction(string title, string action = default,
        string controller = default,
        object values = default,
        PathString? pathBase = default,
        FragmentString fragment = default,
        LinkOptions options = default);
    void DefaultPage(string title, string? page = default,
    string? handler = default,
    object? values = default,
        PathString? pathBase = default,
    FragmentString fragment = default,
        LinkOptions? options = default);

    void Add(string title);
    void Add(string title, string url);

    void AddAction(string title, string action = default,
        string controller = default,
        object values = default,
        PathString? pathBase = default,
        FragmentString fragment = default,
        LinkOptions options = default);

    void AddPage(string title, string? page = default,
        string? handler = default,
        object? values = default,
        PathString? pathBase = default,
        FragmentString fragment = default,
        LinkOptions? options = default);
}

public static class BreadCrumbServiceExtensions
{
    public static IBreadCrumbService StartAt(this IBreadCrumbService breadCrumbService, string title)
    {
        breadCrumbService.Default(title);
        return breadCrumbService;
    }

    public static IBreadCrumbService StartAt(this IBreadCrumbService breadCrumbService, string title, string url)
    {
        breadCrumbService.Default(title, url ?? "");
        return breadCrumbService;
    }
    public static IBreadCrumbService StartAtAction(this IBreadCrumbService breadCrumbService, string title, string action = default,
        string controller = default,
        object values = default,
        PathString? pathBase = default,
        FragmentString fragment = default,
        LinkOptions options = default)
    {
        breadCrumbService.DefaultAction(title, action, controller, values, pathBase, fragment, options);
        return breadCrumbService;
    }

    public static IBreadCrumbService StartAtPage(this IBreadCrumbService breadCrumbService, string title,
        string? page = default,
        string? handler = default,
        object? values = default,
        PathString? pathBase = default,
        FragmentString fragment = default,
        LinkOptions? options = default)
    {
        breadCrumbService.DefaultPage(title, page, handler, values, pathBase, fragment, options);
        return breadCrumbService;
    }

    public static IBreadCrumbService Then(this IBreadCrumbService breadCrumbService, string title)
    {
        breadCrumbService.Add(title);
        return breadCrumbService;
    }

    public static IBreadCrumbService Then(this IBreadCrumbService breadCrumbService, string title, string url)
    {
        breadCrumbService.Add(title, url);
        return breadCrumbService;
    }

    public static IBreadCrumbService ThenAction(this IBreadCrumbService breadCrumbService, string title,
        string action = default,
        string controller = default,
        object values = default,
        PathString? pathBase = default,
        FragmentString fragment = default,
        LinkOptions options = default)
    {
        breadCrumbService.AddAction(title, action, controller, values, pathBase, fragment, options);
        return breadCrumbService;
    }

    public static IBreadCrumbService ThenPage(this IBreadCrumbService breadCrumbService, string title,
        string? page = default,
        string? handler = default,
        object? values = default,
        PathString? pathBase = default,
        FragmentString fragment = default,
        LinkOptions? options = default)
    {
        breadCrumbService.AddPage(title, page, handler, values, pathBase, fragment, options);
        return breadCrumbService;
    }
}