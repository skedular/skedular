using Enterprise.Shared.Configurations.Extensions;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Slack.Shared.Database.Entities;

namespace Slack.Shared.Database;

public class SlackDbContext(
    DbContextOptions<SlackDbContext> options,
    CustomDbContextOptions customDbContextOptions) : DbContextBase<SlackDbContext>(options, customDbContextOptions)
{
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<Location> Location { get; set; }
    public DbSet<Organization> Organization { get; set; }
    public DbSet<OrganizationMember> OrganizationMember { get; set; }
    public DbSet<Team> Team { get; set; }
    public DbSet<Workspace> Workspace { get; set; }
    public DbSet<WorkspaceChannel> WorkspaceChannel { get; set; }
    public DbSet<WorkspaceMember> WorkspaceMember { get; set; }

    // ReSharper disable once UnusedType.Global
    public class SlackDbContextDesignFactory : IDesignTimeDbContextFactory<SlackDbContext>
    {
        public SlackDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().BuildConfig<Program>(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), args);

            return new SlackDbContext(
                configuration.CreateDbContextOptionBuilder<SlackDbContext>().Options,
                new CustomDbContextOptions { IsPooled = false });
        }
    }
}
