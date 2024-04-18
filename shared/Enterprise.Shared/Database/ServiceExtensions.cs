using System.Reflection;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Quartz.Impl.AdoJobStore.Common;

namespace Enterprise.Shared.Database;

public static class ServiceExtensions
{
    public static void AddDatabaseHealthCheck(this DatabaseSetup databaseSetup) =>
        databaseSetup.ServiceCollection.AddDatabaseHealthCheck(databaseSetup.NpgsqlDataSource);

    public static DatabaseSetup AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isPooled,
        string name = ConnectionStringKeys.Default)
    {
        services
            .AddSingleton(new CustomDbContextOptions { IsPooled = isPooled })
            .AddSingleton<IDbTransactionBuilder, DbTransactionBuilder>();

        var postgresSqlConfigurationOptions =
            configuration
                .GetSection(PostgresConfigurationOptions.Key)
                .Get<PostgresConfigurationOptions>() ?? new PostgresConfigurationOptions();

        var applicationConfiguration = configuration
            .GetSection(ApplicationConfiguration.Key)
            .Get<ApplicationConfiguration>();
        ArgumentNullException.ThrowIfNull(applicationConfiguration);

        var connectionString = configuration.GetConnectionString(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        var npgsqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);
        if (!string.IsNullOrWhiteSpace(applicationConfiguration.Environment))
        {
            npgsqlConnectionStringBuilder.Database =
                $"{applicationConfiguration.Environment}.{npgsqlConnectionStringBuilder.Database}";
        }

        if (!string.IsNullOrWhiteSpace(postgresSqlConfigurationOptions.Username))
        {
            npgsqlConnectionStringBuilder.Username = postgresSqlConfigurationOptions.Username;
        }

        if (!string.IsNullOrWhiteSpace(postgresSqlConfigurationOptions.Password))
        {
            npgsqlConnectionStringBuilder.Password = postgresSqlConfigurationOptions.Password;
        }

        postgresSqlConfigurationOptions.DefaultConnection = npgsqlConnectionStringBuilder.ConnectionString;

        var dataSource = postgresSqlConfigurationOptions.BuildDataSource();

        return new DatabaseSetup(services, dataSource);
    }

    public static DatabaseSetupContext<TDbContext> WithPooledDbContextFactory<TDbContext>(
        this DatabaseSetup databaseSetup,
        Migration option,
        IHostEnvironment environment)
        where TDbContext : DbContext
    {
        databaseSetup.ServiceCollection
            .AddPooledDbContextFactory<TDbContext>(options =>
            {
                if (environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                }

                options.AddInterceptors(new SelectForUpdateCommandInterceptor());

                options.UseNpgsql(databaseSetup.NpgsqlDataSource, sqlOptions =>
                {
                    sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

                    if (option != Migration.SetAssembly)
                    {
                        return;
                    }

                    sqlOptions.MigrationsAssembly(typeof(TDbContext).GetTypeInfo().Assembly.GetName().Name);
                });
            });

        return new DatabaseSetupContext<TDbContext>(databaseSetup);
    }

    public static DatabaseSetupContext<TDbContext> WithDbContextFactory<TDbContext>(
        this DatabaseSetup databaseSetup,
        Migration option,
        IHostEnvironment environment)
        where TDbContext : DbContext
    {
        databaseSetup.ServiceCollection
            .AddDbContextFactory<TDbContext>(options =>
            {
                if (environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                }

                options.UseNpgsql(databaseSetup.NpgsqlDataSource, sqlOptions =>
                {
                    sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

                    if (option != Migration.SetAssembly)
                    {
                        return;
                    }

                    sqlOptions.MigrationsAssembly(typeof(TDbContext).GetTypeInfo().Assembly.GetName().Name);
                });
            });

        return new DatabaseSetupContext<TDbContext>(databaseSetup);
    }

    public static DatabaseSetup WithQuartzNpgsqlDbProvider(this DatabaseSetup databaseSetup)
    {
        databaseSetup.ServiceCollection.AddSingleton<IDbProvider>(
            new QuartzNpgsqlDbProvider(databaseSetup.NpgsqlDataSource));

        return databaseSetup;
    }
}
