using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Notification.Shared.Database.Entities;

namespace Notification.Shared.Database;

public class NotificationDbContext(DbContextOptions<NotificationDbContext> options, CustomDbContextOptions customDbContextOptions)
    : DbContextBase<NotificationDbContext>(options, customDbContextOptions), IKafkaOutboxStore, ITemporalOutboxStore, ITemporalSignalOutboxStore
{
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<Location> Location { get; set; }
    public DbSet<Entities.Notification> Notification { get; set; }
    public DbSet<Organization> Organization { get; set; }
    public DbSet<OrganizationSsoSetting> OrganizationSsoSetting { get; set; }
    public DbSet<Team> Team { get; set; }
    public DbSet<KafkaOutbox> KafkaOutbox { get; set; }
    public DbSet<TemporalOutbox> TemporalOutbox { get; set; }
    public DbSet<TemporalSignalOutbox> TemporalSignalOutbox { get; set; }

    // ReSharper disable once UnusedType.Global
    public class NotificationDbContextDesignFactory : IDesignTimeDbContextFactory<NotificationDbContext>
    {
        public NotificationDbContext CreateDbContext(string[] args) =>
            new(args.ToDbContextOption<NotificationDbContext>(), new CustomDbContextOptions { IsPooled = false });
    }
}
