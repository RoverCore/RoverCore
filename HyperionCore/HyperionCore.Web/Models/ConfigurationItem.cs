using System.ComponentModel.DataAnnotations;

namespace Hyperion.Web.Models
{
    public class ConfigurationItem
    {
        [Key]
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
