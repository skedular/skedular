using HealthChecks.NpgSql;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Enterprise.Shared.Database.PostgreSql;

public static class HealthCheckExtensions
{
    /// <param name="services">The service collection to configure.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Checks for SQL Server connectivity
        ///     This binds to the "services" tag that outputs to /health/readiness
        /// </summary>
        public IHealthChecksBuilder AddDatabaseHealthCheck(
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
}
