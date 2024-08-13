using Microsoft.EntityFrameworkCore;
using MsTeams.Shared.Database;

namespace MsTeams.Shared.Repositories;

public interface IRepositoryFactory
{
    ICustomerRepository CustomerRepository { get; }
    IIdentityRepository IdentityRepository { get; }
    IInstallStateUserIdLookupRepository InstallStateUserIdLookupRepository { get; }
    ILocationRepository LocationRepository { get; }
    IOrganizationRepository OrganizationRepository { get; }
    IOrganizationMemberRepository OrganizationMemberRepository { get; }
    ITeamRepository TeamRepository { get; }
    ITenantRepository TenantRepository { get; }
    ITenantMemberRepository TenantMemberRepository { get; }
}

public class RepositoryFactory : IRepositoryFactory, IAsyncDisposable
{
    private readonly MsTeamsDbContext _dbContext;

    public RepositoryFactory(IDbContextFactory<MsTeamsDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        _dbContext = dbContextFactory.CreateDbContext();

        CustomerRepository = new CustomerRepository(_dbContext, timeProvider);
        IdentityRepository = new IdentityRepository(_dbContext, timeProvider);
        InstallStateUserIdLookupRepository = new InstallStateUserIdLookupRepository(_dbContext, timeProvider);
        LocationRepository = new LocationRepository(_dbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(_dbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(_dbContext, timeProvider);
        TeamRepository = new TeamRepository(_dbContext, timeProvider);
        TenantRepository = new TenantRepository(_dbContext, timeProvider);
        TenantMemberRepository = new TenantMemberRepository(_dbContext, timeProvider);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    public ICustomerRepository CustomerRepository { get; }
    public IIdentityRepository IdentityRepository { get; }
    public IInstallStateUserIdLookupRepository InstallStateUserIdLookupRepository { get; }
    public ILocationRepository LocationRepository { get; }
    public IOrganizationRepository OrganizationRepository { get; }
    public IOrganizationMemberRepository OrganizationMemberRepository { get; }
    public ITeamRepository TeamRepository { get; }
    public ITenantRepository TenantRepository { get; }
    public ITenantMemberRepository TenantMemberRepository { get; }

    protected virtual async ValueTask DisposeAsyncCore() => await _dbContext.DisposeAsync();
}
