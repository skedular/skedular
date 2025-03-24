using Customer.Shared.Database.Entities;
using Enterprise.Shared.Configurations.Extensions;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Customer.Shared.Database;

public class CustomerDbContext(
    DbContextOptions<CustomerDbContext> options,
    CustomDbContextOptions customDbContextOptions) : DbContextBase<CustomerDbContext>(options, customDbContextOptions)
{
    public DbSet<Entities.Customer> Customer { get; set; }
    public DbSet<CustomerFeedback> CustomerFeedback { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<Location> Location { get; set; }
    public DbSet<LocationMember> LocationMember { get; set; }
    public DbSet<Organization> Organization { get; set; }
    public DbSet<OrganizationTag> OrganizationTag { get; set; }
    public DbSet<OrganizationMember> OrganizationMember { get; set; }
    public DbSet<Team> Team { get; set; }
    public DbSet<TeamMember> TeamMember { get; set; }
    public DbSet<Resource> Resource { get; set; }

    // ReSharper disable once UnusedType.Global
    public class CustomerDbContextDesignFactory : IDesignTimeDbContextFactory<CustomerDbContext>
    {
        public CustomerDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().BuildConfig<Program>(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), args);

            return new CustomerDbContext(
                configuration.CreateDbContextOptionBuilder<CustomerDbContext>().Options,
                new CustomDbContextOptions { IsPooled = false });
        }
    }
}
