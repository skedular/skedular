using Booking.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IRepositoryFactory
{
    IBookingRepository BookingRepository { get; }
    ICustomerRepository CustomerRepository { get; }
    IIdentityRepository IdentityRepository { get; }
    IOrganizationRepository OrganizationRepository { get; }
    IOrganizationMemberRepository OrganizationMemberRepository { get; }
    ILocationRepository LocationRepository { get; }
    ILocationMemberRepository LocationMemberRepository { get; }
    IDeskRepository DeskRepository { get; }
    IRoomRepository RoomRepository { get; }
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
        LocationMemberRepository = new LocationMemberRepository(_dbContext, timeProvider);
        DeskRepository = new DeskRepository(_dbContext, timeProvider);
        RoomRepository = new RoomRepository(_dbContext, timeProvider);
        TeamRepository = new TeamRepository(_dbContext, timeProvider);
        TeamMemberRepository = new TeamMemberRepository(_dbContext, timeProvider);
        OrganizationTagRepository = new OrganizationTagRepository(_dbContext, timeProvider);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public IBookingRepository BookingRepository { get; }
    public ICustomerRepository CustomerRepository { get; }
    public IIdentityRepository IdentityRepository { get; }
    public IOrganizationRepository OrganizationRepository { get; }
    public IOrganizationMemberRepository OrganizationMemberRepository { get; }
    public ILocationRepository LocationRepository { get; }
    public ILocationMemberRepository LocationMemberRepository { get; }
    public IDeskRepository DeskRepository { get; }
    public IRoomRepository RoomRepository { get; }
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
