using System.Reflection;
using Enterprise.Shared.Database.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.Database;

public abstract class DbContextBase<TDbContext>(DbContextOptions<TDbContext> options, CustomDbContextOptions customDbContextOptions)
    : DbContext(options), IUnitOfWork where TDbContext : DbContextBase<TDbContext>
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        var contextAssembly = Assembly.GetAssembly(typeof(TDbContext));
        ArgumentNullException.ThrowIfNull(contextAssembly);

        builder.ApplyConfigurationsFromAssembly(contextAssembly);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!customDbContextOptions.IsPooled)
        {
            optionsBuilder.AddInterceptors(new SelectForUpdateCommandInterceptor());
        }

        base.OnConfiguring(optionsBuilder);
    }
}
