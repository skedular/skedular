using Enterprise.Shared.Database;
using Location.Shared.Database;
using Microsoft.EntityFrameworkCore;
using TimeProvider = System.TimeProvider;

namespace Location.Shared.Repositories;

public interface IRepositoryFactory
{
    IUnitOfWork UnitOfWork { get; }
    IAddressRepository AddressRepository { get; }
    IBookingRepository BookingRepository { get; }
    ICustomerRepository CustomerRepository { get; }
    IDailyDeskCountRecordingRepository DailyDeskCountRecordingRepository { get; }
    IDailyRoomCountRecordingRepository DailyRoomCountRecordingRepository { get; }
    IResourceRepository ResourceRepository { get; }
    IDeskRepository DeskRepository { get; }
    IRoomRepository RoomRepository { get; }
    IIdentityRepository IdentityRepository { get; }
    IJoinInvitationRepository JoinInvitationRepository { get; }
    ILocationMemberRepository LocationMemberRepository { get; }
    ILocationRepository LocationRepository { get; }
    IOrganizationMemberRepository OrganizationMemberRepository { get; }
    IOrganizationRepository OrganizationRepository { get; }
    IOrganizationTagRepository OrganizationTagRepository { get; }
    IOrganizationResourceTypeRepository OrganizationResourceTypeRepository { get; }
}

public class RepositoryFactory : IRepositoryFactory, IDisposable
{
    private readonly LocationDbContext _dbContext;
    private bool _disposed;

    public RepositoryFactory(IDbContextFactory<LocationDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        _dbContext = dbContextFactory.CreateDbContext();

        AddressRepository = new AddressRepository(_dbContext, timeProvider);
        BookingRepository = new BookingRepository(_dbContext, timeProvider);
        CustomerRepository = new CustomerRepository(_dbContext, timeProvider);
        DailyDeskCountRecordingRepository = new DailyDeskCountRecordingRepository(_dbContext, timeProvider);
        DailyRoomCountRecordingRepository = new DailyRoomCountRecordingRepository(_dbContext, timeProvider);
        ResourceRepository = new ResourceRepository(_dbContext, timeProvider);
        DeskRepository = new DeskRepository(_dbContext, timeProvider);
        RoomRepository = new RoomRepository(_dbContext, timeProvider);
        IdentityRepository = new IdentityRepository(_dbContext, timeProvider);
        JoinInvitationRepository = new JoinInvitationRepository(_dbContext, timeProvider);
        LocationMemberRepository = new LocationMemberRepository(_dbContext, timeProvider);
        LocationRepository = new LocationRepository(_dbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(_dbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(_dbContext, timeProvider);
        OrganizationTagRepository = new OrganizationTagRepository(_dbContext, timeProvider);
        OrganizationResourceTypeRepository = new OrganizationResourceTypeRepository(_dbContext, timeProvider);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public IUnitOfWork UnitOfWork => _dbContext;
    public IAddressRepository AddressRepository { get; }
    public IBookingRepository BookingRepository { get; }
    public ICustomerRepository CustomerRepository { get; }
    public IDailyDeskCountRecordingRepository DailyDeskCountRecordingRepository { get; }
    public IDailyRoomCountRecordingRepository DailyRoomCountRecordingRepository { get; }
    public IResourceRepository ResourceRepository { get; }
    public IDeskRepository DeskRepository { get; }
    public IRoomRepository RoomRepository { get; }
    public IIdentityRepository IdentityRepository { get; }
    public IJoinInvitationRepository JoinInvitationRepository { get; }
    public ILocationMemberRepository LocationMemberRepository { get; }
    public ILocationRepository LocationRepository { get; }
    public IOrganizationMemberRepository OrganizationMemberRepository { get; }
    public IOrganizationRepository OrganizationRepository { get; }
    public IOrganizationTagRepository OrganizationTagRepository { get; }
    public IOrganizationResourceTypeRepository OrganizationResourceTypeRepository { get; }

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
