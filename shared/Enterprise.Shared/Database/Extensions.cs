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
    extension(IServiceCollection services)
    {
        public IServiceCollection WithPooledDbContext<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionName,
            bool isPostgisEnabled = false)
            where TDbContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(connectionName);
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            return services.WithPooledDbContextWithConnectionString<TDbContext>(configuration, environment, connectionString, isPostgisEnabled);
        }

        public IServiceCollection WithPooledDbContextWithConnectionString<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionString,
            bool isPostgisEnabled = false)
            where TDbContext : DbContext
        {
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

        public IServiceCollection WithDbContext<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionName,
            bool isPostgisEnabled = false)
            where TDbContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(connectionName);
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            return services.WithDbContextWithConnectionString<TDbContext>(configuration, environment, connectionString, isPostgisEnabled);
        }

        public IServiceCollection WithDbContextWithConnectionString<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionString,
            bool isPostgisEnabled = false)
            where TDbContext : DbContext
        {
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

        public IServiceCollection WithPooledDbContextFactory<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionName,
            bool isPostgisEnabled = false)
            where TDbContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(connectionName);
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            return services.WithPooledDbContextFactoryWithConnectionString<TDbContext>(configuration, environment, connectionString,
                isPostgisEnabled);
        }

        public IServiceCollection WithPooledDbContextFactoryWithConnectionString<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionString,
            bool isPostgisEnabled = false)
            where TDbContext : DbContext
        {
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

        public IServiceCollection WithDbContextFactory<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionName,
            bool isPostgisEnabled = false)
            where TDbContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(connectionName);
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            return services.WithDbContextFactoryWithConnectionString<TDbContext>(configuration, environment, connectionString, isPostgisEnabled);
        }

        public IServiceCollection WithDbContextFactoryWithConnectionString<TDbContext>(
            IConfiguration configuration,
            IHostEnvironment environment,
            string connectionString,
            bool isPostgisEnabled = false)
            where TDbContext : DbContext
        {
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

        private NpgsqlDataSource GetDatasource(bool isPooled, bool isPostgisEnabled, string connectionString)
        {
            services
                .AddSingleton(new CustomDbContextOptions { IsPooled = isPooled, IsPostgisEnabled = isPostgisEnabled })
                .AddSingleton<IDbTransactionBuilder, DbTransactionBuilder>()
                .AddSingleton<IDatabaseMigrationService, DatabaseMigrationService>();

            var dataSource = connectionString.BuildDataSource(isPostgisEnabled);

            services.AddDatabaseHealthCheck(dataSource);

            return dataSource;
        }
    }
}
