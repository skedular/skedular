using System.Reflection;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace Enterprise.Shared.Database;

public static class Extensions
{
    public static IServiceCollection WithPooledDbContext<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string connectionName,
        bool isPostgisEnabled = false)
        where TDbContext : DbContext
    {
        var connectionString = configuration.GetConnectionString(connectionName);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
        var dataSource = GetDatasource(services, true, isPostgisEnabled, connectionString);

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

    public static IServiceCollection WithDbContext<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string connectionName,
        bool isPostgisEnabled = false)
        where TDbContext : DbContext
    {
        var connectionString = configuration.GetConnectionString(connectionName);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
        var dataSource = GetDatasource(services, false, isPostgisEnabled, connectionString);

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

    public static IServiceCollection WithPooledDbContextFactory<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string connectionName,
        bool isPostgisEnabled = false)
        where TDbContext : DbContext
    {
        var connectionString = configuration.GetConnectionString(connectionName);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
        var dataSource = GetDatasource(services, true, isPostgisEnabled, connectionString);

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

    public static IServiceCollection WithDbContextFactory<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string connectionName,
        bool isPostgisEnabled = false)
        where TDbContext : DbContext
    {
        var connectionString = configuration.GetConnectionString(connectionName);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
        var dataSource = GetDatasource(services, false, isPostgisEnabled, connectionString);

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

    private static NpgsqlDataSource GetDatasource(this IServiceCollection services, bool isPooled, bool isPostgisEnabled, string connectionString)
    {
        services
            .AddSingleton(new CustomDbContextOptions { IsPooled = isPooled, IsPostgisEnabled = isPostgisEnabled })
            .AddSingleton<IDbTransactionBuilder, DbTransactionBuilder>()
            .AddSingleton<IDatabaseMigrationService, DatabaseMigrationService>();

        var dataSource = connectionString.BuildDataSource();

        services.AddDatabaseHealthCheck(dataSource);

        return dataSource;
    }
}
