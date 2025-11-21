using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Cache;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddRedis(IConfiguration configuration, string connectionName)
        {
            var connectionString = configuration.GetConnectionString(connectionName);
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            return services
                .AddStackExchangeRedisCache(options => options.Configuration = connectionString);
        }
    }
}
