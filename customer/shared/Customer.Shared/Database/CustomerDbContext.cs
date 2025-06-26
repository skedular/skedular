using Customer.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Customer.Shared.Database;

public class CustomerDbContext(DbContextOptions<CustomerDbContext> options, CustomDbContextOptions customDbContextOptions)
    : DbContextBase<CustomerDbContext>(options, customDbContextOptions), IKafkaOutboxStore, ITemporalOutboxStore, ITemporalSignalOutboxStore
{
    public DbSet<Entities.Customer> Customer { get; set; }
    public DbSet<CustomerFeedback> CustomerFeedback { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<Location> Location { get; set; }
    public DbSet<Organization> Organization { get; set; }
    public DbSet<OrganizationTag> OrganizationTag { get; set; }
    public DbSet<OrganizationMember> OrganizationMember { get; set; }
    public DbSet<OrganizationSsoSetting> OrganizationSsoSetting { get; set; }
    public DbSet<Team> Team { get; set; }
    public DbSet<TeamMember> TeamMember { get; set; }
    public DbSet<Resource> Resource { get; set; }
    public DbSet<StripeCustomer> StripeCustomer { get; set; }
    public DbSet<StripePaymentIntent> StripePaymentIntent { get; set; }
    public DbSet<StripePaymentMethod> StripePaymentMethod { get; set; }
    public DbSet<CustomerBillingDetails> CustomerBillingDetails { get; set; }
    public DbSet<KafkaOutbox> KafkaOutbox { get; set; }
    public DbSet<TemporalOutbox> TemporalOutbox { get; set; }
    public DbSet<TemporalSignalOutbox> TemporalSignalOutbox { get; set; }

    // ReSharper disable once UnusedType.Global
    public class CustomerDbContextDesignFactory : IDesignTimeDbContextFactory<CustomerDbContext>
    {
        public CustomerDbContext CreateDbContext(string[] args) =>
            new(args.ToDbContextOption<CustomerDbContext>(), new CustomDbContextOptions { IsPooled = false });
    }
}
