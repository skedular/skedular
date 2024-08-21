using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using TimeProvider = System.TimeProvider;

namespace Organization.Shared.Repositories;

public interface IRepositoryFactory
{
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
    ITeamRepository TeamRepository { get; }
    ITermsOfUseRepository TermsOfUseRepository { get; }
    IJoinInvitationRepository JoinInvitationRepository { get; }
}

public class RepositoryFactory : IRepositoryFactory, IAsyncDisposable
{
    private readonly OrganizationDbContext _dbContext;

    public RepositoryFactory(IDbContextFactory<OrganizationDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        _dbContext = dbContextFactory.CreateDbContext();

        AzureInstallStateUserIdLookupRepository = new AzureInstallStateUserIdLookupRepository(_dbContext, timeProvider);
        AzureTenantRepository = new AzureTenantRepository(_dbContext, timeProvider);
        AzureTenantMemberRepository = new AzureTenantMemberRepository(_dbContext, timeProvider);
        BookingRepository = new BookingRepository(_dbContext, timeProvider);
        CustomerRepository = new CustomerRepository(_dbContext, timeProvider);
        DailyMemberCountRecordingRepository = new DailyMemberCountRecordingRepository(_dbContext, timeProvider);
        IdentityRepository = new IdentityRepository(_dbContext, timeProvider);
        IndustryMainCategoryRepository = new IndustryMainCategoryRepository(_dbContext);
        IndustrySubCategoryRepository = new IndustrySubCategoryRepository(_dbContext);
        LocationRepository = new LocationRepository(_dbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(_dbContext, timeProvider);
        OrganizationOfferingActiveMemberRepository =
            new OrganizationOfferingActiveMemberRepository(_dbContext, timeProvider);
        OrganizationOfferingRepository = new OrganizationOfferingRepository(_dbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(_dbContext, timeProvider);
        TeamRepository = new TeamRepository(_dbContext, timeProvider);
        TermsOfUseRepository = new TermsOfUseRepository(_dbContext);
        JoinInvitationRepository = new JoinInvitationRepository(_dbContext, timeProvider);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

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
    public ITeamRepository TeamRepository { get; }
    public ITermsOfUseRepository TermsOfUseRepository { get; }
    public IJoinInvitationRepository JoinInvitationRepository { get; }

    protected virtual async ValueTask DisposeAsyncCore() => await _dbContext.DisposeAsync();
}
