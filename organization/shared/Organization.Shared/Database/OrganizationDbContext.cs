using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Database;

public class OrganizationDbContext(DbContextOptions<OrganizationDbContext> options, CustomDbContextOptions customDbContextOptions)
    : DbContextBase<OrganizationDbContext>(options, customDbContextOptions), IKafkaOutboxStore
{
    public DbSet<Address> Address { get; set; }
    public DbSet<AzureInstallStateUserIdLookup> AzureInstallStateUserIdLookup { get; set; }
    public DbSet<AzureTenant> AzureTenant { get; set; }
    public DbSet<OrganizationSsoSetting> OrganizationSsoSetting { get; set; }
    public DbSet<AzureTenantMember> AzureTenantMember { get; set; }
    public DbSet<Booking> Booking { get; set; }
    public DbSet<Customer> Customer { get; set; }
    public DbSet<DailyMemberCountRecording> DailyMemberCountRecording { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<IndustryMainCategory> IndustryMainCategory { get; set; }
    public DbSet<IndustrySubCategory> IndustrySubCategory { get; set; }
    public DbSet<JoinInvitation> JoinInvitation { get; set; }
    public DbSet<Location> Location { get; set; }
    public DbSet<Entities.Organization> Organization { get; set; }
    public DbSet<OrganizationMember> OrganizationMember { get; set; }
    public DbSet<OrganizationOffering> OrganizationOffering { get; set; }
    public DbSet<OrganizationOfferingActiveMember> OrganizationOfferingActiveMember { get; set; }
    public DbSet<Team> Team { get; set; }
    public DbSet<TermsOfUse> TermsOfUse { get; set; }
    public DbSet<Tag> Tag { get; set; }
    public DbSet<KafkaOutbox> KafkaOutbox { get; set; }

    // ReSharper disable once UnusedType.Global
    public class OrganizationDbContextDesignFactory : IDesignTimeDbContextFactory<OrganizationDbContext>
    {
        public OrganizationDbContext CreateDbContext(string[] args) =>
            new(args.ToDbContextOption<OrganizationDbContext>(), new CustomDbContextOptions { IsPooled = false });
    }
}
