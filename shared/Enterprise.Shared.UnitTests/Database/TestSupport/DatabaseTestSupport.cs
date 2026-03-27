using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.UnitTests.Database.TestSupport;

public class ParentEntity : EntityBase
{
    public string? Name { get; set; }
    public ICollection<SpecEntity> Children { get; set; } = [];
}

public class OwnerEntity : EntityBase
{
    public string? Name { get; set; }
    public ICollection<SpecEntity> OwnedEntities { get; set; } = [];
}

public class SpecEntity : EntityBaseWithDeleted
{
    public string? Name { get; set; }
    public string? ParentId { get; set; }
    public ParentEntity? Parent { get; set; }
    public string? OwnerId { get; set; }
    public OwnerEntity? Owner { get; set; }
}

public class ReplicatedDeletedEntity : ReplicatedEntityBaseWithDeleted
{
    public string? Name { get; set; }
}

public class DatabaseTestContext(DbContextOptions options) : DbContext(options), IUnitOfWork
{
    public DbSet<ParentEntity> Parents => Set<ParentEntity>();
    public DbSet<OwnerEntity> Owners => Set<OwnerEntity>();
    public DbSet<SpecEntity> Specs => Set<SpecEntity>();
    public DbSet<ReplicatedDeletedEntity> ReplicatedDeletedEntities => Set<ReplicatedDeletedEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ParentEntity>(builder =>
        {
            builder.ConfigureEntityBase();
            builder.Property(item => item.Name);
        });

        modelBuilder.Entity<OwnerEntity>(builder =>
        {
            builder.ConfigureEntityBase();
            builder.Property(item => item.Name);
        });

        modelBuilder.Entity<SpecEntity>(builder =>
        {
            builder.ConfigureEntityBaseWithDeleted();
            builder.Property(item => item.Name);
            builder.HasOne(item => item.Parent).WithMany(item => item.Children).HasForeignKey(item => item.ParentId);
            builder.HasOne(item => item.Owner).WithMany(item => item.OwnedEntities).HasForeignKey(item => item.OwnerId);
        });

        modelBuilder.Entity<ReplicatedDeletedEntity>(builder =>
        {
            builder.ConfigureReplicatedEntityBaseWithDeleted();
            builder.Property(item => item.Name);
        });
    }
}

public sealed class PostgresTestDbContext(
    DbContextOptions<PostgresTestDbContext> options,
    CustomDbContextOptions customDbContextOptions)
    : DbContextBase<PostgresTestDbContext>(options, customDbContextOptions)
{
    public DbSet<ParentEntity> Parents => Set<ParentEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ParentEntity>(builder =>
        {
            builder.ConfigureEntityBase();
            builder.Property(item => item.Name);
        });
    }
}

public sealed class SqlServerTestDbContext(
    DbContextOptions<SqlServerTestDbContext> options,
    CustomDbContextOptions customDbContextOptions)
    : Shared.Database.SqlServer.DbContextBase<SqlServerTestDbContext>(options, customDbContextOptions)
{
    public DbSet<ParentEntity> Parents => Set<ParentEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ParentEntity>(builder =>
        {
            builder.ConfigureEntityBase();
            builder.Property(item => item.Name);
        });
    }
}

public sealed class PostgresTestRepositoryFactory : RepositoryFactoryBase<PostgresTestDbContext>
{
    public void SetDbContext(PostgresTestDbContext? dbContext) => _dbContext = dbContext;
}

public sealed class SqlServerTestRepositoryFactory : Shared.Database.SqlServer.RepositoryFactoryBase<SqlServerTestDbContext>
{
    public void SetDbContext(SqlServerTestDbContext? dbContext) => _dbContext = dbContext;
}
