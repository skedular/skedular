using Api.Shared.Services.Cache;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using TimeProvider = System.TimeProvider;

namespace Organization.Shared.Repositories;

public interface IRepositoryFactory
{
    OrganizationDbContext DbContext { get; }
    IUnitOfWork UnitOfWork { get; }
    IOrganizationPhysicalAddressRepository OrganizationPhysicalAddressRepository { get; }
    IAzureInstallStateUserIdLookupRepository AzureInstallStateUserIdLookupRepository { get; }
    IAzureTenantRepository AzureTenantRepository { get; }
    IAzureTenantMemberRepository AzureTenantMemberRepository { get; }
    IBookingRepository BookingRepository { get; }
    ICustomerRepository CustomerRepository { get; }
    IDailyMemberCountRecordingRepository DailyMemberCountRecordingRepository { get; }
    IIdentityRepository IdentityRepository { get; }
    IIndustryMainCategoryRepository IndustryMainCategoryRepository { get; }
    IIndustrySubCategoryRepository IndustrySubCategoryRepository { get; }
    ILocationRepository LocationRepository { get; }
    IOrganizationMemberRepository OrganizationMemberRepository { get; }
    IOrganizationOfferingActiveMemberRepository OrganizationOfferingActiveMemberRepository { get; }
    IOrganizationOfferingRepository OrganizationOfferingRepository { get; }
    IOrganizationRepository OrganizationRepository { get; }
    IOrganizationSsoSettingsRepository OrganizationSsoSettingsRepository { get; }
    ITeamRepository TeamRepository { get; }
    ITermsOfUseRepository TermsOfUseRepository { get; }
    IJoinInvitationRepository JoinInvitationRepository { get; }
    ITagRepository TagRepository { get; }
    IOrganizationStripeCustomerRepository OrganizationStripeCustomerRepository { get; }
    IOrganizationStripePaymentIntentRepository OrganizationStripePaymentIntentRepository { get; }
    IOrganizationStripePaymentMethodRepository OrganizationStripePaymentMethodRepository { get; }
    IOrganizationBillingDetailsRepository OrganizationBillingDetailsRepository { get; }
    IOrganizationStripeConnectAccountRefreshCodeRepository OrganizationStripeConnectAccountRefreshCodeRepository { get; }
    IOrganizationStripeConnectAccountRepository OrganizationStripeConnectAccountRepository { get; }
    IOrganizationStripeConnectAccountAuthorizationRepository OrganizationStripeConnectAccountAuthorizationRepository { get; }
    IOrganizationBankAccountRepository OrganizationBankAccountRepository { get; }
    IOrganizationTaxDetailsRepository OrganizationTaxDetailsRepository { get; }
}

