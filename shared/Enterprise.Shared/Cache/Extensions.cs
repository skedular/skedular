using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Enterprise.Shared.Cache;

public static class Extensions
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration, string connectionName)
    {
        var connectionString = configuration.GetConnectionString(connectionName);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(connectionString));

        return services.AddStackExchangeRedisCache(options => options.Configuration = connectionString);
    }
}
