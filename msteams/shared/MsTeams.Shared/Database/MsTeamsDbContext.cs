using Enterprise.Shared.Configurations.Extensions;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MsTeams.Shared.Database.Entities;

namespace MsTeams.Shared.Database;

public class MsTeamsDbContext(DbContextOptions<MsTeamsDbContext> options, CustomDbContextOptions customDbContextOptions)
    : DbContextBase<MsTeamsDbContext>(options, customDbContextOptions)
{
    public DbSet<AzureTenant> AzureTenant { get; set; }
    public DbSet<AzureTenantTeam> AzureTenantTeam { get; set; }
    public DbSet<AzureTenantTeamChannel> AzureTenantTeamChannel { get; set; }
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<Location> Location { get; set; }
    public DbSet<Organization> Organization { get; set; }
    public DbSet<OrganizationMember> OrganizationMember { get; set; }
    public DbSet<Team> Team { get; set; }

    public class MsTeamsDbContextDesignFactory : IDesignTimeDbContextFactory<MsTeamsDbContext>
    {
        public MsTeamsDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().BuildConfig<Program>(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), args);

            return new MsTeamsDbContext(
                configuration.CreateDbContextOptionBuilder<MsTeamsDbContext>().Options,
                new CustomDbContextOptions { IsPooled = false });
        }
    }
}
