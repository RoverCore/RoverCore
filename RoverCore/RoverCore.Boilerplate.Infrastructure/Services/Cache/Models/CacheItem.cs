namespace RoverCore.Boilerplate.Infrastructure.Services.Cache.Models;

#pragma warning disable CS8618
public class CacheItem<T>
{
    public T Data { get; set; }

    public DateTime Created { get; set; }

    public TimeSpan LifeSpan { get; set; }

    public bool IsExpired()
    {
        return Created.Add(LifeSpan) < DateTime.Now;
    }
}
#pragma warning restore CS8618