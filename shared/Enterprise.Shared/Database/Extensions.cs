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
    public static IServiceCollection WithPooledDbContextFactory<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string connectionName)
        where TDbContext : DbContext
    {
        var connectionString = configuration.GetConnectionString(connectionName);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return services;
        }

        var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
        var dataSource = GetDatasource(services, true, connectionString);

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
        var connectionString = configuration.GetConnectionString(connectionName);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return services;
        }

        var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
        var dataSource = GetDatasource(services, false, connectionString);

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

    private static NpgsqlDataSource GetDatasource(this IServiceCollection services, bool isPooled, string connectionString)
    {
        services
            .AddSingleton(new CustomDbContextOptions { IsPooled = isPooled })
            .AddSingleton<IDbTransactionBuilder, DbTransactionBuilder>()
            .AddSingleton<IDatabaseMigrationService, DatabaseMigrationService>();

        var dataSource = connectionString.BuildDataSource();

        services.AddDatabaseHealthCheck(dataSource);

        return dataSource;
    }
}
