namespace HyperionCore.Infrastructure.Services;

public interface IBreadCrumbService
{
    List<BreadCrumb> BreadCrumbs { get; set; }

    void Add(string title, string url = null);

}