using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RoverCore.Settings.Models
{
    public class NavMenu
    {
        [JsonPropertyName("MenuId")]
        public string MenuId { get; set; } = String.Empty;

        [JsonPropertyName("NavMenuItems")]
        public List<NavMenuItem> NavMenuItems { get; set; }
    }
}
