using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Enterprise.Shared.Database;

public static class DbContextExtensions
{
    public static DbContextOptionsBuilder<TDbContext> CreateDbContextOptionBuilder<TDbContext>() where TDbContext : DbContext =>
        new DbContextOptionsBuilder<TDbContext>()
            .UseLazyLoadingProxies();

    public static DbContextOptionsBuilder<TDbContext> CreateDbContextOptionBuilder<TDbContext>(this IConfigurationRoot configuration)
        where TDbContext : DbContext =>
        CreateDbContextOptionBuilder<TDbContext>()
            .UseNpgsql(configuration.GetConnectionString(ConnectionStringKeys.DefaultPostgresConnection));
}
