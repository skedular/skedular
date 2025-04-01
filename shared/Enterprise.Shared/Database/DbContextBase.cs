using System.Reflection;
using Enterprise.Shared.Database.Interceptors;
using Enterprise.Shared.Outbox.Database;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.Database;

public abstract class DbContextBase<TContext>(DbContextOptions<TContext> options, CustomDbContextOptions customDbContextOptions)
    : DbContext(options), IUnitOfWork, IOutboxStore where TContext : DbContextBase<TContext>
{
    public DbSet<Outbox.Database.Entities.Outbox> Outbox { get; set; }

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

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTimeOffset>(builder =>
            builder.HaveConversion<DateTimeOffsetToUtcConverter>()
                .HaveConversion<NullableDateTimeOffsetToUtcConverter>());

        base.ConfigureConventions(configurationBuilder);
    }
}
