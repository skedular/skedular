using Billing.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Billing.Shared.Database;

public class BillingDbContext(DbContextOptions<BillingDbContext> options, CustomDbContextOptions customDbContextOptions)
    : DbContextBase<BillingDbContext>(options, customDbContextOptions), IOutboxStore
{
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<Organization> Organization { get; set; }
    public DbSet<OrganizationMember> OrganizationMember { get; set; }
    public DbSet<OrganizationSsoSetting> OrganizationSsoSetting { get; set; }
    public DbSet<OrganizationOffering> OrganizationOffering { get; set; }
    public DbSet<Outbox> Outbox { get; set; }

    // ReSharper disable once UnusedType.Global
    public class BillingDbContextDesignFactory : IDesignTimeDbContextFactory<BillingDbContext>
    {
        public BillingDbContext CreateDbContext(string[] args) =>
            new(args.ToDbContextOption<Program, BillingDbContext>(), new CustomDbContextOptions { IsPooled = false });
    }
}
