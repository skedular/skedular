using HealthChecks.NpgSql;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Enterprise.Shared.Database.PostgreSql;

public static class HealthCheckExtensions
{
    /// <summary>
    ///     Checks for SQL Server connectivity
    ///     This binds to the "services" tag that outputs to /health/readiness
    /// </summary>
    public static IHealthChecksBuilder AddDatabaseHealthCheck(
        this IServiceCollection services,
        NpgsqlDataSource npgsqlDataSource,
        string? healthCheckName,
        int healthCheckTimeOutInSeconds = 5) =>
        services
            .AddHealthChecks()
            .AddNpgSql(
                new NpgSqlHealthCheckOptions(npgsqlDataSource),
                tags: [HealthCheck.Constants.ReadinessTag],
                timeout: TimeSpan.FromSeconds(healthCheckTimeOutInSeconds),
                name: healthCheckName
            );
}
