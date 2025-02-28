using Booking.Shared.Database.Entities;
using Enterprise.Shared.Configurations.Extensions;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Booking.Shared.Database;

public class BookingDbContext(
    DbContextOptions<BookingDbContext> options,
    CustomDbContextOptions customDbContextOptions) : DbContextBase<BookingDbContext>(options, customDbContextOptions)
{
    public DbSet<Entities.Booking> Booking { get; set; }
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Desk> Desk { get; set; }
    public DbSet<Room> Room { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<Location> Location { get; set; }
    public DbSet<LocationMember> LocationMember { get; set; }
    public DbSet<LocationResource> LocationResource { get; set; }
    public DbSet<Organization> Organization { get; set; }
    public DbSet<OrganizationMember> OrganizationMember { get; set; }
    public DbSet<OrganizationResourceType> OrganizationResourceType { get; set; }
    public DbSet<OrganizationTag> OrganizationTag { get; set; }
    public DbSet<Team> Team { get; set; }
    public DbSet<TeamMember> TeamMember { get; set; }

    // ReSharper disable once UnusedType.Global
    public class BookingDbContextDesignFactory : IDesignTimeDbContextFactory<BookingDbContext>
    {
        public BookingDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().BuildConfig<Program>(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), args);

            return new BookingDbContext(
                configuration.CreateDbContextOptionBuilder<BookingDbContext>().Options,
                new CustomDbContextOptions { IsPooled = false });
        }
    }
}
