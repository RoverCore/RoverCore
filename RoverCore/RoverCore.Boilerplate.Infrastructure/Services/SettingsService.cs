using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RoverCore.Boilerplate.Domain.Entities;
using RoverCore.Boilerplate.Domain.Entities.Settings;
using RoverCore.Boilerplate.Infrastructure.Extensions;
using RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts;
using Serviced;

namespace RoverCore.Boilerplate.Infrastructure.Services;

public class SettingsService : IScoped
{
	private const string SettingsKey = "ApplicationSettings";
    private readonly ApplicationDbContext _context;
    private readonly ApplicationSettings _settings;
    private readonly Logger<SettingsService> _logger;

    public SettingsService(ApplicationDbContext context, ApplicationSettings settings, Logger<SettingsService> logger)
    {
        _context = context;
        _settings = settings;
        _logger = logger;
    }

    /// <summary>
    /// Loads persisted settings from database
    /// </summary>
    /// <returns></returns>
    public async Task LoadPersistedSettings()
    {
	    var svConfig = await _context.ConfigurationItem
		    .FirstOrDefaultAsync(x => x.Key == SettingsKey);

	    if (svConfig != null)
	    {
		    try
		    {
			    var savedSettingsJson = svConfig.Value ?? string.Empty;
			    var savedSettings = JsonConvert.DeserializeObject<ApplicationSettings>(savedSettingsJson);

			    // Copy saved settings to existing singleton service
			    CopySettings(savedSettings!, _settings);

            }
            catch (Exception ex)
		    {
                _logger.LogError(ex, $"Unable to load {SettingsKey} settings from database");
		    }
        }
    }

    /// <summary>
    /// Returns the settings service singleton reference
    /// </summary>
    /// <returns></returns>
    public ApplicationSettings GetSettings()
    {
	    return _settings;
    }

    /// <summary>
    /// Saves the existing application settings to the database
    /// </summary>
    /// <returns></returns>
    public async Task SaveSettings()
    {
	    await Commit(SettingsKey, JsonConvert.SerializeObject(_settings));
    }

    /// <summary>
    /// Copies the values for each property in settings to the ApplicationSettings singleton and saves a copy in the database
    /// </summary>
    /// <param name="newSettings"></param>
    /// <returns></returns>
    public async Task PatchSettings(ApplicationSettings newSettings)
    {
        // Copy new settings to existing singleton service
	    CopySettings(newSettings, _settings);

        await Commit(SettingsKey, JsonConvert.SerializeObject(_settings));
    }

    private void CopySettings(ApplicationSettings? source, ApplicationSettings target)
    {
	    if (source == null) return;

	    // Copy new settings to existing singleton service
	    foreach (PropertyInfo property in typeof(ApplicationSettings).GetProperties().Where(p => p.CanWrite))
	    {
		    var value = property.GetValue(source, null);

            if (value != null)
	            property.SetValue(target, value, null);
	    }
    }

    private async Task Commit(string key, string value)
    {
        var configItem = await _context.ConfigurationItem
	        .FirstOrDefaultAsync(x => x.Key == key);

        if (configItem is null)
        {
	        configItem = new ConfigurationItem
	        {
		        Key = key,
		        Value = value
	        };
	        _context.Add(configItem);
        }

        configItem.Value = value;

        await _context.SaveChangesAsync();
    }

}