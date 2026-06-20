using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Enterprise.Shared.Database;

public abstract class RelationalDbContextBase<TDbContext>(
    DbContextOptions<TDbContext> options,
    CustomDbContextOptions<TDbContext> customDbContextOptions)
    : DbContext(options), IUnitOfWork where TDbContext : RelationalDbContextBase<TDbContext>
{
    protected CustomDbContextOptions<TDbContext> CustomDbContextOptions { get; } = customDbContextOptions;

    public bool HasActiveTransaction => Database.CurrentTransaction is not null;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var contextAssembly = Assembly.GetAssembly(typeof(TDbContext));
        ArgumentNullException.ThrowIfNull(contextAssembly);

        builder.ApplyConfigurationsFromAssembly(contextAssembly);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        ConfigureProviderModel(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!CustomDbContextOptions.IsPooled)
        {
            optionsBuilder.AddInterceptors(CreateSelectForUpdateCommandInterceptor());
        }

        base.OnConfiguring(optionsBuilder);
    }

    protected virtual void ConfigureProviderModel(ModelBuilder builder)
    {
    }

    protected abstract IInterceptor CreateSelectForUpdateCommandInterceptor();
}
