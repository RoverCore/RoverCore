using System;

namespace RoverCore.Web.Models
{
    /// <summary>
    /// Additional extra local configuration settings for this web service
    /// </summary>
    public class ApplicationSettings 
    {
        // Properties placed here can be added to the Settings section of appsettings.json
        public string SiteName { get; set; } = String.Empty;
        public string Company { get; set; } = String.Empty;
    }
}
