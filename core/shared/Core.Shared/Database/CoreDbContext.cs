using Core.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Core.Shared.Database;

public class CoreDbContext(DbContextOptions<CoreDbContext> options, CustomDbContextOptions customDbContextOptions)
    : DbContextBase<CoreDbContext>(options, customDbContextOptions), IKafkaOutboxStore
{
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<CdnFile> CdnFile { get; set; }
    public DbSet<KafkaOutbox> KafkaOutbox { get; set; }

    public class CoreDbContextDesignFactory : IDesignTimeDbContextFactory<CoreDbContext>
    {
        public CoreDbContext CreateDbContext(string[] args) =>
            new(args.ToDbContextOption<CoreDbContext>(), new CustomDbContextOptions { IsPooled = false });
    }
}
