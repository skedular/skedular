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
    public static IServiceCollection WithPooledDbContext<TDbContext>(
        this IServiceCollection services,
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

    public static IServiceCollection WithPooledDbContextWithConnectionString<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string connectionString,
        bool isPostgisEnabled = false,
        string? healthCheckName = null)
        where TDbContext : DbContext
    {
        var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
        var validatedConnectionString = services.GetDatasource(true, isPostgisEnabled, connectionString, healthCheckName, configuration);

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

    public static IServiceCollection WithDbContext<TDbContext>(
        this IServiceCollection services,
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

    public static IServiceCollection WithDbContextWithConnectionString<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string connectionString,
        bool isPostgisEnabled = false,
        string? healthCheckName = null)
        where TDbContext : DbContext
    {
        var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
        var validatedConnectionString = services.GetDatasource(false, isPostgisEnabled, connectionString, healthCheckName, configuration);

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

    public static IServiceCollection WithPooledDbContextFactory<TDbContext>(
        this IServiceCollection services,
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

    public static IServiceCollection WithPooledDbContextFactoryWithConnectionString<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string connectionString,
        bool isPostgisEnabled = false,
        string? healthCheckName = null)
        where TDbContext : DbContext
    {
        var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
        var validatedConnectionString = services.GetDatasource(true, isPostgisEnabled, connectionString, healthCheckName, configuration);

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

    public static IServiceCollection WithDbContextFactory<TDbContext>(
        this IServiceCollection services,
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

    public static IServiceCollection WithDbContextFactoryWithConnectionString<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string connectionString,
        bool isPostgisEnabled = false,
        string? healthCheckName = null)
        where TDbContext : DbContext
    {
        var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
        var validatedConnectionString = services.GetDatasource(false, isPostgisEnabled, connectionString, healthCheckName, configuration);

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

    private static string GetDatasource(
        this IServiceCollection services,
        bool isPooled,
        bool isPostgisEnabled,
        string connectionString,
        string? healthCheckName,
        IConfiguration configuration)
    {
        services
            .AddSingleton(new CustomDbContextOptions { IsPooled = isPooled, IsPostgisEnabled = isPostgisEnabled })
            .AddSingleton<IDbTransactionBuilder, DbTransactionBuilder>()
            .AddSingleton<IDatabaseMigrationService, DatabaseMigrationService>();

        var validatedConnectionString = connectionString.BuildConnectionString();

        services.AddDatabaseHealthCheck(validatedConnectionString, healthCheckName);
        services.AddSqlServerTelemetry(configuration);

        return validatedConnectionString;
    }

    private static void AddSqlServerTelemetry(this IServiceCollection services, IConfiguration configuration)
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

    private sealed class SqlServerTelemetryRegistrationMarker;
}
