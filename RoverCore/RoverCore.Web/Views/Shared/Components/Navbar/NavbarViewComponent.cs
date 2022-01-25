using Microsoft.AspNetCore.Mvc;
using RoverCore.Settings.Services;
using RoverCore.Web.Models;

//using RoverCore.Settings.Models;
//using RoverCore.Settings.Services;

namespace RoverCore.Web.Views.Shared.Components.Navbar
{
    public class NavbarViewComponent : ViewComponent
    {
        private RoverCore.Settings.Models.Settings _settings { get; set; }

        public NavbarViewComponent(SettingsService<RoverCore.Settings.Models.Settings> _settingsService)
        {
            _settings = _settingsService.Settings;
        }

        public IViewComponentResult Invoke()
        {
            return View(_settings);
        }
    }
}
