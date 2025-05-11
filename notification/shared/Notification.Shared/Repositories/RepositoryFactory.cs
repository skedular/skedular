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
    INotificationRepository NotificationRepository { get; }
    ITeamRepository TeamRepository { get; }
    IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }
}

public class RepositoryFactory : IRepositoryFactory, IDisposable
{
    private bool _disposed;

    public RepositoryFactory(IDbContextFactory<NotificationDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        DbContext = dbContextFactory.CreateDbContext();

        CustomerRepository = new CustomerRepository(DbContext, timeProvider);
        IdentityRepository = new IdentityRepository(DbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(DbContext, timeProvider);
        LocationRepository = new LocationRepository(DbContext, timeProvider);
        NotificationRepository = new NotificationRepository(DbContext, timeProvider);
        TeamRepository = new TeamRepository(DbContext, timeProvider);
        OrganizationSsoSettingRepository = new OrganizationSsoSettingRepository(DbContext, timeProvider);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public NotificationDbContext DbContext { get; }

    public IUnitOfWork UnitOfWork => DbContext;
    public ICustomerRepository CustomerRepository { get; }
    public IIdentityRepository IdentityRepository { get; }
    public IOrganizationRepository OrganizationRepository { get; }
    public ILocationRepository LocationRepository { get; }
    public INotificationRepository NotificationRepository { get; }
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
