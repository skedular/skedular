using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Slack.Shared.Database.Entities;

namespace Slack.Shared.Database;

public class SlackDbContext(DbContextOptions<SlackDbContext> options, CustomDbContextOptions customDbContextOptions)
    : DbContextBase<SlackDbContext>(options, customDbContextOptions), IKafkaOutboxStore, ITemporalOutboxStore
{
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<Location> Location { get; set; }
    public DbSet<Organization> Organization { get; set; }
    public DbSet<OrganizationMember> OrganizationMember { get; set; }
    public DbSet<OrganizationSsoSetting> OrganizationSsoSetting { get; set; }
    public DbSet<Team> Team { get; set; }
    public DbSet<Workspace> Workspace { get; set; }
    public DbSet<WorkspaceChannel> WorkspaceChannel { get; set; }
    public DbSet<WorkspaceMember> WorkspaceMember { get; set; }
    public DbSet<KafkaOutbox> KafkaOutbox { get; set; }
    public DbSet<TemporalOutbox> TemporalOutbox { get; set; }

    // ReSharper disable once UnusedType.Global
    public class SlackDbContextDesignFactory : IDesignTimeDbContextFactory<SlackDbContext>
    {
        public SlackDbContext CreateDbContext(string[] args) =>
            new(args.ToDbContextOption<SlackDbContext>(), new CustomDbContextOptions { IsPooled = false });
    }
}
