using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Enterprise.Shared.Cache;

public static class Extensions
{
    /// <summary>
    ///     Registers a StackExchange.Redis <see cref="IConnectionMultiplexer" /> singleton and the
    ///     distributed cache (<c>AddStackExchangeRedisCache</c>) using the named connection string.
    ///     Must be called before <c>AddHybridCaching()</c> and before <c>AddGraphql()</c> when Redis
    ///     subscriptions are enabled.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">Application configuration (reads <c>ConnectionStrings:{connectionName}</c>).</param>
    /// <param name="connectionName">The connection-string key to look up in <c>ConnectionStrings</c>.</param>
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration, string connectionName)
    {
        var connectionString = configuration.GetConnectionString(connectionName);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(connectionString));

        return services.AddStackExchangeRedisCache(options => options.Configuration = connectionString);
    }
}
