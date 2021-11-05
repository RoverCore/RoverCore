using Hyperion.Web.Data;
using Hyperion.Web.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hyperion.Web.Services
{
    public class Configuration
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        
        public Configuration(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public string Get(string key)
        {
            var config = _cache.Get<List<ConfigurationItem>>("Configuration");

            if(config == null)
            {
                config = InitializeConfigCache();
            }

            return config.FirstOrDefault(x => x.Key == key)?.Value;
        }

        public void Set(string key, string value)
        {
            var configItem = new ConfigurationItem
            {
                Key = key,
                Value = Get(key)
            };

            if (configItem.Value != null)
            {
                _context.Attach(configItem);
            }

            configItem.Value = value;

            _context.AddOrUpdate(configItem);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
            InitializeConfigCache();
        }

        private List<ConfigurationItem> InitializeConfigCache()
        {
            var config = _context.ConfigurationItem.ToList();
            _cache.Set("Configuration", config);
            return config;
        }
    }
}
