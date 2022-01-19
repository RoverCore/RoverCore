namespace RoverCore.Infrastructure.Services;

public interface IBreadCrumbService
{
    List<BreadCrumb> BreadCrumbs { get; set; }

    IBreadCrumbService StartAt(string title, string url = null);
    IBreadCrumbService Then(string title, string url = null);

}