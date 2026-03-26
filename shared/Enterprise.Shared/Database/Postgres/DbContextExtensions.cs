using Enterprise.Shared.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Enterprise.Shared.Database.Postgres;

public static class DbContextExtensions
{
    extension(string[] args)
    {
        public DbContextOptions<TDbContext> ToDbContextOption<TDbContext>(bool isPostgisEnabled)
            where TDbContext : DbContext =>
            new DbContextOptionsBuilder<TDbContext>()
                .UseLazyLoadingProxies()
                .UseNpgsql(
                    new ConfigurationBuilder()
                        .BuildConfig(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), args)
                        .GetConnectionString(string.Empty),
                    npgsqlOptions =>
                    {
                        if (isPostgisEnabled)
                        {
                            npgsqlOptions.UseNetTopologySuite();
                        }
                    }).Options;

        public DbContextOptions<TDbContext> ToDbContextOption<TProgram, TDbContext>(bool isPostgisEnabled)
            where TProgram : class
            where TDbContext : DbContext =>
            new DbContextOptionsBuilder<TDbContext>()
                .UseLazyLoadingProxies()
                .UseNpgsql(
                    new ConfigurationBuilder()
                        .BuildConfig<TProgram>(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), args)
                        .GetConnectionString(string.Empty),
                    npgsqlOptions =>
                    {
                        if (isPostgisEnabled)
                        {
                            npgsqlOptions.UseNetTopologySuite();
                        }
                    }).Options;
    }
}