public class RepositoryFactory : RepositoryFactoryBase<OrganizationDbContext>, IRepositoryFactory
{
    public RepositoryFactory(
        IDbContextFactory<OrganizationDbContext> dbContextFactory,
        TimeProvider timeProvider,
        IGenericCustomerCacheService genericCustomerCacheService)
    {
        _dbContext = dbContextFactory.CreateDbContext();

        OrganizationPhysicalAddressRepository = new OrganizationPhysicalAddressRepository(_dbContext, timeProvider);
        AzureInstallStateUserIdLookupRepository = new AzureInstallStateUserIdLookupRepository(_dbContext, timeProvider);
        AzureTenantRepository = new AzureTenantRepository(_dbContext, timeProvider);
        AzureTenantMemberRepository = new AzureTenantMemberRepository(_dbContext, timeProvider);
        BookingRepository = new BookingRepository(_dbContext, timeProvider);
        CustomerRepository = new CustomerRepository(_dbContext, timeProvider, genericCustomerCacheService);
        DailyMemberCountRecordingRepository = new DailyMemberCountRecordingRepository(_dbContext, timeProvider);
        IdentityRepository = new IdentityRepository(_dbContext, timeProvider);
        IndustryMainCategoryRepository = new IndustryMainCategoryRepository(_dbContext, timeProvider);
        IndustrySubCategoryRepository = new IndustrySubCategoryRepository(_dbContext, timeProvider);
        LocationRepository = new LocationRepository(_dbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(_dbContext, timeProvider);
        OrganizationOfferingActiveMemberRepository = new OrganizationOfferingActiveMemberRepository(_dbContext, timeProvider);
        OrganizationOfferingRepository = new OrganizationOfferingRepository(_dbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(_dbContext, timeProvider);
        OrganizationSsoSettingsRepository = new OrganizationSsoSettingsRepository(_dbContext, timeProvider);
        TeamRepository = new TeamRepository(_dbContext, timeProvider);
        TermsOfUseRepository = new TermsOfUseRepository(_dbContext, timeProvider);
        JoinInvitationRepository = new JoinInvitationRepository(_dbContext, timeProvider);
        TagRepository = new TagRepository(_dbContext, timeProvider);
        OrganizationStripeCustomerRepository = new OrganizationOrganizationStripeCustomerRepository(_dbContext, timeProvider);
        OrganizationStripePaymentIntentRepository = new OrganizationOrganizationStripePaymentIntentRepository(_dbContext, timeProvider);
        OrganizationStripePaymentMethodRepository = new OrganizationStripePaymentMethodRepository(_dbContext, timeProvider);
        OrganizationBillingDetailsRepository = new OrganizationOrganizationBillingDetailsRepository(_dbContext, timeProvider);
        OrganizationStripeConnectAccountRefreshCodeRepository = new OrganizationStripeConnectAccountRefreshCodeRepository(_dbContext, timeProvider);
        OrganizationStripeConnectAccountRepository = new OrganizationStripeConnectAccountRepository(_dbContext, timeProvider);
        OrganizationStripeConnectAccountAuthorizationRepository =
            new OrganizationStripeConnectAccountAuthorizationRepository(_dbContext, timeProvider);
        OrganizationBankAccountRepository = new OrganizationBankAccountRepository(_dbContext, timeProvider);
        OrganizationTaxDetailsRepository = new OrganizationTaxDetailsRepository(_dbContext, timeProvider);
    }

    public IOrganizationPhysicalAddressRepository OrganizationPhysicalAddressRepository { get; }
    public IAzureInstallStateUserIdLookupRepository AzureInstallStateUserIdLookupRepository { get; }
    public IAzureTenantRepository AzureTenantRepository { get; }
    public IAzureTenantMemberRepository AzureTenantMemberRepository { get; }
    public IBookingRepository BookingRepository { get; }
    public ICustomerRepository CustomerRepository { get; }
    public IDailyMemberCountRecordingRepository DailyMemberCountRecordingRepository { get; }
    public IIdentityRepository IdentityRepository { get; }
    public IIndustryMainCategoryRepository IndustryMainCategoryRepository { get; }
    public IIndustrySubCategoryRepository IndustrySubCategoryRepository { get; }
    public ILocationRepository LocationRepository { get; }
    public IOrganizationMemberRepository OrganizationMemberRepository { get; }
    public IOrganizationOfferingActiveMemberRepository OrganizationOfferingActiveMemberRepository { get; }
    public IOrganizationOfferingRepository OrganizationOfferingRepository { get; }
    public IOrganizationRepository OrganizationRepository { get; }
    public IOrganizationSsoSettingsRepository OrganizationSsoSettingsRepository { get; }
    public ITeamRepository TeamRepository { get; }
    public ITermsOfUseRepository TermsOfUseRepository { get; }
    public IJoinInvitationRepository JoinInvitationRepository { get; }
    public ITagRepository TagRepository { get; }
    public IOrganizationStripeCustomerRepository OrganizationStripeCustomerRepository { get; }
    public IOrganizationStripePaymentIntentRepository OrganizationStripePaymentIntentRepository { get; }
    public IOrganizationStripePaymentMethodRepository OrganizationStripePaymentMethodRepository { get; }
    public IOrganizationBillingDetailsRepository OrganizationBillingDetailsRepository { get; }
    public IOrganizationStripeConnectAccountRefreshCodeRepository OrganizationStripeConnectAccountRefreshCodeRepository { get; }
    public IOrganizationStripeConnectAccountRepository OrganizationStripeConnectAccountRepository { get; }
    public IOrganizationStripeConnectAccountAuthorizationRepository OrganizationStripeConnectAccountAuthorizationRepository { get; }
    public IOrganizationBankAccountRepository OrganizationBankAccountRepository { get; }
    public IOrganizationTaxDetailsRepository OrganizationTaxDetailsRepository { get; }
}
