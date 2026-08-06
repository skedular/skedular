using Core.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.Outbox.Kafka;
using Enterprise.Shared.Outbox.Temporal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Core.Shared.Database;

public class CoreDbContext(DbContextOptions<CoreDbContext> options, CustomDbContextOptions<CoreDbContext> customDbContextOptions)
    : DbContextBase<CoreDbContext>(options, customDbContextOptions), IKafkaOutboxStore, ITemporalOutboxStore, ITemporalSignalOutboxStore
{
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<Organization> Organization { get; set; }
    public DbSet<OrganizationMember> OrganizationMember { get; set; }
    public DbSet<OrganizationSsoSetting> OrganizationSsoSetting { get; set; }
    public DbSet<CdnFile> CdnFile { get; set; }
    public DbSet<PrivateFile> PrivateFile { get; set; }
    public DbSet<KafkaOutbox> KafkaOutbox { get; set; }
    public DbSet<TemporalOutbox> TemporalOutbox { get; set; }
    public DbSet<TemporalSignalOutbox> TemporalSignalOutbox { get; set; }

    public class CoreDbContextDesignFactory : IDesignTimeDbContextFactory<CoreDbContext>
    {
        public CoreDbContext CreateDbContext(string[] args) =>
            new(args.ToDbContextOption<CoreDbContext>(true), new CustomDbContextOptions<CoreDbContext>
            {
                IsPooled = false,
                IsPostgisEnabled = true,
            });
    }
}
