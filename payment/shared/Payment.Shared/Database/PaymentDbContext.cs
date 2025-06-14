using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Payment.Shared.Database.Entities;

namespace Payment.Shared.Database;

public class PaymentDbContext(DbContextOptions<PaymentDbContext> options, CustomDbContextOptions customDbContextOptions)
    : DbContextBase<PaymentDbContext>(options, customDbContextOptions), IKafkaOutboxStore, ITemporalOutboxStore
{
    public DbSet<Address> Address { get; set; }
    public DbSet<Booking> Booking { get; set; }
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<Organization> Organization { get; set; }
    public DbSet<OrganizationMember> OrganizationMember { get; set; }
    public DbSet<OrganizationSsoSetting> OrganizationSsoSetting { get; set; }
    public DbSet<OrganizationOffering> OrganizationOffering { get; set; }
    public DbSet<Product> Product { get; set; }
    public DbSet<ProductVersion> ProductVersion { get; set; }
    public DbSet<StripeCheckoutSession> StripeCheckoutSession { get; set; }
    public DbSet<StripeConnectAccount> StripeConnectAccount { get; set; }
    public DbSet<StripeConnectAccountAuthorization> StripeConnectAccountAuthorization { get; set; }
    public DbSet<StripeConnectAccountRefreshCode> StripeConnectAccountRefreshCode { get; set; }
    public DbSet<StripeCustomer> StripeCustomer { get; set; }
    public DbSet<StripePrice> StripePrice { get; set; }
    public DbSet<StripeProduct> StripeProduct { get; set; }
    public DbSet<KafkaOutbox> KafkaOutbox { get; set; }
    public DbSet<TemporalOutbox> TemporalOutbox { get; set; }

    // ReSharper disable once UnusedType.Global
    public class PaymentDbContextDesignFactory : IDesignTimeDbContextFactory<PaymentDbContext>
    {
        public PaymentDbContext CreateDbContext(string[] args) =>
            new(args.ToDbContextOption<PaymentDbContext>(), new CustomDbContextOptions { IsPooled = false });
    }
}
