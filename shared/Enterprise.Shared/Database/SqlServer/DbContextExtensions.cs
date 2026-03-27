using Enterprise.Shared.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Enterprise.Shared.Database.SqlServer;

public static class DbContextExtensions
{
    public static DbContextOptions<TDbContext> ToDbContextOption<TDbContext>(string[] args, bool isPostgisEnabled = false)
        where TDbContext : DbContext =>
        new DbContextOptionsBuilder<TDbContext>()
            .UseLazyLoadingProxies()
            .UseSqlServer(
                new ConfigurationBuilder()
                    .BuildConfig(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), args)
                    .GetConnectionString(string.Empty))
            .Options;

    public static DbContextOptions<TDbContext> ToDbContextOption<TProgram, TDbContext>(string[] args, bool isPostgisEnabled = false)
        where TProgram : class
        where TDbContext : DbContext =>
        new DbContextOptionsBuilder<TDbContext>()
            .UseLazyLoadingProxies()
            .UseSqlServer(
                new ConfigurationBuilder()
                    .BuildConfig<TProgram>(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), args)
                    .GetConnectionString(string.Empty))
            .Options;
}
