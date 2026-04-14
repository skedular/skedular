using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database.SqlServer.Interceptors;
using Enterprise.Shared.Telemetry.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Trace;

namespace Enterprise.Shared.Database.SqlServer;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection WithPooledSqlServerDbContext<TDbContext>(IConfiguration configuration,
            IHostEnvironment environment,
            string connectionName,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(connectionName);
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            return services.WithPooledSqlServerDbContextWithConnectionString<TDbContext>(
                configuration,
                environment,
                connectionString,
                isPostgisEnabled,
                healthCheckName);
        }

        public IServiceCollection WithPooledSqlServerDbContextWithConnectionString<TDbContext>(IConfiguration configuration,
            IHostEnvironment environment,
            string connectionString,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
            var validatedConnectionString = services.GetDatasource<TDbContext>(
                true,
                isPostgisEnabled,
                connectionString,
                healthCheckName,
                configuration);

            return services.AddDbContextPool<TDbContext>(options =>
            {
                if (environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                }

                options
                    .AddInterceptors(new SelectForUpdateCommandInterceptor())
                    .UseSqlServer(
                        validatedConnectionString,
                        sqlServerOptions =>
                        {
                            sqlServerOptions.UseQuerySplittingBehavior(applicationConfiguration?.QuerySplittingBehavior ??
                                                                       QuerySplittingBehavior.SplitQuery);
                            sqlServerOptions.MigrationsAssembly(typeof(TDbContext).GetTypeInfo().Assembly.GetName().Name);
                        })
                    .ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning));
            });
        }

        public IServiceCollection WithSqlServerDbContext<TDbContext>(IConfiguration configuration,
            IHostEnvironment environment,
            string connectionName,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(connectionName);
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            return services.WithSqlServerDbContextWithConnectionString<TDbContext>(
                configuration,
                environment,
                connectionString,
                isPostgisEnabled,
                healthCheckName);
        }

        public IServiceCollection WithSqlServerDbContextWithConnectionString<TDbContext>(IConfiguration configuration,
            IHostEnvironment environment,
            string connectionString,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
            var validatedConnectionString = services.GetDatasource<TDbContext>(
                false,
                isPostgisEnabled,
                connectionString,
                healthCheckName,
                configuration);

            return services.AddDbContext<TDbContext>(options =>
            {
                if (environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                }

                options
                    .UseSqlServer(
                        validatedConnectionString,
                        sqlServerOptions =>
                        {
                            sqlServerOptions.UseQuerySplittingBehavior(applicationConfiguration?.QuerySplittingBehavior ??
                                                                       QuerySplittingBehavior.SplitQuery);
                            sqlServerOptions.MigrationsAssembly(typeof(TDbContext).GetTypeInfo().Assembly.GetName().Name);
                        })
                    .ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning));
            });
        }

        public IServiceCollection WithPooledSqlServerDbContextFactory<TDbContext>(IConfiguration configuration,
            IHostEnvironment environment,
            string connectionName,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(connectionName);
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            return services.WithPooledSqlServerDbContextFactoryWithConnectionString<TDbContext>(
                configuration,
                environment,
                connectionString,
                isPostgisEnabled,
                healthCheckName);
        }

        public IServiceCollection WithPooledSqlServerDbContextFactoryWithConnectionString<TDbContext>(IConfiguration configuration,
            IHostEnvironment environment,
            string connectionString,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
            var validatedConnectionString = services.GetDatasource<TDbContext>(
                true,
                isPostgisEnabled,
                connectionString,
                healthCheckName,
                configuration);

            return services.AddPooledDbContextFactory<TDbContext>(options =>
            {
                if (environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                }

                options
                    .AddInterceptors(new SelectForUpdateCommandInterceptor())
                    .UseSqlServer(
                        validatedConnectionString,
                        sqlServerOptions =>
                        {
                            sqlServerOptions.UseQuerySplittingBehavior(applicationConfiguration?.QuerySplittingBehavior ??
                                                                       QuerySplittingBehavior.SplitQuery);
                            sqlServerOptions.MigrationsAssembly(typeof(TDbContext).GetTypeInfo().Assembly.GetName().Name);
                        })
                    .ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning));
            });
        }

        public IServiceCollection WithSqlServerDbContextFactory<TDbContext>(IConfiguration configuration,
            IHostEnvironment environment,
            string connectionName,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(connectionName);
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            return services.WithSqlServerDbContextFactoryWithConnectionString<TDbContext>(
                configuration,
                environment,
                connectionString,
                isPostgisEnabled,
                healthCheckName);
        }

        public IServiceCollection WithSqlServerDbContextFactoryWithConnectionString<TDbContext>(IConfiguration configuration,
            IHostEnvironment environment,
            string connectionString,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
            var validatedConnectionString = services.GetDatasource<TDbContext>(
                false,
                isPostgisEnabled,
                connectionString,
                healthCheckName,
                configuration);

            return services.AddDbContextFactory<TDbContext>(options =>
            {
                if (environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                }

                options
                    .UseSqlServer(
                        validatedConnectionString,
                        sqlServerOptions =>
                        {
                            sqlServerOptions.UseQuerySplittingBehavior(applicationConfiguration?.QuerySplittingBehavior ??
                                                                       QuerySplittingBehavior.SplitQuery);
                            sqlServerOptions.MigrationsAssembly(typeof(TDbContext).GetTypeInfo().Assembly.GetName().Name);
                        })
                    .ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning));
            });
        }

        private string GetDatasource<TDbContext>(
            bool isPooled,
            bool isPostgisEnabled,
            string connectionString,
            string? healthCheckName,
            IConfiguration configuration)
            where TDbContext : DbContext
        {
            services
                .AddSingleton(new CustomDbContextOptions<TDbContext> { IsPooled = isPooled, IsPostgisEnabled = isPostgisEnabled })
                .AddSingleton<IDbTransactionBuilder, DbTransactionBuilder>()
                .AddSingleton<IDatabaseMigrationService, DatabaseMigrationService>();

            var validatedConnectionString = connectionString.BuildConnectionString();

            services.AddDatabaseHealthCheck(validatedConnectionString, healthCheckName);
            services.AddTelemetry(configuration);

            return validatedConnectionString;
        }

        private void AddTelemetry(IConfiguration configuration)
        {
            if (services.Any(item => item.ServiceType == typeof(SqlServerTelemetryRegistrationMarker)))
            {
                return;
            }

            services.TryAddSingleton<SqlServerTelemetryRegistrationMarker>();
            var openTelemetryConfiguration = configuration.GetSection(OpenTelemetryConfiguration.Key).Get<OpenTelemetryConfiguration>();

            services.AddOpenTelemetry()
                .WithTracing(tracing =>
                {
                    tracing.AddSqlClientInstrumentation();

                    if (openTelemetryConfiguration?.EntityFrameworkEnabled == true)
                    {
                        tracing.AddEntityFrameworkCoreInstrumentation(options =>
                        {
                            options.EnrichWithIDbCommand = delegate(Activity activity, IDbCommand command)
                            {
                                activity.DisplayName = $"{command.CommandType} main";
                                activity.SetTag("db.type", command.CommandType);
                                activity.SetTag("db.text", command.CommandText);
                                activity.SetTag(
                                    "db.parameters",
                                    string.Join(",",
                                        command.Parameters.OfType<DbParameter>()
                                            .Select(parameter => $"{parameter.ParameterName}={parameter.Value}")));
                            };
                        });
                    }
                });
        }
    }

    private sealed class SqlServerTelemetryRegistrationMarker;
}
