using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MsTeams.Shared.Database.Entities;

namespace MsTeams.Shared.Database;

public class MsTeamsDbContext(DbContextOptions<MsTeamsDbContext> options, CustomDbContextOptions customDbContextOptions)
    : DbContextBase<MsTeamsDbContext>(options, customDbContextOptions), IOutboxStore
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
    public DbSet<Outbox> Outbox { get; set; }

    public class MsTeamsDbContextDesignFactory : IDesignTimeDbContextFactory<MsTeamsDbContext>
    {
        public MsTeamsDbContext CreateDbContext(string[] args) =>
            new(DbContextExtensions.CreateDbContextOptionBuilder<MsTeamsDbContext>().Options, new CustomDbContextOptions { IsPooled = false });
    }
}
