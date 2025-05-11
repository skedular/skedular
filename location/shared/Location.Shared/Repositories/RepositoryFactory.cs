using Enterprise.Shared.Database;
using Location.Shared.Database;
using Microsoft.EntityFrameworkCore;
using TimeProvider = System.TimeProvider;

namespace Location.Shared.Repositories;

public interface IRepositoryFactory
{
    LocationDbContext DbContext { get; }
    IUnitOfWork UnitOfWork { get; }
    IAddressRepository AddressRepository { get; }
    IBookingRepository BookingRepository { get; }
    ICustomerRepository CustomerRepository { get; }
    IDailyDeskCountRecordingRepository DailyDeskCountRecordingRepository { get; }
    IDailyRoomCountRecordingRepository DailyRoomCountRecordingRepository { get; }
    IResourceRepository ResourceRepository { get; }
    IIdentityRepository IdentityRepository { get; }
    ILocationRepository LocationRepository { get; }
    IOrganizationMemberRepository OrganizationMemberRepository { get; }
    IOrganizationRepository OrganizationRepository { get; }
    IOrganizationTagRepository OrganizationTagRepository { get; }
    IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }
}

public class RepositoryFactory : IRepositoryFactory, IDisposable
{
    private bool _disposed;

    public RepositoryFactory(IDbContextFactory<LocationDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        DbContext = dbContextFactory.CreateDbContext();

        AddressRepository = new AddressRepository(DbContext, timeProvider);
        BookingRepository = new BookingRepository(DbContext, timeProvider);
        CustomerRepository = new CustomerRepository(DbContext, timeProvider);
        DailyDeskCountRecordingRepository = new DailyDeskCountRecordingRepository(DbContext, timeProvider);
        DailyRoomCountRecordingRepository = new DailyRoomCountRecordingRepository(DbContext, timeProvider);
        ResourceRepository = new ResourceRepository(DbContext, timeProvider);
        IdentityRepository = new IdentityRepository(DbContext, timeProvider);
        LocationRepository = new LocationRepository(DbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(DbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(DbContext, timeProvider);
        OrganizationTagRepository = new OrganizationTagRepository(DbContext, timeProvider);
        OrganizationSsoSettingRepository = new OrganizationSsoSettingRepository(DbContext, timeProvider);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public LocationDbContext DbContext { get; }

    public IUnitOfWork UnitOfWork => DbContext;
    public IAddressRepository AddressRepository { get; }
    public IBookingRepository BookingRepository { get; }
    public ICustomerRepository CustomerRepository { get; }
    public IDailyDeskCountRecordingRepository DailyDeskCountRecordingRepository { get; }
    public IDailyRoomCountRecordingRepository DailyRoomCountRecordingRepository { get; }
    public IResourceRepository ResourceRepository { get; }
    public IIdentityRepository IdentityRepository { get; }
    public ILocationRepository LocationRepository { get; }
    public IOrganizationMemberRepository OrganizationMemberRepository { get; }
    public IOrganizationRepository OrganizationRepository { get; }
    public IOrganizationTagRepository OrganizationTagRepository { get; }
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
