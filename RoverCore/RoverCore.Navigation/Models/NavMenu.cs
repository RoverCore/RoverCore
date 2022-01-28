using System.Text.Json.Serialization;

namespace RoverCore.Navigation.Models;

public class NavMenu
{
    [JsonPropertyName("MenuId")]
    public string MenuId { get; set; } = String.Empty;

    [JsonPropertyName("NavMenuItems")]
    public List<NavMenuItem> NavMenuItems { get; set; }
}