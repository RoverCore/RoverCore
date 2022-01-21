using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RoverCore.Settings.Models;

namespace RoverCore.Settings.Models
{
    public class Settings
    {
        [JsonPropertyName("SiteName")]
        public string SiteName { get; set; }
        [JsonPropertyName("Company")]
        public string Company { get; set; }
        [JsonPropertyName("NavMenu")]
        public NavMenu NavMenu { get; set; }
    }
}
