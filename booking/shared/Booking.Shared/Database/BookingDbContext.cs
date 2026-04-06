using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Booking.Shared.Database;

public class BookingDbContext(DbContextOptions<BookingDbContext> options, CustomDbContextOptions customDbContextOptions)
    : DbContextBase<BookingDbContext>(options, customDbContextOptions), IKafkaOutboxStore, ITemporalOutboxStore, ITemporalSignalOutboxStore
{
    public DbSet<MarketplaceBookingSubscription> MarketplaceBookingSubscription { get; set; }
    public DbSet<RecurringBooking> RecurringBooking { get; set; }
    public DbSet<Entities.Booking> Booking { get; set; }
    public DbSet<MarketplaceBooking> MarketplaceBooking { get; set; }
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<Location> Location { get; set; }
    public DbSet<Organization> Organization { get; set; }
    public DbSet<OrganizationMember> OrganizationMember { get; set; }
    public DbSet<OrganizationSsoSetting> OrganizationSsoSetting { get; set; }
    public DbSet<OrganizationTag> OrganizationTag { get; set; }
    public DbSet<Product> Product { get; set; }
    public DbSet<ProductVersion> ProductVersion { get; set; }
    public DbSet<Team> Team { get; set; }
    public DbSet<TeamMember> TeamMember { get; set; }
    public DbSet<Resource> Resource { get; set; }
    public DbSet<ResourceBookingSlot> ResourceBookingSlot { get; set; }
    public DbSet<StripeProduct> StripeProduct { get; set; }
    public DbSet<StripePrice> StripePrice { get; set; }
    public DbSet<StripeCustomer> StripeCustomer { get; set; }
    public DbSet<StripeCheckoutSession> StripeCheckoutSession { get; set; }
    public DbSet<OrganizationInvoiceCounter> OrganizationInvoiceCounter { get; set; }
    public DbSet<OrganizationArrearsInvoice> OrganizationArrearsInvoice { get; set; }
    public DbSet<OrganizationArrearsInvoiceLine> OrganizationArrearsInvoiceLine { get; set; }
    public DbSet<AccountingInvoiceExportLink> AccountingInvoiceExportLink { get; set; }
    public DbSet<AccountingInvoiceInstance> AccountingInvoiceInstance { get; set; }
    public DbSet<AccountingContactLink> AccountingContactLink { get; set; }
    public DbSet<AccountingPaymentEvent> AccountingPaymentEvent { get; set; }
    public DbSet<KafkaOutbox> KafkaOutbox { get; set; }
    public DbSet<TemporalOutbox> TemporalOutbox { get; set; }
    public DbSet<TemporalSignalOutbox> TemporalSignalOutbox { get; set; }

    // ReSharper disable once UnusedType.Global
    public class BookingDbContextDesignFactory : IDesignTimeDbContextFactory<BookingDbContext>
    {
        public BookingDbContext CreateDbContext(string[] args) =>
            new(args.ToDbContextOption<BookingDbContext>(true), new CustomDbContextOptions { IsPooled = false, IsPostgisEnabled = true });
    }
}
