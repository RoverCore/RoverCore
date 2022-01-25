﻿using System;

namespace RoverCore.Web.Models
{
    /// <summary>
    /// Additional extra strongly-typed application settings for this web service
    /// The purpose of this file is mainly to provide configuration settings for
    /// </summary>
    public class ApplicationSettings 
    {
        // Properties placed here can be added to the Settings section of appsettings.json
        public string SiteName { get; set; } = String.Empty;
        public string Company { get; set; } = String.Empty;
    }
}
