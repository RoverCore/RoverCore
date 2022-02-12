using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoverCore.Boilerplate.Domain.Entities.Settings
{
    public class EmailSettings
    {
		[Required]
		[DisplayName("Default Sender Address")]
		[DataType(DataType.EmailAddress)] 
		public string DefaultSenderAddress { get; set; } = string.Empty;
		[Required]
		[DisplayName("Default Sender Name")] 
		public string DefaultSenderName { get; set; } = string.Empty;
	    public string Server { get; set; } = string.Empty;
		public int Port { get; set; } = 25;
	    public string User { get; set; } = string.Empty;
	    [DataType(DataType.Password)]
		public string Password { get; set; } = string.Empty;
	    [DisplayName("Use SSL?")]
		public bool UseSsl { get; set; } = false;
		[DisplayName("Require Authentication?")]
		public bool RequiresAuthentication { get; set; } = false;
		[DisplayName("Preferred Encoding")]
		public string PreferredEncoding { get; set; } = string.Empty;
		[DisplayName("Use Pickup Directory?")]
		public bool UsePickupDirectory { get; set; } = false;
		[DisplayName("Mail Pickup Directory Server Path")]
		public string MailPickupDirectory { get; set; } = string.Empty;
    }
}
