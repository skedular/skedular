using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using MsTeams.Shared.Database;

namespace MsTeams.Shared.Repositories;

public interface IRepositoryFactory
{
    MsTeamsDbContext DbContext { get; }
    IUnitOfWork UnitOfWork { get; }
    IAzureTenantRepository AzureTenantRepository { get; }
    IAzureTenantTeamChannelRepository AzureTenantTeamChannelRepository { get; }
    IAzureTenantTeamRepository AzureTenantTeamRepository { get; }
    ICustomerRepository CustomerRepository { get; }
    IIdentityRepository IdentityRepository { get; }
    ILocationRepository LocationRepository { get; }
    IOrganizationRepository OrganizationRepository { get; }
    IOrganizationMemberRepository OrganizationMemberRepository { get; }
    ITeamRepository TeamRepository { get; }
    IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }
}

public class RepositoryFactory : IRepositoryFactory, IDisposable
{
    private bool _disposed;

    public RepositoryFactory(IDbContextFactory<MsTeamsDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        DbContext = dbContextFactory.CreateDbContext();

        AzureTenantRepository = new AzureTenantRepository(DbContext, timeProvider);
        AzureTenantTeamChannelRepository = new AzureTenantTeamChannelRepository(DbContext, timeProvider);
        AzureTenantTeamRepository = new AzureTenantTeamRepository(DbContext, timeProvider);
        CustomerRepository = new CustomerRepository(DbContext, timeProvider);
        IdentityRepository = new IdentityRepository(DbContext, timeProvider);
        LocationRepository = new LocationRepository(DbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(DbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(DbContext, timeProvider);
        TeamRepository = new TeamRepository(DbContext, timeProvider);
        OrganizationSsoSettingRepository = new OrganizationSsoSettingRepository(DbContext, timeProvider);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public MsTeamsDbContext DbContext { get; }

    public IUnitOfWork UnitOfWork => DbContext;
    public IAzureTenantRepository AzureTenantRepository { get; }
    public IAzureTenantTeamChannelRepository AzureTenantTeamChannelRepository { get; }
    public IAzureTenantTeamRepository AzureTenantTeamRepository { get; }
    public ICustomerRepository CustomerRepository { get; }
    public IIdentityRepository IdentityRepository { get; }
    public ILocationRepository LocationRepository { get; }
    public IOrganizationRepository OrganizationRepository { get; }
    public IOrganizationMemberRepository OrganizationMemberRepository { get; }
    public ITeamRepository TeamRepository { get; }
    public IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }

    ~RepositoryFactory() => Dispose(false);

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            DbContext.Dispose();
        }

        _disposed = true;
    }
}
