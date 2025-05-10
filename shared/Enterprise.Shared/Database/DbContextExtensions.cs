using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.Database;

public static class DbContextExtensions
{
    public static DbContextOptionsBuilder<TDbContext> CreateDbContextOptionBuilder<TDbContext>() where TDbContext : DbContext =>
        new DbContextOptionsBuilder<TDbContext>()
            .UseLazyLoadingProxies();
}
