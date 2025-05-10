using System.Reflection;
using Enterprise.Shared.Database.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.Database;

public abstract class DbContextBase<TContext>(DbContextOptions<TContext> options, CustomDbContextOptions customDbContextOptions)
    : DbContext(options), IUnitOfWork where TContext : DbContextBase<TContext>
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        var contextAssembly = Assembly.GetAssembly(typeof(TContext));
        ArgumentNullException.ThrowIfNull(contextAssembly);

        builder.ApplyConfigurationsFromAssembly(contextAssembly);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // SelectForUpdateCommandInterceptor required for Outbox
        if (!customDbContextOptions.IsPooled)
        {
            optionsBuilder.AddInterceptors(new SelectForUpdateCommandInterceptor());
        }

        base.OnConfiguring(optionsBuilder);
    }
}
