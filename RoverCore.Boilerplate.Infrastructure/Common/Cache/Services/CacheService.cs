using Microsoft.Extensions.Caching.Memory;
using RoverCore.Boilerplate.Infrastructure.Common.Cache.Models;
using Serviced;

namespace RoverCore.Boilerplate.Infrastructure.Common.Cache.Services;

public class CacheService : ITransient
{
    private readonly IMemoryCache _cache;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key, Func<Task<T>> dataFetchFunction, TimeSpan lifeSpan)
    {
        T? oRet;

        var cachedData = _cache.Get<CacheItem<T>>(key);
        if (cachedData == null || cachedData.IsExpired())
        {
            var sourceResponse = await dataFetchFunction();
            if (sourceResponse != null)
            {
                // store in in-memory cache
                cachedData = new CacheItem<T>
                {
                    Created = DateTime.Now,
                    LifeSpan = lifeSpan,
                    Data = sourceResponse
                };
                _cache.Set(key, cachedData);

                oRet = sourceResponse;
            }
            else
            {
                // an error occurred while trying to retrieve from the source
                // return the default for the generic type
                oRet = default;
            }
        }
        else
        {
            // all good, use cached data
            oRet = cachedData.Data;
        }

        return oRet;
    }

    public T? Get<T>(string key, Func<T> dataFetchFunction, TimeSpan lifeSpan)
    {
        T? oRet;

        var cachedData = _cache.Get<CacheItem<T>>(key);
        if (cachedData == null || cachedData.IsExpired())
        {
            var sourceResponse = dataFetchFunction();
            if (sourceResponse != null)
            {
                // store in in-memory cache
                cachedData = new CacheItem<T>
                {
                    Created = DateTime.Now,
                    LifeSpan = lifeSpan,
                    Data = sourceResponse
                };
                _cache.Set(key, cachedData);

                oRet = sourceResponse;
            }
            else
            {
                // an error occurred while trying to retrieve from the source
                // return the default for the generic type
                oRet = default;
            }
        }
        else
        {
            // all good, use cached data
            oRet = cachedData.Data;
        }

        return oRet;
    }
}