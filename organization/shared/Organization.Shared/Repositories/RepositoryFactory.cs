using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using TimeProvider = System.TimeProvider;

namespace Organization.Shared.Repositories;

public interface IRepositoryFactory
{
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
    private readonly OrganizationDbContext _dbContext;
    private bool _disposed;

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
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public IUnitOfWork UnitOfWork => _dbContext;
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
            _dbContext.Dispose();
        }

        _disposed = true;
    }
}
