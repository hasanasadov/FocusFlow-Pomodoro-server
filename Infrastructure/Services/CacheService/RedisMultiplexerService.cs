using Application.Services.CacheService;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Infrastructure.Services.CacheService;

public sealed class RedisMultiplexerService : ICacheService
{
    private readonly IDatabase _database;
    private readonly ConnectionMultiplexer _connectionMultiplexer;

    public RedisMultiplexerService(IConfiguration configuration)
    {
        var redisHost = Environment.GetEnvironmentVariable("REDIS_HOST") ?? "localhost";
        var redisPort = Environment.GetEnvironmentVariable("REDIS_PORT") ?? "6379";
        var redisPassword = Environment.GetEnvironmentVariable("REDIS_PASSWORD") ?? "";

        string redisConfiguration = $"{redisHost}:{redisPort}";

        if (!string.IsNullOrEmpty(redisPassword))
        {
            redisConfiguration += $",password={redisPassword}";
        }
        if (string.IsNullOrEmpty(redisConfiguration))
        {
            redisConfiguration = "localhost:6379";
        }
        _connectionMultiplexer = ConnectionMultiplexer.Connect(redisConfiguration);
        _database = _connectionMultiplexer.GetDatabase();
    }

    public async ValueTask<T?> GetStringAsync<T>(string key)
    {
        string? value = await _database.StringGetAsync(key);
        if (value is null)
        {
            return default;
        }
        return JsonConvert.DeserializeObject<T>(value);
    }

    public Task SetStringAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        return _database.StringSetAsync(key, JsonConvert.SerializeObject(value), expiry);
    }

    public async Task UpdateAsync<T>(string key, T value)
    {
        string? valueString = await _database.StringGetAsync(key);
        if (valueString is null || value is null)
        {
            return;
        }
        await _database.StringSetAsync(key, JsonConvert.SerializeObject(value));
    }

    public Task RemoveAsync(string key)
    {
        return _database.KeyDeleteAsync(key);
    }
    public Task ClearAsync()
    {
        return _database.ExecuteAsync("FLUSHDB");
    }
}