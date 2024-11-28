using Application.Services.CacheService;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Infrastructure.Services.CacheService;

[Obsolete("RedisService is deprecated. Please use RedisMultiplexerService instead.")]
public sealed class RedisService(IDistributedCache distributedCache) : ICacheService
{
    private readonly IDistributedCache _distributedCache = distributedCache;

    public Task ClearAsync()
    {
        if (_distributedCache is IConnectionMultiplexer multiplexer)
        {
            var endpoints = multiplexer.GetEndPoints();
            var server = multiplexer.GetServer(endpoints.First());
            return server.FlushDatabaseAsync();
        }
        else
        {
            throw new NotSupportedException("Clearing all keys is not supported with the current IDistributedCache implementation.");
        }
    }

    public async ValueTask<T?> GetStringAsync<T>(string key)
    {
        string? value = await _distributedCache.GetStringAsync(key);
        return value is null ? default : JsonConvert.DeserializeObject<T>(value);
    }

    public Task RemoveAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task SetStringAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        return Task.FromResult(_distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(value),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry
            })
        );
    }

    public Task UpdateAsync<T>(string key, T value)
    {
        throw new NotImplementedException();
    }
}