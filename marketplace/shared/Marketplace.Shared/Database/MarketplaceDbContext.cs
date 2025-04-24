using Enterprise.Shared.Configurations.Extensions;
using Enterprise.Shared.Database;
using Marketplace.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Marketplace.Shared.Database;

public class MarketplaceDbContext(DbContextOptions<MarketplaceDbContext> options, CustomDbContextOptions customDbContextOptions)
    : DbContextBase<MarketplaceDbContext>(options, customDbContextOptions)
{
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<Organization> Organization { get; set; }
    public DbSet<OrganizationTag> OrganizationTag { get; set; }
    public DbSet<OrganizationMember> OrganizationMember { get; set; }
    public DbSet<OrganizationSsoSetting> OrganizationSsoSetting { get; set; }
    public DbSet<Product> Product { get; set; }
    public DbSet<ProductVersion> ProductVersion { get; set; }
    public DbSet<OrganizationStripeConnectAccount> OrganizationStripeConnectAccount { get; set; }

    public class MarketplaceDbContextDesignFactory : IDesignTimeDbContextFactory<MarketplaceDbContext>
    {
        public MarketplaceDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().BuildConfig<Program>(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), args);

            return new MarketplaceDbContext(
                configuration.CreateDbContextOptionBuilder<MarketplaceDbContext>().Options,
                new CustomDbContextOptions { IsPooled = false });
        }
    }
}
