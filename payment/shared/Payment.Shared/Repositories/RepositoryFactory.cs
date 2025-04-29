using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Payment.Shared.Database;

namespace Payment.Shared.Repositories;

public interface IRepositoryFactory
{
    IUnitOfWork UnitOfWork { get; }
    IAddressRepository AddressRepository { get; }
    IBookingRepository BookingRepository { get; }
    ICustomerRepository CustomerRepository { get; }
    IIdentityRepository IdentityRepository { get; }
    IOrganizationRepository OrganizationRepository { get; }
    IOrganizationMemberRepository OrganizationMemberRepository { get; }
    IOrganizationOfferingRepository OrganizationOfferingRepository { get; }
    IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }
    IOrganizationStripeConnectAccountRepository OrganizationStripeConnectAccountRepository { get; }
    IOrganizationStripeConnectAccountRefreshCodeRepository OrganizationStripeConnectAccountRefreshCodeRepository { get; }
    IProductRepository ProductRepository { get; }
    IProductVersionRepository ProductVersionRepository { get; }
    IStripeCustomerRepository StripeCustomerRepository { get; }
    IStripePaymentIntentRepository StripePaymentIntentRepository { get; }
    IStripePaymentMethodRepository StripePaymentMethodRepository { get; }
    IStripeProductRepository StripeProductRepository { get; }
    IStripePriceRepository StripePriceRepository { get; }
}

public class RepositoryFactory : IRepositoryFactory, IDisposable
{
    private readonly PaymentDbContext _dbContext;
    private bool _disposed;

    public RepositoryFactory(IDbContextFactory<PaymentDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        _dbContext = dbContextFactory.CreateDbContext();

        AddressRepository = new AddressRepository(_dbContext, timeProvider);
        BookingRepository = new BookingRepository(_dbContext, timeProvider);
        CustomerRepository = new CustomerRepository(_dbContext, timeProvider);
        IdentityRepository = new IdentityRepository(_dbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(_dbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(_dbContext, timeProvider);
        OrganizationOfferingRepository = new OrganizationOfferingRepository(_dbContext, timeProvider);
        OrganizationStripeConnectAccountRepository = new OrganizationStripeConnectAccountRepository(_dbContext, timeProvider);
        OrganizationSsoSettingRepository = new OrganizationSsoSettingRepository(_dbContext, timeProvider);
        OrganizationStripeConnectAccountRefreshCodeRepository = new OrganizationStripeConnectAccountRefreshCodeRepository(_dbContext, timeProvider);
        ProductRepository = new ProductRepository(_dbContext, timeProvider);
        ProductVersionRepository = new ProductVersionRepository(_dbContext, timeProvider);
        StripeCustomerRepository = new StripeCustomerRepository(_dbContext, timeProvider);
        StripePaymentIntentRepository = new StripePaymentIntentRepository(_dbContext, timeProvider);
        StripePaymentMethodRepository = new StripePaymentMethodRepository(_dbContext, timeProvider);
        StripeProductRepository = new StripeProductRepository(_dbContext, timeProvider);
        StripePriceRepository = new StripePriceRepository(_dbContext, timeProvider);
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
    public IIdentityRepository IdentityRepository { get; }
    public IOrganizationRepository OrganizationRepository { get; }
    public IOrganizationMemberRepository OrganizationMemberRepository { get; }
    public IOrganizationOfferingRepository OrganizationOfferingRepository { get; }
    public IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }
    public IOrganizationStripeConnectAccountRepository OrganizationStripeConnectAccountRepository { get; }
    public IOrganizationStripeConnectAccountRefreshCodeRepository OrganizationStripeConnectAccountRefreshCodeRepository { get; }
    public IProductRepository ProductRepository { get; }
    public IProductVersionRepository ProductVersionRepository { get; }
    public IStripeCustomerRepository StripeCustomerRepository { get; }
    public IStripePaymentIntentRepository StripePaymentIntentRepository { get; }
    public IStripePaymentMethodRepository StripePaymentMethodRepository { get; }
    public IStripeProductRepository StripeProductRepository { get; }
    public IStripePriceRepository StripePriceRepository { get; }

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
