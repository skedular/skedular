using Enterprise.Shared.Configurations.Extensions;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Notification.Shared.Database.Entities;

namespace Notification.Shared.Database;

public class NotificationDbContext(DbContextOptions<NotificationDbContext> options, CustomDbContextOptions customDbContextOptions)
    : DbContextBase<NotificationDbContext>(options, customDbContextOptions)
{
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<Location> Location { get; set; }
    public DbSet<Entities.Notification> Notification { get; set; }
    public DbSet<Organization> Organization { get; set; }
    public DbSet<Team> Team { get; set; }

    // ReSharper disable once UnusedType.Global
    public class NotificationDbContextDesignFactory : IDesignTimeDbContextFactory<NotificationDbContext>
    {
        public NotificationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().BuildConfig<Program>(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), args);

            return new NotificationDbContext(
                configuration.CreateDbContextOptionBuilder<NotificationDbContext>().Options,
                new CustomDbContextOptions { IsPooled = false });
        }
    }
}
