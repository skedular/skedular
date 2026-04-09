using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database.Postgres.Interceptors;
using Enterprise.Shared.Telemetry.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Npgsql;
using OpenTelemetry.Trace;

namespace Enterprise.Shared.Database.Postgres;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection WithPooledDbContext<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionName,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(connectionName);
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            return services.WithPooledDbContextWithConnectionString<TDbContext>(
                configuration,
                environment,
                connectionString,
                isPostgisEnabled,
                healthCheckName);
        }

        public IServiceCollection WithPooledDbContextWithConnectionString<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionString,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
            var dataSource = services.GetDatasource(true, isPostgisEnabled, connectionString, healthCheckName, configuration);

            return services.AddDbContextPool<TDbContext>(options =>
            {
                if (environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                }

                options
                    .AddInterceptors(new SelectForUpdateCommandInterceptor())
                    .UseNpgsql(
                        dataSource,
                        npgsqlOptions =>
                        {
                            npgsqlOptions.UseQuerySplittingBehavior(applicationConfiguration?.QuerySplittingBehavior ??
                                                                    QuerySplittingBehavior.SplitQuery);
                            npgsqlOptions.MigrationsAssembly(typeof(TDbContext).GetTypeInfo().Assembly.GetName().Name);

                            if (isPostgisEnabled)
                            {
                                npgsqlOptions.UseNetTopologySuite();
                            }
                        })
                    .ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning));
            });
        }

        public IServiceCollection WithDbContext<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionName,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(connectionName);
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            return services.WithDbContextWithConnectionString<TDbContext>(
                configuration,
                environment,
                connectionString,
                isPostgisEnabled,
                healthCheckName);
        }

        public IServiceCollection WithDbContextWithConnectionString<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionString,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
            var dataSource = services.GetDatasource(false, isPostgisEnabled, connectionString, healthCheckName, configuration);

            return services.AddDbContext<TDbContext>(options =>
            {
                if (environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                }

                options
                    .UseNpgsql(
                        dataSource,
                        npgsqlOptions =>
                        {
                            npgsqlOptions.UseQuerySplittingBehavior(applicationConfiguration?.QuerySplittingBehavior ??
                                                                    QuerySplittingBehavior.SplitQuery);
                            npgsqlOptions.MigrationsAssembly(typeof(TDbContext).GetTypeInfo().Assembly.GetName().Name);

                            if (isPostgisEnabled)
                            {
                                npgsqlOptions.UseNetTopologySuite();
                            }
                        })
                    .ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning));
            });
        }

        public IServiceCollection WithPooledDbContextFactory<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionName,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(connectionName);
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            return services.WithPooledDbContextFactoryWithConnectionString<TDbContext>(
                configuration,
                environment,
                connectionString,
                isPostgisEnabled,
                healthCheckName);
        }

        public IServiceCollection WithPooledDbContextFactoryWithConnectionString<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionString,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
            var dataSource = services.GetDatasource(true, isPostgisEnabled, connectionString, healthCheckName, configuration);

            return services.AddPooledDbContextFactory<TDbContext>(options =>
            {
                if (environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                }

                options
                    .AddInterceptors(new SelectForUpdateCommandInterceptor())
                    .UseNpgsql(
                        dataSource,
                        npgsqlOptions =>
                        {
                            npgsqlOptions.UseQuerySplittingBehavior(applicationConfiguration?.QuerySplittingBehavior ??
                                                                    QuerySplittingBehavior.SplitQuery);
                            npgsqlOptions.MigrationsAssembly(typeof(TDbContext).GetTypeInfo().Assembly.GetName().Name);

                            if (isPostgisEnabled)
                            {
                                npgsqlOptions.UseNetTopologySuite();
                            }
                        })
                    .ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning));
            });
        }

        public IServiceCollection WithDbContextFactory<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionName,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(connectionName);
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            return services.WithDbContextFactoryWithConnectionString<TDbContext>(
                configuration,
                environment,
                connectionString,
                isPostgisEnabled,
                healthCheckName);
        }

        public IServiceCollection WithDbContextFactoryWithConnectionString<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionString,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
            var dataSource = services.GetDatasource(false, isPostgisEnabled, connectionString, healthCheckName, configuration);

            return services.AddDbContextFactory<TDbContext>(options =>
            {
                if (environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                }

                options
                    .UseNpgsql(
                        dataSource,
                        npgsqlOptions =>
                        {
                            npgsqlOptions.UseQuerySplittingBehavior(applicationConfiguration?.QuerySplittingBehavior ??
                                                                    QuerySplittingBehavior.SplitQuery);
                            npgsqlOptions.MigrationsAssembly(typeof(TDbContext).GetTypeInfo().Assembly.GetName().Name);

                            if (isPostgisEnabled)
                            {
                                npgsqlOptions.UseNetTopologySuite();
                            }
                        })
                    .ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning));
            });
        }

        private NpgsqlDataSource GetDatasource(bool isPooled, bool isPostgisEnabled, string connectionString, string? healthCheckName,
            IConfiguration configuration)
        {
            services
                .AddSingleton(new CustomDbContextOptions { IsPooled = isPooled, IsPostgisEnabled = isPostgisEnabled })
                .AddSingleton<IDbTransactionBuilder, DbTransactionBuilder>()
                .AddSingleton<IDatabaseMigrationService, DatabaseMigrationService>();

            var dataSource = connectionString.BuildDataSource(isPostgisEnabled);

            services.AddDatabaseHealthCheck(dataSource, healthCheckName);
            services.AddPostgresTelemetry(configuration);

            return dataSource;
        }

        private void AddPostgresTelemetry(IConfiguration configuration)
        {
            if (services.Any(item => item.ServiceType == typeof(PostgresTelemetryRegistrationMarker)))
            {
                return;
            }

            services.TryAddSingleton<PostgresTelemetryRegistrationMarker>();
            var openTelemetryConfiguration = configuration.GetSection(OpenTelemetryConfiguration.Key).Get<OpenTelemetryConfiguration>();

            services.AddOpenTelemetry()
                .WithMetrics(metrics => metrics.AddNpgsqlInstrumentation())
                .WithTracing(tracing =>
                {
                    tracing.AddNpgsql();

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

    private sealed class PostgresTelemetryRegistrationMarker;
}
