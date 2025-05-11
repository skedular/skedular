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
}

public class RepositoryFactory : IRepositoryFactory, IDisposable
{
    private bool _disposed;

    public RepositoryFactory(IDbContextFactory<OrganizationDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        DbContext = dbContextFactory.CreateDbContext();

        AddressRepository = new AddressRepository(DbContext, timeProvider);
        AzureInstallStateUserIdLookupRepository = new AzureInstallStateUserIdLookupRepository(DbContext, timeProvider);
        AzureTenantRepository = new AzureTenantRepository(DbContext, timeProvider);
        AzureTenantMemberRepository = new AzureTenantMemberRepository(DbContext, timeProvider);
        BookingRepository = new BookingRepository(DbContext, timeProvider);
        CustomerRepository = new CustomerRepository(DbContext, timeProvider);
        DailyMemberCountRecordingRepository = new DailyMemberCountRecordingRepository(DbContext, timeProvider);
        IdentityRepository = new IdentityRepository(DbContext, timeProvider);
        IndustryMainCategoryRepository = new IndustryMainCategoryRepository(DbContext, timeProvider);
        IndustrySubCategoryRepository = new IndustrySubCategoryRepository(DbContext, timeProvider);
        LocationRepository = new LocationRepository(DbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(DbContext, timeProvider);
        OrganizationOfferingActiveMemberRepository = new OrganizationOfferingActiveMemberRepository(DbContext, timeProvider);
        OrganizationOfferingRepository = new OrganizationOfferingRepository(DbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(DbContext, timeProvider);
        OrganizationSsoSettingRepository = new OrganizationSsoSettingRepository(DbContext, timeProvider);
        TeamRepository = new TeamRepository(DbContext, timeProvider);
        TermsOfUseRepository = new TermsOfUseRepository(DbContext, timeProvider);
        JoinInvitationRepository = new JoinInvitationRepository(DbContext, timeProvider);
        TagRepository = new TagRepository(DbContext, timeProvider);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public OrganizationDbContext DbContext { get; }

    public IUnitOfWork UnitOfWork => DbContext;
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
