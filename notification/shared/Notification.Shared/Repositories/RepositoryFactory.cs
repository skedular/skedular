using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Notification.Shared.Database;

namespace Notification.Shared.Repositories;

public interface IRepositoryFactory
{
    NotificationDbContext DbContext { get; }
    IUnitOfWork UnitOfWork { get; }
    ICustomerRepository CustomerRepository { get; }
    IIdentityRepository IdentityRepository { get; }
    IOrganizationRepository OrganizationRepository { get; }
    ILocationRepository LocationRepository { get; }
    ITeamRepository TeamRepository { get; }
    IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }
}

public class RepositoryFactory : RepositoryFactoryBase<NotificationDbContext>, IRepositoryFactory
{
    public RepositoryFactory(IDbContextFactory<NotificationDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        _dbContext = dbContextFactory.CreateDbContext();

        CustomerRepository = new CustomerRepository(_dbContext, timeProvider);
        IdentityRepository = new IdentityRepository(_dbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(_dbContext, timeProvider);
        LocationRepository = new LocationRepository(_dbContext, timeProvider);
        TeamRepository = new TeamRepository(_dbContext, timeProvider);
        OrganizationSsoSettingRepository = new OrganizationSsoSettingRepository(_dbContext, timeProvider);
    }

    public ICustomerRepository CustomerRepository { get; }
    public IIdentityRepository IdentityRepository { get; }
    public IOrganizationRepository OrganizationRepository { get; }
    public ILocationRepository LocationRepository { get; }
    public ITeamRepository TeamRepository { get; }
    public IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }
}
