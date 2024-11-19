using Enterprise.Shared.HealthCheck;
using HealthChecks.NpgSql;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Enterprise.Shared.Database;

public static class HealthCheckExtensions
{
    /// <summary>
    ///     Checks for Sql Server connectivity
    ///     This binds to the "services" tag that outputs to /health/readiness
    /// </summary>
    public static IHealthChecksBuilder AddDatabaseHealthCheck(
        this IServiceCollection services,
        NpgsqlDataSource npgsqlDataSource,
        int healthCheckTimeOutInSeconds = 5) =>
        services
            .AddHealthChecks()
            .AddNpgSql(
                new NpgSqlHealthCheckOptions(npgsqlDataSource),
                tags: [HealthCheckTags.Readiness],
                timeout: TimeSpan.FromSeconds(healthCheckTimeOutInSeconds)
            );
}
