using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RoverCore.Abstractions.Settings;
using RoverCore.Boilerplate.Domain.Entities;
using RoverCore.Boilerplate.Domain.Entities.Settings;
using RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts;
using Serviced;

namespace RoverCore.Boilerplate.Infrastructure.Common.Settings.Services;

public class SettingsService<T> : ISettingsService<T>
{
    private const string SettingsKey = "ApplicationSettings";
    private readonly ApplicationDbContext _context;
    private readonly T _settings;
    private readonly ILogger _logger;
    private readonly IMemoryCache _cache;

    public SettingsService(ApplicationDbContext context, T settings, ILogger<SettingsService<T>> logger, IMemoryCache cache)
    {
        _context = context;
        _settings = settings;
        _logger = logger;
        _cache = cache;
    }

    /// <summary>
    /// Loads persisted settings from database
    /// </summary>
    /// <returns></returns>
    public async Task LoadPersistedSettings()
    {
        var svConfig = _cache.Get<ConfigurationItem>(SettingsKey);

        svConfig ??= await InitializeCache();

        if (svConfig != null)
        {
            try
            {
                var savedSettingsJson = svConfig.Value ?? string.Empty;
                var savedSettings = JsonConvert.DeserializeObject<T>(savedSettingsJson);

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
    public T GetSettings()
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
    public async Task PatchSettings(T newSettings)
    {
        // Copy new settings to existing singleton service
        CopySettings(newSettings, _settings);

        await Commit(SettingsKey, JsonConvert.SerializeObject(_settings));
    }

    private void CopySettings(T? source, T target)
    {
        if (source == null) return;

        // Copy new settings to existing singleton service
        foreach (PropertyInfo property in typeof(T).GetProperties().Where(p => p.CanWrite))
        {
            var value = property.GetValue(source, null);

            if (value != null)
                property.SetValue(target, value, null);
        }
    }

    /// <summary>
    /// Persist the settings to storage
    /// </summary>
    /// <param name="key">key to store configuration under</param>
    /// <param name="value"></param>
    /// <returns></returns>
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

    private async Task<ConfigurationItem?> InitializeCache()
    {
        var svConfig = await _context.ConfigurationItem
            .FirstOrDefaultAsync(x => x.Key == SettingsKey);

        if (svConfig != null)
        {
            _cache.Set(SettingsKey, svConfig);
        }

        return svConfig;
    }
}