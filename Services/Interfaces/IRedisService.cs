namespace Services.Interfaces;

public interface IRedisService
{
    Task<string?> GetCacheAsync(string key);
    Task SetCacheAsync(string key, object value, TimeSpan timeToLive, bool isSerialized = true);
    Task DeleteCacheAsync(string key);
    Task DeleteCacheWithPatternAsync(string pattern);
}