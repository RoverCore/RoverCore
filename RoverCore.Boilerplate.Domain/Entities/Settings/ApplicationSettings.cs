using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoverCore.Boilerplate.Domain.Entities.Settings
{
    /// <summary>
    /// Additional extra strongly-typed application settings for this web service
    /// The purpose of this file to provide configuration settings for the site.
    /// </summary>
    public class ApplicationSettings : CoreSettings
    {
        // Properties placed here can be added to the Settings section of appsettings.json
        [DisplayName("Site Name")]
        [Required(ErrorMessage = "You must supply a name for this site")]
        public string SiteName { get; set; } = string.Empty;
        [DisplayName("Absolute Base Url for Site")]
        [DataType(DataType.Url)]
        public string BaseUrl { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        [DisplayName("Small Logo Url")]
        [DataType(DataType.ImageUrl)] 
        public string LogoImageUrlSmall { get; set; } = string.Empty;
		public EmailSettings Email { get; set; }
    }
}
