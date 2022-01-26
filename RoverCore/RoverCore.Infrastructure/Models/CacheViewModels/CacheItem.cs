namespace RoverCore.Infrastructure.Models.CacheViewModels;

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