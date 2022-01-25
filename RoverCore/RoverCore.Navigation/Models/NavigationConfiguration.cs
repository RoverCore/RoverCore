using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RoverCore.Settings.Models;

namespace RoverCore.Settings.Models
{
    public class NavigationConfiguration
    {
        public List<NavMenu> Menus { get; set; }
    }
}
