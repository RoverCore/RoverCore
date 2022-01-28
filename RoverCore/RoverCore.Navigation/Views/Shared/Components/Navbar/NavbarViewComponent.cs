using Microsoft.AspNetCore.Mvc;
using RoverCore.Navigation.Services;

namespace RoverCore.Navigation.Views.Shared.Components.Navbar;

public class NavbarViewComponent : ViewComponent
{
    private NavigationService _navigationService { get; set; }

    public NavbarViewComponent(NavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public IViewComponentResult Invoke(string? menuId = null)
    {
        var menu = _navigationService.Menu(menuId);

        return View(menu);
    }
}