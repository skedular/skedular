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

public class RepositoryFactory : RepositoryFactoryBase<MsTeamsDbContext>, IRepositoryFactory
{
    public RepositoryFactory(IDbContextFactory<MsTeamsDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        _dbContext = dbContextFactory.CreateDbContext();

        AzureTenantRepository = new AzureTenantRepository(_dbContext, timeProvider);
        AzureTenantTeamChannelRepository = new AzureTenantTeamChannelRepository(_dbContext, timeProvider);
        AzureTenantTeamRepository = new AzureTenantTeamRepository(_dbContext, timeProvider);
        CustomerRepository = new CustomerRepository(_dbContext, timeProvider);
        IdentityRepository = new IdentityRepository(_dbContext, timeProvider);
        LocationRepository = new LocationRepository(_dbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(_dbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(_dbContext, timeProvider);
        TeamRepository = new TeamRepository(_dbContext, timeProvider);
        OrganizationSsoSettingRepository = new OrganizationSsoSettingRepository(_dbContext, timeProvider);
    }

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
}
