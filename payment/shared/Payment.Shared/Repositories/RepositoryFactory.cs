using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Payment.Shared.Database;

namespace Payment.Shared.Repositories;

public interface IRepositoryFactory
{
    PaymentDbContext DbContext { get; }
    IUnitOfWork UnitOfWork { get; }
    IAddressRepository AddressRepository { get; }
    IBookingRepository BookingRepository { get; }
    ICustomerRepository CustomerRepository { get; }
    IIdentityRepository IdentityRepository { get; }
    IOrganizationRepository OrganizationRepository { get; }
    IOrganizationMemberRepository OrganizationMemberRepository { get; }
    IOrganizationOfferingRepository OrganizationOfferingRepository { get; }
    IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }
    IProductRepository ProductRepository { get; }
    IProductVersionRepository ProductVersionRepository { get; }
    IStripeCheckoutSessionRepository StripeCheckoutSessionRepository { get; }
    IStripeConnectAccountRefreshCodeRepository StripeConnectAccountRefreshCodeRepository { get; }
    IStripeConnectAccountRepository StripeConnectAccountRepository { get; }
    IStripeConnectAccountAuthorizationRepository StripeConnectAccountAuthorizationRepository { get; }
    IStripeCustomerRepository StripeCustomerRepository { get; }
    IStripePaymentIntentRepository StripePaymentIntentRepository { get; }
    IStripePaymentMethodRepository StripePaymentMethodRepository { get; }
    IStripePriceRepository StripePriceRepository { get; }
    IStripeProductRepository StripeProductRepository { get; }
}

public class RepositoryFactory : IRepositoryFactory, IDisposable
{
    private bool _disposed;

    public RepositoryFactory(IDbContextFactory<PaymentDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        DbContext = dbContextFactory.CreateDbContext();

        AddressRepository = new AddressRepository(DbContext, timeProvider);
        BookingRepository = new BookingRepository(DbContext, timeProvider);
        CustomerRepository = new CustomerRepository(DbContext, timeProvider);
        IdentityRepository = new IdentityRepository(DbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(DbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(DbContext, timeProvider);
        OrganizationOfferingRepository = new OrganizationOfferingRepository(DbContext, timeProvider);
        OrganizationSsoSettingRepository = new OrganizationSsoSettingRepository(DbContext, timeProvider);
        ProductRepository = new ProductRepository(DbContext, timeProvider);
        ProductVersionRepository = new ProductVersionRepository(DbContext, timeProvider);
        StripeCheckoutSessionRepository = new StripeCheckoutSessionRepository(DbContext, timeProvider);
        StripeConnectAccountRefreshCodeRepository = new StripeConnectAccountRefreshCodeRepository(DbContext, timeProvider);
        StripeConnectAccountRepository = new StripeConnectAccountRepository(DbContext, timeProvider);
        StripeConnectAccountAuthorizationRepository = new StripeConnectAccountAuthorizationRepository(DbContext, timeProvider);
        StripeCustomerRepository = new StripeCustomerRepository(DbContext, timeProvider);
        StripePaymentIntentRepository = new StripePaymentIntentRepository(DbContext, timeProvider);
        StripePaymentMethodRepository = new StripePaymentMethodRepository(DbContext, timeProvider);
        StripePriceRepository = new StripePriceRepository(DbContext, timeProvider);
        StripeProductRepository = new StripeProductRepository(DbContext, timeProvider);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public PaymentDbContext DbContext { get; }

    public IUnitOfWork UnitOfWork => DbContext;
    public IAddressRepository AddressRepository { get; }
    public IBookingRepository BookingRepository { get; }
    public ICustomerRepository CustomerRepository { get; }
    public IIdentityRepository IdentityRepository { get; }
    public IOrganizationRepository OrganizationRepository { get; }
    public IOrganizationMemberRepository OrganizationMemberRepository { get; }
    public IOrganizationOfferingRepository OrganizationOfferingRepository { get; }
    public IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }
    public IProductRepository ProductRepository { get; }
    public IProductVersionRepository ProductVersionRepository { get; }
    public IStripeCheckoutSessionRepository StripeCheckoutSessionRepository { get; }
    public IStripeConnectAccountRefreshCodeRepository StripeConnectAccountRefreshCodeRepository { get; }
    public IStripeConnectAccountRepository StripeConnectAccountRepository { get; }
    public IStripeConnectAccountAuthorizationRepository StripeConnectAccountAuthorizationRepository { get; }
    public IStripeCustomerRepository StripeCustomerRepository { get; }
    public IStripePaymentIntentRepository StripePaymentIntentRepository { get; }
    public IStripePaymentMethodRepository StripePaymentMethodRepository { get; }
    public IStripePriceRepository StripePriceRepository { get; }
    public IStripeProductRepository StripeProductRepository { get; }

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
