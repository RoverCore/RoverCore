using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RoverCore.Boilerplate.Domain.Entities.Settings
{
    /// <summary>
    /// Core strongly-typed application settings for this web service
    /// The purpose of this file is mainly to provide configuration settings for
    /// </summary>
    public class CoreSettings
    {
        /// <summary>
        /// Entity Framework migrations can be applied when the application starts up if true
        /// </summary>
        [DisplayName("Apply Migrations On Startup")]
        [Editable(false)]
        public bool ApplyMigrationsOnStartup { get; set; } = true;
        /// <summary>
        /// Any ISeeder seeders in the project will run on startup if true
        /// </summary>
        [DisplayName("Seed Data On Startup")]
        [Editable(false)] 
        public bool SeedDataOnStartup { get; set; } = true;
    }

}
