using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Enterprise.Shared.Cache;

public static class Extensions
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration, string connectionName)
    {
        var connectionString = configuration.GetConnectionString(connectionName);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return services;
        }

        return services
            .AddSingleton(_ => ConnectionMultiplexer.Connect(connectionString))
            .AddScoped<IDistributedCache, DistributedCache>();
    }
}
