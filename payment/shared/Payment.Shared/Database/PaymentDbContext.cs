using Enterprise.Shared.Configurations.Extensions;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Payment.Shared.Database.Entities;

namespace Payment.Shared.Database;

public class PaymentDbContext(DbContextOptions<PaymentDbContext> options, CustomDbContextOptions customDbContextOptions)
    : DbContextBase<PaymentDbContext>(options, customDbContextOptions)
{
    public DbSet<Address> Address { get; set; }
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<Organization> Organization { get; set; }
    public DbSet<OrganizationMember> OrganizationMember { get; set; }
    public DbSet<OrganizationSsoSetting> OrganizationSsoSetting { get; set; }
    public DbSet<OrganizationOffering> OrganizationOffering { get; set; }
    public DbSet<OrganizationOfferingStripePaymentIntent> OrganizationOfferingStripePaymentIntent { get; set; }
    public DbSet<OrganizationStripeConnectAccount> OrganizationStripeConnectAccount { get; set; }
    public DbSet<OrganizationStripeConnectAccountRefreshCode> OrganizationStripeConnectAccountRefreshCode { get; set; }
    public DbSet<OrganizationStripePaymentMethod> OrganizationStripePaymentMethod { get; set; }
    public DbSet<Product> Product { get; set; }
    public DbSet<ProductVersion> ProductVersion { get; set; }
    public DbSet<StripeCustomer> StripeCustomer { get; set; }

    // ReSharper disable once UnusedType.Global
    public class PaymentDbContextDesignFactory : IDesignTimeDbContextFactory<PaymentDbContext>
    {
        public PaymentDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().BuildConfig<Program>(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), args);

            return new PaymentDbContext(
                configuration.CreateDbContextOptionBuilder<PaymentDbContext>().Options,
                new CustomDbContextOptions { IsPooled = false });
        }
    }
}
