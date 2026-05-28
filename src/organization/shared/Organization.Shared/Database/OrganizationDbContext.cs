using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.Outbox.Kafka;
using Enterprise.Shared.Outbox.Temporal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Database;

public class OrganizationDbContext(
    DbContextOptions<OrganizationDbContext> options,
    CustomDbContextOptions<OrganizationDbContext> customDbContextOptions)
    : DbContextBase<OrganizationDbContext>(options, customDbContextOptions), IKafkaOutboxStore, ITemporalOutboxStore, ITemporalSignalOutboxStore
{
    public DbSet<OrganizationPhysicalAddress> OrganizationPhysicalAddress { get; set; }
    public DbSet<AzureInstallStateUserIdLookup> AzureInstallStateUserIdLookup { get; set; }
    public DbSet<AzureTenant> AzureTenant { get; set; }
    public DbSet<OrganizationSsoSettings> OrganizationSsoSettings { get; set; }
    public DbSet<AzureTenantMember> AzureTenantMember { get; set; }
    public DbSet<Customer> Customer { get; set; }
    public DbSet<DailyBookingCountRecording> DailyBookingCountRecording { get; set; }
    public DbSet<DailyMemberCountRecording> DailyMemberCountRecording { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<IndustryMainCategory> IndustryMainCategory { get; set; }
    public DbSet<IndustrySubCategory> IndustrySubCategory { get; set; }
    public DbSet<JoinInvitation> JoinInvitation { get; set; }
    public DbSet<Entities.Organization> Organization { get; set; }
    public DbSet<OrganizationMember> OrganizationMember { get; set; }
    public DbSet<OrganizationOffering> OrganizationOffering { get; set; }
    public DbSet<OrganizationOfferingActiveMember> OrganizationOfferingActiveMember { get; set; }
    public DbSet<TermsOfUse> TermsOfUse { get; set; }
    public DbSet<Tag> Tag { get; set; }
    public DbSet<OrganizationStripeCustomer> OrganizationStripeCustomer { get; set; }
    public DbSet<OrganizationStripePaymentIntent> OrganizationStripePaymentIntent { get; set; }
    public DbSet<OrganizationStripePaymentMethod> OrganizationStripePaymentMethod { get; set; }
    public DbSet<OrganizationBillingDetails> OrganizationBillingDetails { get; set; }
    public DbSet<OrganizationStripeConnectAccount> OrganizationStripeConnectAccount { get; set; }
    public DbSet<OrganizationStripeConnectAccountAuthorization> OrganizationStripeConnectAccountAuthorization { get; set; }
    public DbSet<OrganizationStripeConnectAccountRefreshCode> OrganizationStripeConnectAccountRefreshCode { get; set; }
    public DbSet<OrganizationBankAccount> OrganizationBankAccount { get; set; }
    public DbSet<OrganizationTaxDetails> OrganizationTaxDetails { get; set; }
    public DbSet<OrganizationXeroConnection> OrganizationXeroConnection { get; set; }
    public DbSet<KafkaOutbox> KafkaOutbox { get; set; }
    public DbSet<TemporalOutbox> TemporalOutbox { get; set; }
    public DbSet<TemporalSignalOutbox> TemporalSignalOutbox { get; set; }

    // ReSharper disable once UnusedType.Global
    public class OrganizationDbContextDesignFactory : IDesignTimeDbContextFactory<OrganizationDbContext>
    {
        public OrganizationDbContext CreateDbContext(string[] args) =>
            new(args.ToDbContextOption<OrganizationDbContext>(true),
                new CustomDbContextOptions<OrganizationDbContext> { IsPooled = false, IsPostgisEnabled = true });
    }
}
