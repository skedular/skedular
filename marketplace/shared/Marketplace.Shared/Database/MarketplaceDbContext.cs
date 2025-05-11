using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Database.Entities;
using Marketplace.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Marketplace.Shared.Database;

public class MarketplaceDbContext(DbContextOptions<MarketplaceDbContext> options, CustomDbContextOptions customDbContextOptions)
    : DbContextBase<MarketplaceDbContext>(options, customDbContextOptions), IOutboxStore
{
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<Organization> Organization { get; set; }
    public DbSet<OrganizationTag> OrganizationTag { get; set; }
    public DbSet<OrganizationMember> OrganizationMember { get; set; }
    public DbSet<OrganizationSsoSetting> OrganizationSsoSetting { get; set; }
    public DbSet<Product> Product { get; set; }
    public DbSet<ProductVersion> ProductVersion { get; set; }
    public DbSet<Outbox> Outbox { get; set; }

    public class MarketplaceDbContextDesignFactory : IDesignTimeDbContextFactory<MarketplaceDbContext>
    {
        public MarketplaceDbContext CreateDbContext(string[] args) =>
            new(args.ToDbContextOption<MarketplaceDbContext>(), new CustomDbContextOptions { IsPooled = false });
    }
}
