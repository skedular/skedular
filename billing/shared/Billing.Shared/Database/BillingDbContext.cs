using Billing.Shared.Database.Entities;
using Enterprise.Shared.Configurations.Extensions;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Billing.Shared.Database;

public class BillingDbContext(DbContextOptions<BillingDbContext> options, CustomDbContextOptions customDbContextOptions)
    : DbContextBase<BillingDbContext>(options, customDbContextOptions)
{
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<Organization> Organization { get; set; }
    public DbSet<OrganizationMember> OrganizationMember { get; set; }
    public DbSet<OrganizationSsoSetting> OrganizationSsoSetting { get; set; }
    public DbSet<OrganizationOffering> OrganizationOffering { get; set; }

    // ReSharper disable once UnusedType.Global
    public class BillingDbContextDesignFactory : IDesignTimeDbContextFactory<BillingDbContext>
    {
        public BillingDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().BuildConfig<Program>(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), args);

            return new BillingDbContext(configuration.CreateDbContextOptionBuilder<BillingDbContext>().Options,
                new CustomDbContextOptions { IsPooled = false });
        }
    }
}
