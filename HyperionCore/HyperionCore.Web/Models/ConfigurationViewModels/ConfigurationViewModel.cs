using System.ComponentModel.DataAnnotations;

namespace Hyperion.Web.Models.ConfigurationViewModels
{
    public class ConfigurationViewModel
    {
        [Required]
        [DataType(DataType.Url)]
        [Display(Name = "Feed URL")]
        public string RSSFeedUrl { get; set; }

        [Required]
        [DataType(DataType.Url)]
        [Display(Name = "Mobile App Privacy Policy URL")]
        public string PrivacyPolicyUrl { get; set; }

        [Required]
        [Display(Name = "Listen (Public) Azure Hub Connection String")]
        public string AzureHubListenConnectionString { get; set; }

        [Required]
        [Display(Name = "Full (Private) Azure Hub Connection String")]
        public string AzureHubFullConnectionString { get; set; }

        [Required]
        [Display(Name = "Azure Hub Name")]
        public string AzureHubName { get; set; }
    }
}
