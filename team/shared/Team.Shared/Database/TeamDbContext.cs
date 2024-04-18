using Enterprise.Shared.Database;
using Enterprise.Shared.Infrastructure.Configuration.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Team.Shared.Database.Entities;

namespace Team.Shared.Database;

public class TeamDbContext(
    DbContextOptions<TeamDbContext> options,
    CustomDbContextOptions customDbContextOptions) : DbContextBase<TeamDbContext>(options, customDbContextOptions)
{
    public DbSet<Booking> Booking { get; set; }
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<JoinInvitation> JoinInvitation { get; set; }
    public DbSet<Entities.Team> Team { get; set; }
    public DbSet<TeamMember> TeamMember { get; set; }
    public DbSet<Organization> Organization { get; set; }
    public DbSet<OrganizationMember> OrganizationMember { get; set; }

    // ReSharper disable once UnusedType.Global
    public class TeamDbContextDesignFactory : IDesignTimeDbContextFactory<TeamDbContext>
    {
        public TeamDbContext CreateDbContext(string[] args)
        {
            var configuration =
                new ConfigurationBuilder().BuildConfig<Program>(
                    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), args);

            return new TeamDbContext(
                configuration.CreateDbContextOptionBuilder<TeamDbContext>().Options,
                new CustomDbContextOptions { IsPooled = false });
        }
    }
}
