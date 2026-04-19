using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Database.SqlServer;

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
            string connectionString,
            string? healthCheckName,
            int healthCheckTimeOutInSeconds = 5) =>
            services
                .AddHealthChecks()
                .AddSqlServer(
                    connectionString,
                    tags: [HealthCheck.Constants.ReadinessTag],
                    timeout: TimeSpan.FromSeconds(healthCheckTimeOutInSeconds),
                    name: healthCheckName
                );
    }
}
