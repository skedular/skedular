using Booking.Shared.Database;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IRepositoryFactory
{
    BookingDbContext DbContext { get; }
    IUnitOfWork UnitOfWork { get; }
    IBookingRepository BookingRepository { get; }
    IBookingCheckoutSessionRepository BookingCheckoutSessionRepository { get; }
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
    IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }
    IProductRepository ProductRepository { get; }
    IProductVersionRepository ProductVersionRepository { get; }
}

public class RepositoryFactory : IRepositoryFactory, IDisposable
{
    private bool _disposed;

    public RepositoryFactory(
        IDbContextFactory<BookingDbContext> dbContextFactory,
        TimeProvider timeProvider,
        IBookingCheckoutSessionHelperService bookingCheckoutSessionHelperService)
    {
        DbContext = dbContextFactory.CreateDbContext();

        BookingRepository = new BookingRepository(DbContext, timeProvider, bookingCheckoutSessionHelperService);
        BookingCheckoutSessionRepository = new BookingCheckoutSessionRepository(DbContext, timeProvider);
        CustomerRepository = new CustomerRepository(DbContext, timeProvider);
        IdentityRepository = new IdentityRepository(DbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(DbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(DbContext, timeProvider);
        LocationRepository = new LocationRepository(DbContext, timeProvider);
        ResourceRepository = new ResourceRepository(DbContext, timeProvider);
        ResourceBookingSlotRepository = new ResourceBookingSlotRepository(DbContext, timeProvider);
        TeamRepository = new TeamRepository(DbContext, timeProvider);
        TeamMemberRepository = new TeamMemberRepository(DbContext, timeProvider);
        OrganizationTagRepository = new OrganizationTagRepository(DbContext, timeProvider);
        OrganizationSsoSettingRepository = new OrganizationSsoSettingRepository(DbContext, timeProvider);
        ProductRepository = new ProductRepository(DbContext, timeProvider);
        ProductVersionRepository = new ProductVersionRepository(DbContext, timeProvider);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public BookingDbContext DbContext { get; }

    public IUnitOfWork UnitOfWork => DbContext;
    public IBookingRepository BookingRepository { get; }
    public IBookingCheckoutSessionRepository BookingCheckoutSessionRepository { get; }
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
    public IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }
    public IProductRepository ProductRepository { get; }
    public IProductVersionRepository ProductVersionRepository { get; }

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
