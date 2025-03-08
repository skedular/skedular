using Enterprise.Shared.Configurations.Extensions;
using Enterprise.Shared.Database;
using Location.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Location.Shared.Database;

public class LocationDbContext(
    DbContextOptions<LocationDbContext> options,
    CustomDbContextOptions customDbContextOptions) : DbContextBase<LocationDbContext>(options, customDbContextOptions)
{
    public DbSet<Address> Address { get; set; }
    public DbSet<Booking> Booking { get; set; }
    public DbSet<Customer> Customer { get; set; }
    public DbSet<DailyDeskCountRecording> DailyDeskCountRecording { get; set; }
    public DbSet<DailyRoomCountRecording> DailyRoomCountRecording { get; set; }
    public DbSet<Desk> Desk { get; set; }
    public DbSet<Room> Room { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<JoinInvitation> JoinInvitation { get; set; }
    public DbSet<Entities.Location> Location { get; set; }
    public DbSet<LocationMember> LocationMember { get; set; }
    public DbSet<Organization> Organization { get; set; }
    public DbSet<OrganizationMember> OrganizationMember { get; set; }
    public DbSet<OrganizationTag> OrganizationTag { get; set; }
    public DbSet<Resource> Resource { get; set; }

    // ReSharper disable once UnusedType.Global
    public class LocationDbContextDesignFactory : IDesignTimeDbContextFactory<LocationDbContext>
    {
        public LocationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().BuildConfig<Program>(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), args);

            return new LocationDbContext(
                configuration.CreateDbContextOptionBuilder<LocationDbContext>().Options,
                new CustomDbContextOptions { IsPooled = false });
        }
    }
}
