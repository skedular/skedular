using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.Outbox.Kafka;
using Enterprise.Shared.Outbox.Temporal;
using Location.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Location.Shared.Database;

public class LocationDbContext(DbContextOptions<LocationDbContext> options, CustomDbContextOptions<LocationDbContext> customDbContextOptions)
    : DbContextBase<LocationDbContext>(options, customDbContextOptions), IKafkaOutboxStore, ITemporalOutboxStore, ITemporalSignalOutboxStore
{
    public DbSet<LocationPhysicalAddress> LocationPhysicalAddress { get; set; }
    public DbSet<Customer> Customer { get; set; }
    public DbSet<DailyBookingCountRecording> DailyBookingCountRecording { get; set; }
    public DbSet<DailyDeskBookingCountRecording> DailyDeskBookingCountRecording { get; set; }
    public DbSet<DailyDeskCountRecording> DailyDeskCountRecording { get; set; }
    public DbSet<DailyRoomBookingCountRecording> DailyRoomBookingCountRecording { get; set; }
    public DbSet<DailyRoomCountRecording> DailyRoomCountRecording { get; set; }
    public DbSet<FloorPlan> FloorPlan { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<Entities.Location> Location { get; set; }
    public DbSet<Organization> Organization { get; set; }
    public DbSet<OrganizationMember> OrganizationMember { get; set; }
    public DbSet<OrganizationSsoSetting> OrganizationSsoSetting { get; set; }
    public DbSet<OrganizationTag> OrganizationTag { get; set; }
    public DbSet<Resource> Resource { get; set; }
    public DbSet<ResourcePosition> ResourcePosition { get; set; }
    public DbSet<Product> Product { get; set; }
    public DbSet<ProductVersion> ProductVersion { get; set; }
    public DbSet<PrecomputedLocationProduct> PrecomputedLocationProduct { get; set; }
    public DbSet<DailyResourceAvailabilitySnapshot> DailyResourceAvailabilitySnapshot { get; set; }
    public DbSet<LocationRestrictedInformation> LocationRestrictedInformation { get; set; }
    public DbSet<LocationBookingAccess> LocationBookingAccess { get; set; }
    public DbSet<KafkaOutbox> KafkaOutbox { get; set; }
    public DbSet<TemporalOutbox> TemporalOutbox { get; set; }
    public DbSet<TemporalSignalOutbox> TemporalSignalOutbox { get; set; }

    // ReSharper disable once UnusedType.Global
    public class LocationDbContextDesignFactory : IDesignTimeDbContextFactory<LocationDbContext>
    {
        public LocationDbContext CreateDbContext(string[] args) =>
            new(args.ToDbContextOption<LocationDbContext>(true),
                new CustomDbContextOptions<LocationDbContext>
                {
                    IsPooled = false,
                    IsPostgisEnabled = true,
                });
    }
}
