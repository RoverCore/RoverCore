using Microsoft.AspNetCore.Mvc;
using RoverCore.Settings.Services;

//using RoverCore.Settings.Models;
//using RoverCore.Settings.Services;

namespace RoverCore.Settings.Views.Shared.Components.Settings
{
    public class SettingsViewComponent : ViewComponent
    {
        private Models.Settings _settings { get; set; }

        public SettingsViewComponent(SettingsService _settingsService)
        {
            _settings = _settingsService.Settings;
        }

        public IViewComponentResult Invoke()
        {
            return View(_settings);
        }
    }
}
