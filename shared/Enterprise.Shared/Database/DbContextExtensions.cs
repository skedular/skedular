using Enterprise.Shared.Configurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Enterprise.Shared.Database;

public static class DbContextExtensions
{
    public static DbContextOptions<TDbContext> ToDbContextOption<TDbContext>(this string[] args)
        where TDbContext : DbContext =>
        new DbContextOptionsBuilder<TDbContext>()
            .UseLazyLoadingProxies()
            .UseNpgsql(
                new ConfigurationBuilder()
                    .BuildConfig(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), args)
                    .GetConnectionString(string.Empty)).Options;

    public static DbContextOptions<TDbContext> ToDbContextOption<TProgram, TDbContext>(this string[] args)
        where TProgram : class
        where TDbContext : DbContext =>
        new DbContextOptionsBuilder<TDbContext>()
            .UseLazyLoadingProxies()
            .UseNpgsql(
                new ConfigurationBuilder()
                    .BuildConfig<TProgram>(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), args)
                    .GetConnectionString(string.Empty)).Options;
}
