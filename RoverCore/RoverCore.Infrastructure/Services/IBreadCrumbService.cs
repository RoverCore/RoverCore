namespace RoverCore.Infrastructure.Services;

public interface IBreadCrumbService
{
    List<BreadCrumb> BreadCrumbs { get; set; }

    IBreadCrumbService Add(string title, string url);

}

public static class BreadCrumbServiceExtensions
{
    public static IBreadCrumbService StartAt(this IBreadCrumbService breadCrumbService, string title, string url = null)
    {
        breadCrumbService.BreadCrumbs.Clear();
        return breadCrumbService.Add(title, url ?? "");
    }
    public static IBreadCrumbService Then(this IBreadCrumbService breadCrumbService, string title, string url = null)
    {
        return breadCrumbService.Add(title, url);
    }
}