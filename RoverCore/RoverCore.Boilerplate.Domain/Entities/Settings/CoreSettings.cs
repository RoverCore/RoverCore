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
        public bool ApplyMigrationsOnStartup { get; set; } = true;
        /// <summary>
        /// Any ISeeder seeders in the project will run on startup if true
        /// </summary>
        public bool SeedDataOnStartup { get; set; } = true;
    }
}
