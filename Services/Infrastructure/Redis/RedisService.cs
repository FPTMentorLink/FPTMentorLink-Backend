using System.Text.Json;
using Services.Interfaces;
using StackExchange.Redis;

namespace Services.Infrastructure.Redis;

public class RedisService(IConnectionMultiplexer redis) : IRedisService
{
    private readonly IDatabase _redis = redis.GetDatabase();

    public async Task<string?> GetCacheAsync(string key)
    {
        var data = await _redis.StringGetAsync(key);
        return data.IsNullOrEmpty ? null : data.ToString();
    }

    public async Task SetCacheAsync(string key, object value, TimeSpan timeToLive, bool isSerialized = true)
    {
        if (!isSerialized)
        {
            await _redis.StringSetAsync(key, value.ToString(), timeToLive);
            return;
        }

        var jsonOption = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var data = JsonSerializer.Serialize(value, jsonOption);
        await _redis.StringSetAsync(key, data, timeToLive);
    }

    public Task DeleteCacheAsync(string key)
    {
        return _redis.KeyDeleteAsync(key);
    }

    public Task DeleteCacheWithPatternAsync(string pattern)
    {
        var server = _redis.Multiplexer.GetServer(_redis.Multiplexer.GetEndPoints().First());
        var keys = server.Keys(0, pattern).ToArray();
        return _redis.KeyDeleteAsync(keys);
    }
}