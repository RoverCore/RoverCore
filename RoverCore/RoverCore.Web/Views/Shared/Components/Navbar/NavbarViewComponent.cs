using Microsoft.AspNetCore.Mvc;
using RoverCore.Settings.Services;
using RoverCore.Web.Models;

//using RoverCore.Settings.Models;
//using RoverCore.Settings.Services;

namespace RoverCore.Web.Views.Shared.Components.Navbar
{
    public class NavbarViewComponent : ViewComponent
    {
        private LocalSettings _settings { get; set; }

        public NavbarViewComponent(SettingsService<LocalSettings> _settingsService)
        {
            _settings = _settingsService.Settings;
        }

        public IViewComponentResult Invoke()
        {
            return View(_settings);
        }
    }
}
