using Enterprise.Shared.Database.SqlServer.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Enterprise.Shared.Database.SqlServer;

public abstract class DbContextBase<TDbContext>(DbContextOptions<TDbContext> options, CustomDbContextOptions<TDbContext> customDbContextOptions)
    : RelationalDbContextBase<TDbContext>(options, customDbContextOptions)
    where TDbContext : DbContextBase<TDbContext>
{
    protected override void ConfigureProviderModel(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes().Where(item => typeof(EntityBase).IsAssignableFrom(item.ClrType)))
        {
            builder.Entity(entityType.ClrType)
                .Property<uint>(nameof(EntityBase.EntityFrameworkVersion))
                .IsConcurrencyToken()
                .ValueGeneratedNever()
                .HasColumnType("bigint");
        }
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyEntityFrameworkVersionUpdates();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyEntityFrameworkVersionUpdates();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override IInterceptor CreateSelectForUpdateCommandInterceptor() => new SelectForUpdateCommandInterceptor();

    private void ApplyEntityFrameworkVersionUpdates()
    {
        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(item => item.EntityFrameworkVersion).CurrentValue = 0;
                continue;
            }

            if (entry.State != EntityState.Modified)
            {
                continue;
            }

            var property = entry.Property(item => item.EntityFrameworkVersion);
            property.CurrentValue = checked(property.OriginalValue + 1);
            property.IsModified = true;
        }
    }
}
