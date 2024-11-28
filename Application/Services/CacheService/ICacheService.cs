namespace Application.Services.CacheService;

public interface ICacheService
{
    Task SetStringAsync<T>(string key, T value, TimeSpan? expiry = null);
    ValueTask<T?> GetStringAsync<T>(string key);
    Task ClearAsync();
    Task UpdateAsync<T>(string key, T value);
    Task RemoveAsync(string key);
}