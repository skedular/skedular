using Microsoft.EntityFrameworkCore;
using Notification.Shared.Database;

namespace Notification.Shared.Repositories;

public interface IRepositoryFactory
{
    ICustomerRepository CustomerRepository { get; }
    IIdentityRepository IdentityRepository { get; }
    IOrganizationRepository OrganizationRepository { get; }
    ILocationRepository LocationRepository { get; }
    INotificationRepository NotificationRepository { get; }
    ITeamRepository TeamRepository { get; }
}

public class RepositoryFactory : IRepositoryFactory, IDisposable
{
    private readonly NotificationDbContext _dbContext;
    private bool _disposed;

    public RepositoryFactory(IDbContextFactory<NotificationDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        _dbContext = dbContextFactory.CreateDbContext();

        CustomerRepository = new CustomerRepository(_dbContext, timeProvider);
        IdentityRepository = new IdentityRepository(_dbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(_dbContext, timeProvider);
        LocationRepository = new LocationRepository(_dbContext, timeProvider);
        NotificationRepository = new NotificationRepository(_dbContext, timeProvider);
        TeamRepository = new TeamRepository(_dbContext, timeProvider);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public ICustomerRepository CustomerRepository { get; }
    public IIdentityRepository IdentityRepository { get; }
    public IOrganizationRepository OrganizationRepository { get; }
    public ILocationRepository LocationRepository { get; }
    public INotificationRepository NotificationRepository { get; }
    public ITeamRepository TeamRepository { get; }

    ~RepositoryFactory() => Dispose(false);

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _dbContext.Dispose();
        }

        _disposed = true;
    }
}
