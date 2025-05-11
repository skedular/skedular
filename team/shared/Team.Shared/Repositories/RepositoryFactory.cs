using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Team.Shared.Database;
using TimeProvider = System.TimeProvider;

namespace Team.Shared.Repositories;

public interface IRepositoryFactory
{
    TeamDbContext DbContext { get; }
    IUnitOfWork UnitOfWork { get; }
    IBookingRepository BookingRepository { get; }
    ICustomerRepository CustomerRepository { get; }
    IIdentityRepository IdentityRepository { get; }
    IJoinInvitationRepository JoinInvitationRepository { get; }
    ITeamMemberRepository TeamMemberRepository { get; }
    ITeamRepository TeamRepository { get; }
    IOrganizationMemberRepository OrganizationMemberRepository { get; }
    IOrganizationRepository OrganizationRepository { get; }
    ILocationRepository LocationRepository { get; }
    IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }
}

public class RepositoryFactory : IRepositoryFactory, IDisposable
{
    private bool _disposed;

    public RepositoryFactory(IDbContextFactory<TeamDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        DbContext = dbContextFactory.CreateDbContext();

        BookingRepository = new BookingRepository(DbContext, timeProvider);
        CustomerRepository = new CustomerRepository(DbContext, timeProvider);
        IdentityRepository = new IdentityRepository(DbContext, timeProvider);
        JoinInvitationRepository = new JoinInvitationRepository(DbContext, timeProvider);
        TeamMemberRepository = new TeamMemberRepository(DbContext, timeProvider);
        TeamRepository = new TeamRepository(DbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(DbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(DbContext, timeProvider);
        LocationRepository = new LocationRepository(DbContext, timeProvider);
        OrganizationSsoSettingRepository = new OrganizationSsoSettingRepository(DbContext, timeProvider);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public TeamDbContext DbContext { get; }

    public IUnitOfWork UnitOfWork => DbContext;
    public IBookingRepository BookingRepository { get; }
    public ICustomerRepository CustomerRepository { get; }
    public IIdentityRepository IdentityRepository { get; }
    public IJoinInvitationRepository JoinInvitationRepository { get; }
    public ITeamMemberRepository TeamMemberRepository { get; }
    public ITeamRepository TeamRepository { get; }
    public IOrganizationMemberRepository OrganizationMemberRepository { get; }
    public IOrganizationRepository OrganizationRepository { get; }
    public ILocationRepository LocationRepository { get; }
    public IOrganizationSsoSettingRepository OrganizationSsoSettingRepository { get; }


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
