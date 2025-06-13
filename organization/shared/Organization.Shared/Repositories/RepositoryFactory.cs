using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using TimeProvider = System.TimeProvider;

namespace Organization.Shared.Repositories;

public interface IRepositoryFactory
{
    OrganizationDbContext DbContext { get; }
    IUnitOfWork UnitOfWork { get; }
    IAddressRepository AddressRepository { get; }
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
    IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }
    ITeamRepository TeamRepository { get; }
    ITermsOfUseRepository TermsOfUseRepository { get; }
    IJoinInvitationRepository JoinInvitationRepository { get; }
    ITagRepository TagRepository { get; }
    IStripeCustomerRepository StripeCustomerRepository { get; }
    IStripePaymentIntentRepository StripePaymentIntentRepository { get; }
    IStripePaymentMethodRepository StripePaymentMethodRepository { get; }
    IOrganizationBillingDetailsRepository OrganizationBillingDetailsRepository { get; }
}

public class RepositoryFactory : RepositoryFactoryBase<OrganizationDbContext>, IRepositoryFactory
{
    public RepositoryFactory(IDbContextFactory<OrganizationDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        _dbContext = dbContextFactory.CreateDbContext();

        AddressRepository = new AddressRepository(_dbContext, timeProvider);
        AzureInstallStateUserIdLookupRepository = new AzureInstallStateUserIdLookupRepository(_dbContext, timeProvider);
        AzureTenantRepository = new AzureTenantRepository(_dbContext, timeProvider);
        AzureTenantMemberRepository = new AzureTenantMemberRepository(_dbContext, timeProvider);
        BookingRepository = new BookingRepository(_dbContext, timeProvider);
        CustomerRepository = new CustomerRepository(_dbContext, timeProvider);
        DailyMemberCountRecordingRepository = new DailyMemberCountRecordingRepository(_dbContext, timeProvider);
        IdentityRepository = new IdentityRepository(_dbContext, timeProvider);
        IndustryMainCategoryRepository = new IndustryMainCategoryRepository(_dbContext, timeProvider);
        IndustrySubCategoryRepository = new IndustrySubCategoryRepository(_dbContext, timeProvider);
        LocationRepository = new LocationRepository(_dbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(_dbContext, timeProvider);
        OrganizationOfferingActiveMemberRepository = new OrganizationOfferingActiveMemberRepository(_dbContext, timeProvider);
        OrganizationOfferingRepository = new OrganizationOfferingRepository(_dbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(_dbContext, timeProvider);
        OrganizationSsoSettingRepository = new OrganizationSsoSettingRepository(_dbContext, timeProvider);
        TeamRepository = new TeamRepository(_dbContext, timeProvider);
        TermsOfUseRepository = new TermsOfUseRepository(_dbContext, timeProvider);
        JoinInvitationRepository = new JoinInvitationRepository(_dbContext, timeProvider);
        TagRepository = new TagRepository(_dbContext, timeProvider);
        StripeCustomerRepository = new StripeCustomerRepository(_dbContext, timeProvider);
        StripePaymentIntentRepository = new StripePaymentIntentRepository(_dbContext, timeProvider);
        StripePaymentMethodRepository = new StripePaymentMethodRepository(_dbContext, timeProvider);
        OrganizationBillingDetailsRepository = new OrganizationOrganizationBillingDetailsRepository(_dbContext, timeProvider);
    }

    public IAddressRepository AddressRepository { get; }
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
    public IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }
    public ITeamRepository TeamRepository { get; }
    public ITermsOfUseRepository TermsOfUseRepository { get; }
    public IJoinInvitationRepository JoinInvitationRepository { get; }
    public ITagRepository TagRepository { get; }
    public IStripeCustomerRepository StripeCustomerRepository { get; }
    public IStripePaymentIntentRepository StripePaymentIntentRepository { get; }
    public IStripePaymentMethodRepository StripePaymentMethodRepository { get; }
    public IOrganizationBillingDetailsRepository OrganizationBillingDetailsRepository { get; }
}
