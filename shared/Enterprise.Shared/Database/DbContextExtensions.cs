using Enterprise.Shared.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Enterprise.Shared.Database;

public static class DbContextExtensions
{
    public static DbContextOptions<TDbContext> ToDbContextOption<TDbContext>(this string[] args, bool isPostgisEnabled)
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

    public static DbContextOptions<TDbContext> ToDbContextOption<TProgram, TDbContext>(this string[] args, bool isPostgisEnabled)
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
