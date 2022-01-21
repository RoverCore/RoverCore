using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RoverCore.Settings.Models
{
    public class NavMenuItem
    {
        [JsonPropertyName("Text")]
        public string? Text { get; set; }

        [JsonPropertyName("Type")]
        public string? Type { get; set; }

        [JsonPropertyName("Controller")]
        public string? Controller { get; set; }

        [JsonPropertyName("Action")]
        public string? Action { get; set; }

        [JsonPropertyName("Values")]
        public Dictionary<string, object> Values { get; set; }

        [JsonPropertyName("Handler")]
        public string? Handler { get; set; }

        [JsonPropertyName("Page")]
        public string? Page { get; set; }

        [JsonPropertyName("Icon")]
        public string? Icon { get; set; }

        [JsonPropertyName("Url")]
        public string? Url { get; set; }

        [JsonPropertyName("Children")]
        public List<NavMenuItem>? Children { get; set; }
    }
}
