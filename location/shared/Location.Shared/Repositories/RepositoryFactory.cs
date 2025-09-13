using Enterprise.Shared.Database;
using Location.Shared.Database;
using Microsoft.EntityFrameworkCore;
using TimeProvider = System.TimeProvider;

namespace Location.Shared.Repositories;

public interface IRepositoryFactory
{
    LocationDbContext DbContext { get; }
    IUnitOfWork UnitOfWork { get; }
    ILocationPhysicalAddressRepository LocationPhysicalAddressRepository { get; }
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
    IFloorPlanRepository FloorPlanRepository { get; }
    IResourcePositionRepository ResourcePositionRepository { get; }
}

public class RepositoryFactory : RepositoryFactoryBase<LocationDbContext>, IRepositoryFactory
{
    public RepositoryFactory(IDbContextFactory<LocationDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        _dbContext = dbContextFactory.CreateDbContext();

        LocationPhysicalAddressRepository = new LocationPhysicalAddressRepository(_dbContext, timeProvider);
        BookingRepository = new BookingRepository(_dbContext, timeProvider);
        CustomerRepository = new CustomerRepository(_dbContext, timeProvider);
        DailyDeskCountRecordingRepository = new DailyDeskCountRecordingRepository(_dbContext, timeProvider);
        DailyRoomCountRecordingRepository = new DailyRoomCountRecordingRepository(_dbContext, timeProvider);
        ResourceRepository = new ResourceRepository(_dbContext, timeProvider);
        IdentityRepository = new IdentityRepository(_dbContext, timeProvider);
        LocationRepository = new LocationRepository(_dbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(_dbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(_dbContext, timeProvider);
        OrganizationTagRepository = new OrganizationTagRepository(_dbContext, timeProvider);
        OrganizationSsoSettingRepository = new OrganizationSsoSettingRepository(_dbContext, timeProvider);
        FloorPlanRepository = new FloorPlanRepository(_dbContext, timeProvider);
        ResourcePositionRepository = new ResourcePositionRepository(_dbContext, timeProvider);
    }

    public ILocationPhysicalAddressRepository LocationPhysicalAddressRepository { get; }
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
    public IFloorPlanRepository FloorPlanRepository { get; }
    public IResourcePositionRepository ResourcePositionRepository { get; }
}
