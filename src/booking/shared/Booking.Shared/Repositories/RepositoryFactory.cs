using Booking.Shared.Database;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Booking.Shared.Repositories;

public interface IRepositoryFactory
{
    BookingDbContext DbContext { get; }
    IUnitOfWork UnitOfWork { get; }
    IMarketplaceBookingSubscriptionRepository MarketplaceBookingSubscriptionRepository { get; }
    IRecurringBookingRepository RecurringBookingRepository { get; }
    IBookingRepository BookingRepository { get; }
    IMarketplaceBookingRepository MarketplaceBookingRepository { get; }
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
    IStripeProductRepository StripeProductRepository { get; }
    IStripePriceRepository StripePriceRepository { get; }
    IStripeCustomerRepository StripeCustomerRepository { get; }
    IStripeCheckoutSessionRepository StripeCheckoutSessionRepository { get; }
    IOrganizationInvoiceCounterRepository OrganizationInvoiceCounterRepository { get; }
    IOrganizationArrearsInvoiceRepository OrganizationArrearsInvoiceRepository { get; }
    IMarketplaceRefundRepository MarketplaceRefundRepository { get; }
    IMarketplaceRefundEventRepository MarketplaceRefundEventRepository { get; }
    IMarketplaceBookingFailureRepository MarketplaceBookingFailureRepository { get; }
    IMarketplaceBookingFailureEventRepository MarketplaceBookingFailureEventRepository { get; }
    IMarketplaceBookingFailureDeliveryRepository MarketplaceBookingFailureDeliveryRepository { get; }
    IAccountingInvoiceExportLinkRepository AccountingInvoiceExportLinkRepository { get; }
    IAccountingInvoiceInstanceRepository AccountingInvoiceInstanceRepository { get; }
    IAccountingContactLinkRepository AccountingContactLinkRepository { get; }
    IAccountingPaymentEventRepository AccountingPaymentEventRepository { get; }
    ISpacesBookingUsageRepository SpacesBookingUsageRepository { get; }
    void ResetChangeTracker();
}

public class RepositoryFactory : RepositoryFactoryBase<BookingDbContext>, IRepositoryFactory
{
    public RepositoryFactory(IDbContextFactory<BookingDbContext> dbContextFactory, TimeProvider timeProvider, ILoggerFactory loggerFactory)
    {
        _dbContext = dbContextFactory.CreateDbContext();

        MarketplaceBookingSubscriptionRepository = new MarketplaceBookingSubscriptionRepository(_dbContext, timeProvider);
        RecurringBookingRepository = new RecurringBookingRepository(_dbContext, timeProvider);
        BookingRepository = new BookingRepository(_dbContext, timeProvider);
        MarketplaceBookingRepository = new MarketplaceBookingRepository(_dbContext, timeProvider);
        CustomerRepository = new CustomerRepository(_dbContext, timeProvider);
        IdentityRepository = new IdentityRepository(_dbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(_dbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(_dbContext, timeProvider);
        LocationRepository = new LocationRepository(_dbContext, timeProvider);
        ResourceRepository = new ResourceRepository(_dbContext, timeProvider, loggerFactory.CreateLogger<ResourceRepository>());
        ResourceBookingSlotRepository = new ResourceBookingSlotRepository(_dbContext, timeProvider);
        TeamRepository = new TeamRepository(_dbContext, timeProvider);
        TeamMemberRepository = new TeamMemberRepository(_dbContext, timeProvider);
        OrganizationTagRepository = new OrganizationTagRepository(_dbContext, timeProvider);
        OrganizationSsoSettingRepository = new OrganizationSsoSettingRepository(_dbContext, timeProvider);
        ProductRepository = new ProductRepository(_dbContext, timeProvider);
        ProductVersionRepository = new ProductVersionRepository(_dbContext, timeProvider);
        StripeProductRepository = new StripeProductRepository(_dbContext, timeProvider);
        StripePriceRepository = new StripePriceRepository(_dbContext, timeProvider);
        StripeCustomerRepository = new StripeCustomerRepository(_dbContext, timeProvider);
        StripeCheckoutSessionRepository = new StripeCheckoutSessionRepository(_dbContext, timeProvider);
        OrganizationInvoiceCounterRepository = new OrganizationInvoiceCounterRepository(_dbContext, timeProvider);
        OrganizationArrearsInvoiceRepository = new OrganizationArrearsInvoiceRepository(_dbContext, timeProvider);
        MarketplaceRefundRepository = new MarketplaceRefundRepository(_dbContext, timeProvider);
        MarketplaceRefundEventRepository = new MarketplaceRefundEventRepository(_dbContext, timeProvider);
        MarketplaceBookingFailureRepository = new MarketplaceBookingFailureRepository(_dbContext, timeProvider);
        MarketplaceBookingFailureEventRepository = new MarketplaceBookingFailureEventRepository(_dbContext, timeProvider);
        MarketplaceBookingFailureDeliveryRepository = new MarketplaceBookingFailureDeliveryRepository(_dbContext, timeProvider);
        AccountingInvoiceExportLinkRepository = new AccountingInvoiceExportLinkRepository(_dbContext, timeProvider);
        AccountingInvoiceInstanceRepository = new AccountingInvoiceInstanceRepository(_dbContext, timeProvider);
        AccountingContactLinkRepository = new AccountingContactLinkRepository(_dbContext, timeProvider);
        AccountingPaymentEventRepository = new AccountingPaymentEventRepository(_dbContext, timeProvider);
        SpacesBookingUsageRepository = new SpacesBookingUsageRepository(_dbContext, timeProvider);
    }

    public void ResetChangeTracker() => DbContext.ChangeTracker.Clear();

    public IMarketplaceBookingSubscriptionRepository MarketplaceBookingSubscriptionRepository { get; }
    public IRecurringBookingRepository RecurringBookingRepository { get; }
    public IBookingRepository BookingRepository { get; }
    public IMarketplaceBookingRepository MarketplaceBookingRepository { get; }
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
    public IStripeProductRepository StripeProductRepository { get; }
    public IStripePriceRepository StripePriceRepository { get; }
    public IStripeCustomerRepository StripeCustomerRepository { get; }
    public IStripeCheckoutSessionRepository StripeCheckoutSessionRepository { get; }
    public IOrganizationInvoiceCounterRepository OrganizationInvoiceCounterRepository { get; }
    public IOrganizationArrearsInvoiceRepository OrganizationArrearsInvoiceRepository { get; }
    public IMarketplaceRefundRepository MarketplaceRefundRepository { get; }
    public IMarketplaceRefundEventRepository MarketplaceRefundEventRepository { get; }
    public IMarketplaceBookingFailureRepository MarketplaceBookingFailureRepository { get; }
    public IMarketplaceBookingFailureEventRepository MarketplaceBookingFailureEventRepository { get; }
    public IMarketplaceBookingFailureDeliveryRepository MarketplaceBookingFailureDeliveryRepository { get; }
    public IAccountingInvoiceExportLinkRepository AccountingInvoiceExportLinkRepository { get; }
    public IAccountingInvoiceInstanceRepository AccountingInvoiceInstanceRepository { get; }
    public IAccountingContactLinkRepository AccountingContactLinkRepository { get; }
    public IAccountingPaymentEventRepository AccountingPaymentEventRepository { get; }
    public ISpacesBookingUsageRepository SpacesBookingUsageRepository { get; }
}
