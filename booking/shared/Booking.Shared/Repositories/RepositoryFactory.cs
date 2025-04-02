using Booking.Shared.Database;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IRepositoryFactory
{
    IUnitOfWork UnitOfWork { get; }
    IBookingRepository BookingRepository { get; }
    ICustomerRepository CustomerRepository { get; }
    IIdentityRepository IdentityRepository { get; }
    IOrganizationRepository OrganizationRepository { get; }
    IOrganizationMemberRepository OrganizationMemberRepository { get; }
    ILocationRepository LocationRepository { get; }
    IResourceRepository ResourceRepository { get; }
    IResourceBookingSlotRepository ResourceBookingSlotRepository { get; }
    ITeamRepository TeamRepository { get; }
    ITeamMemberRepository TeamMemberRepository { get; }
    IOrganizationTagRepository OrganizationTagRepository { get; }
}

public class RepositoryFactory : IRepositoryFactory, IDisposable
{
    private readonly BookingDbContext _dbContext;
    private bool _disposed;

    public RepositoryFactory(IDbContextFactory<BookingDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        _dbContext = dbContextFactory.CreateDbContext();

        BookingRepository = new BookingRepository(_dbContext, timeProvider);
        CustomerRepository = new CustomerRepository(_dbContext, timeProvider);
        IdentityRepository = new IdentityRepository(_dbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(_dbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(_dbContext, timeProvider);
        LocationRepository = new LocationRepository(_dbContext, timeProvider);
        ResourceRepository = new ResourceRepository(_dbContext, timeProvider);
        ResourceBookingSlotRepository = new ResourceBookingSlotRepository(_dbContext, timeProvider);
        TeamRepository = new TeamRepository(_dbContext, timeProvider);
        TeamMemberRepository = new TeamMemberRepository(_dbContext, timeProvider);
        OrganizationTagRepository = new OrganizationTagRepository(_dbContext, timeProvider);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public IUnitOfWork UnitOfWork => _dbContext;
    public IBookingRepository BookingRepository { get; }
    public ICustomerRepository CustomerRepository { get; }
    public IIdentityRepository IdentityRepository { get; }
    public IOrganizationRepository OrganizationRepository { get; }
    public IOrganizationMemberRepository OrganizationMemberRepository { get; }
    public ILocationRepository LocationRepository { get; }
    public IResourceRepository ResourceRepository { get; }
    public IResourceBookingSlotRepository ResourceBookingSlotRepository { get; }
    public ITeamRepository TeamRepository { get; }
    public ITeamMemberRepository TeamMemberRepository { get; }
    public IOrganizationTagRepository OrganizationTagRepository { get; }

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
