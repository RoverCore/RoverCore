using System.ComponentModel.DataAnnotations;

namespace RoverCore.Domain.Entities;

public class ConfigurationItem
{
    [Key]
    public string Key { get; set; }

    public string Value { get; set; }
}