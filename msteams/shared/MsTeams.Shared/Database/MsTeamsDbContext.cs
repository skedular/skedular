using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.Outbox.Kafka;
using Enterprise.Shared.Outbox.Temporal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MsTeams.Shared.Database.Entities;

namespace MsTeams.Shared.Database;

public class MsTeamsDbContext(DbContextOptions<MsTeamsDbContext> options, CustomDbContextOptions<MsTeamsDbContext> customDbContextOptions)
    : DbContextBase<MsTeamsDbContext>(options, customDbContextOptions), IKafkaOutboxStore, ITemporalOutboxStore, ITemporalSignalOutboxStore
{
    public DbSet<AzureTenant> AzureTenant { get; set; }
    public DbSet<AzureTenantTeam> AzureTenantTeam { get; set; }
    public DbSet<AzureTenantTeamChannel> AzureTenantTeamChannel { get; set; }
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<Location> Location { get; set; }
    public DbSet<Organization> Organization { get; set; }
    public DbSet<OrganizationMember> OrganizationMember { get; set; }
    public DbSet<OrganizationSsoSetting> OrganizationSsoSetting { get; set; }
    public DbSet<Team> Team { get; set; }
    public DbSet<KafkaOutbox> KafkaOutbox { get; set; }
    public DbSet<TemporalOutbox> TemporalOutbox { get; set; }
    public DbSet<TemporalSignalOutbox> TemporalSignalOutbox { get; set; }

    public class MsTeamsDbContextDesignFactory : IDesignTimeDbContextFactory<MsTeamsDbContext>
    {
        public MsTeamsDbContext CreateDbContext(string[] args) =>
            new(args.ToDbContextOption<MsTeamsDbContext>(true),
                new CustomDbContextOptions<MsTeamsDbContext> { IsPooled = false, IsPostgisEnabled = true });
    }
}
