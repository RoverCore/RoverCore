namespace RoverCore.Boilerplate.Infrastructure.Common.Cache.Services;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, Func<Task<T>> dataFetchFunction, TimeSpan lifeSpan);
    T? Get<T>(string key, Func<T> dataFetchFunction, TimeSpan lifeSpan);
}