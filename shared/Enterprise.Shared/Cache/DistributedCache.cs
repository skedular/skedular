using System.Text.Json;
using Enterprise.Shared.Configurations;
using StackExchange.Redis;

namespace Enterprise.Shared.Cache;

public interface IDistributedCache
{
    Task<bool> ExistsAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry);
    Task<(bool, T?)> GetAsync<T>(string key);
}

public class DistributedCache(ApplicationConfiguration applicationConfiguration, ConnectionMultiplexer connectionMultiplexer) : IDistributedCache
{
    private readonly IDatabase _database = connectionMultiplexer.GetDatabase();

    public async Task<bool> ExistsAsync<T>(string key) => await _database.KeyExistsAsync(AddPrefix(key));

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry) =>
        await _database.StringSetAsync(AddPrefix(key), JsonSerializer.Serialize(value), expiry);

    public async Task<(bool, T?)> GetAsync<T>(string key)
    {
        var data = await _database.StringGetAsync(AddPrefix(key));
        return data.IsNull ? (false, default) : (true, JsonSerializer.Deserialize<T>(data!));
    }

    private string AddPrefix(string key) => $"{applicationConfiguration.Environment}:{key}";
}
