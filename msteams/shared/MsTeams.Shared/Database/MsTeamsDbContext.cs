using Enterprise.Shared.Database;
using Enterprise.Shared.Infrastructure.Configuration.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MsTeams.Shared.Database.Entities;

namespace MsTeams.Shared.Database;

public class MsTeamsDbContext(
    DbContextOptions<MsTeamsDbContext> options,
    CustomDbContextOptions customDbContextOptions) : DbContextBase<MsTeamsDbContext>(options, customDbContextOptions)
{
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Identity> Identity { get; set; }
    public DbSet<TemporaryAuthorizationCode> TemporaryAuthorizationCode { get; set; }
    public DbSet<Tenant> Tenant { get; set; }
    public DbSet<TenantMember> TenantMember { get; set; }

    public class MsTeamsDbContextDesignFactory : IDesignTimeDbContextFactory<MsTeamsDbContext>
    {
        public MsTeamsDbContext CreateDbContext(string[] args)
        {
            var configuration =
                new ConfigurationBuilder().BuildConfig<Program>(
                    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), args);

            return new MsTeamsDbContext(
                configuration.CreateDbContextOptionBuilder<MsTeamsDbContext>().Options,
                new CustomDbContextOptions { IsPooled = false });
        }
    }
}
