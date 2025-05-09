using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Notification.Shared.Database.Entities;

namespace Notification.Shared.Database;

public class NotificationDbContext(DbContextOptions<NotificationDbContext> options, CustomDbContextOptions customDbContextOptions)
    : DbContextBase<NotificationDbContext>(options, customDbContextOptions), IOutboxStore
{
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<Location> Location { get; set; }
    public DbSet<Entities.Notification> Notification { get; set; }
    public DbSet<Organization> Organization { get; set; }
    public DbSet<OrganizationSsoSetting> OrganizationSsoSetting { get; set; }
    public DbSet<Team> Team { get; set; }
    public DbSet<Outbox> Outbox { get; set; }

    // ReSharper disable once UnusedType.Global
    public class NotificationDbContextDesignFactory : IDesignTimeDbContextFactory<NotificationDbContext>
    {
        public NotificationDbContext CreateDbContext(string[] args) =>
            new(DbContextExtensions.CreateDbContextOptionBuilder<NotificationDbContext>().Options, new CustomDbContextOptions { IsPooled = false });
    }
}
