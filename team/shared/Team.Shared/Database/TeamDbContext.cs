using Enterprise.Shared.Configurations.Extensions;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Team.Shared.Database.Entities;

namespace Team.Shared.Database;

public class TeamDbContext(DbContextOptions<TeamDbContext> options, CustomDbContextOptions customDbContextOptions)
    : DbContextBase<TeamDbContext>(options, customDbContextOptions), IOutboxStore
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
    public DbSet<Outbox> Outbox { get; set; }

    // ReSharper disable once UnusedType.Global
    public class TeamDbContextDesignFactory : IDesignTimeDbContextFactory<TeamDbContext>
    {
        public TeamDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().BuildConfig<Program>(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), args);

            return new TeamDbContext(
                configuration.CreateDbContextOptionBuilder<TeamDbContext>().Options,
                new CustomDbContextOptions { IsPooled = false });
        }
    }
}
