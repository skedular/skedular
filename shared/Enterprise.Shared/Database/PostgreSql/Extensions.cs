using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database.PostgreSql.Interceptors;
using Enterprise.Shared.Telemetry.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Npgsql;
using OpenTelemetry.Trace;

namespace Enterprise.Shared.Database.PostgreSql;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection WithPooledPostgreSqlDbContext<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionName,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(connectionName);
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            return services.WithPooledPostgreSqlDbContextWithConnectionString<TDbContext>(
                configuration,
                environment,
                connectionString,
                isPostgisEnabled,
                healthCheckName);
        }

        public IServiceCollection WithPooledPostgreSqlDbContextWithConnectionString<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionString,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
            var dataSource = services.GetDatasource<TDbContext>(true, isPostgisEnabled, connectionString, healthCheckName, configuration);

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

        public IServiceCollection WithPostgreSqlDbContext<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionName,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(connectionName);
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            return services.WithPostgreSqlDbContextWithConnectionString<TDbContext>(
                configuration,
                environment,
                connectionString,
                isPostgisEnabled,
                healthCheckName);
        }

        public IServiceCollection WithPostgreSqlDbContextWithConnectionString<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionString,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
            var dataSource = services.GetDatasource<TDbContext>(false, isPostgisEnabled, connectionString, healthCheckName, configuration);

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

        public IServiceCollection WithPooledPostgreSqlDbContextFactory<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionName,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(connectionName);
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            return services.WithPooledPostgreSqlDbContextFactoryWithConnectionString<TDbContext>(
                configuration,
                environment,
                connectionString,
                isPostgisEnabled,
                healthCheckName);
        }

        public IServiceCollection WithPooledPostgreSqlDbContextFactoryWithConnectionString<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionString,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
            var dataSource = services.GetDatasource<TDbContext>(true, isPostgisEnabled, connectionString, healthCheckName, configuration);

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

        public IServiceCollection WithPostgreSqlDbContextFactory<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionName,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(connectionName);
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            return services.WithPostgreSqlDbContextFactoryWithConnectionString<TDbContext>(
                configuration,
                environment,
                connectionString,
                isPostgisEnabled,
                healthCheckName);
        }

        public IServiceCollection WithPostgreSqlDbContextFactoryWithConnectionString<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionString,
            bool isPostgisEnabled = false,
            string? healthCheckName = null)
            where TDbContext : DbContext
        {
            var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
            var dataSource = services.GetDatasource<TDbContext>(false, isPostgisEnabled, connectionString, healthCheckName, configuration);

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

        private NpgsqlDataSource GetDatasource<TDbContext>(
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

            var dataSource = connectionString.BuildDataSource(isPostgisEnabled);

            services.AddDatabaseHealthCheck(dataSource, healthCheckName);
            services.AddTelemetry(configuration);

            return dataSource;
        }

        private void AddTelemetry(IConfiguration configuration)
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
