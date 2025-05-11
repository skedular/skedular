using System.Reflection;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Quartz.Impl.AdoJobStore.Common;
using StackExchange.Redis;

namespace Enterprise.Shared.Database;

public static class ServiceExtensions
{
    public static IServiceCollection WithPooledDbContextFactory<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string connectionName)
        where TDbContext : DbContext
    {
        var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
        var dataSource = GetDatasource(services, configuration, true, connectionName);

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
                    sqlOptions =>
                    {
                        sqlOptions.UseQuerySplittingBehavior(applicationConfiguration?.QuerySplittingBehavior ?? QuerySplittingBehavior.SplitQuery);
                        sqlOptions.MigrationsAssembly(typeof(TDbContext).GetTypeInfo().Assembly.GetName().Name);
                    })
                .ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning));
        });
    }

    public static IServiceCollection WithDbContextFactory<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string connectionName)
        where TDbContext : DbContext
    {
        var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
        var dataSource = GetDatasource(services, configuration, false, connectionName);

        return services.AddDbContextFactory<TDbContext>(options =>
        {
            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
            }

            options
                .UseNpgsql(
                    dataSource,
                    sqlOptions =>
                    {
                        sqlOptions.UseQuerySplittingBehavior(applicationConfiguration?.QuerySplittingBehavior ?? QuerySplittingBehavior.SplitQuery);
                        sqlOptions.MigrationsAssembly(typeof(TDbContext).GetTypeInfo().Assembly.GetName().Name);
                    })
                .ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning));
        });
    }

    public static IServiceCollection WithQuartzNpgsqlDbProvider(this IServiceCollection services, NpgsqlDataSource dataSource) =>
        services.AddSingleton<IDbProvider>(new QuartzNpgsqlDbProvider(dataSource));

    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration, string name)
    {
        var connectionString = configuration.GetConnectionString(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        return services
            .AddSingleton(_ => ConnectionMultiplexer.Connect(connectionString))
            .AddScoped<IDistributedCache, DistributedCache>();
    }

    private static NpgsqlDataSource GetDatasource(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isPooled,
        string connectionName)
    {
        services
            .AddSingleton(new CustomDbContextOptions { IsPooled = isPooled })
            .AddSingleton<IDbTransactionBuilder, DbTransactionBuilder>()
            .AddSingleton<IDatabaseMigrationService, DatabaseMigrationService>();

        var connectionString = configuration.GetConnectionString(connectionName);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        var dataSource = connectionString.BuildDataSource();

        services.AddDatabaseHealthCheck(dataSource);

        return dataSource;
    }
}
