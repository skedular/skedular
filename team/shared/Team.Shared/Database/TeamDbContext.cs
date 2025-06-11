using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Team.Shared.Database.Entities;

namespace Team.Shared.Database;

public class TeamDbContext(DbContextOptions<TeamDbContext> options, CustomDbContextOptions customDbContextOptions)
    : DbContextBase<TeamDbContext>(options, customDbContextOptions), IKafkaOutboxStore, ITemporalOutboxStore
{
    public DbSet<Booking> Booking { get; set; }
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<JoinInvitation> JoinInvitation { get; set; }
    public DbSet<Entities.Team> Team { get; set; }
    public DbSet<TeamMember> TeamMember { get; set; }
    public DbSet<Organization> Organization { get; set; }
    public DbSet<OrganizationSsoSetting> OrganizationSsoSetting { get; set; }
    public DbSet<OrganizationMember> OrganizationMember { get; set; }
    public DbSet<Location> Location { get; set; }
    public DbSet<KafkaOutbox> KafkaOutbox { get; set; }
    public DbSet<TemporalOutbox> TemporalOutbox { get; set; }

    // ReSharper disable once UnusedType.Global
    public class TeamDbContextDesignFactory : IDesignTimeDbContextFactory<TeamDbContext>
    {
        public TeamDbContext CreateDbContext(string[] args) =>
            new(args.ToDbContextOption<TeamDbContext>(), new CustomDbContextOptions { IsPooled = false });
    }
}
