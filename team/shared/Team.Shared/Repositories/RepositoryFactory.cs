using Microsoft.EntityFrameworkCore;
using Team.Shared.Database;
using TimeProvider = System.TimeProvider;

namespace Team.Shared.Repositories;

public interface IRepositoryFactory
{
    IBookingRepository BookingRepository { get; }
    ICustomerRepository CustomerRepository { get; }
    IIdentityRepository IdentityRepository { get; }
    IJoinInvitationRepository JoinInvitationRepository { get; }
    ITeamMemberRepository TeamMemberRepository { get; }
    ITeamRepository TeamRepository { get; }
    IOrganizationMemberRepository OrganizationMemberRepository { get; }
    IOrganizationRepository OrganizationRepository { get; }
    ILocationRepository LocationRepository { get; }
}

public class RepositoryFactory : IRepositoryFactory, IDisposable
{
    private readonly TeamDbContext _dbContext;
    private bool _disposed;

    public RepositoryFactory(IDbContextFactory<TeamDbContext> dbContextFactory, TimeProvider timeProvider)
    {
        _dbContext = dbContextFactory.CreateDbContext();

        BookingRepository = new BookingRepository(_dbContext, timeProvider);
        CustomerRepository = new CustomerRepository(_dbContext, timeProvider);
        IdentityRepository = new IdentityRepository(_dbContext, timeProvider);
        JoinInvitationRepository = new JoinInvitationRepository(_dbContext, timeProvider);
        TeamMemberRepository = new TeamMemberRepository(_dbContext, timeProvider);
        TeamRepository = new TeamRepository(_dbContext, timeProvider);
        OrganizationMemberRepository = new OrganizationMemberRepository(_dbContext, timeProvider);
        OrganizationRepository = new OrganizationRepository(_dbContext, timeProvider);
        LocationRepository = new LocationRepository(_dbContext, timeProvider);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public IBookingRepository BookingRepository { get; }
    public ICustomerRepository CustomerRepository { get; }
    public IIdentityRepository IdentityRepository { get; }
    public IJoinInvitationRepository JoinInvitationRepository { get; }
    public ITeamMemberRepository TeamMemberRepository { get; }
    public ITeamRepository TeamRepository { get; }
    public IOrganizationMemberRepository OrganizationMemberRepository { get; }
    public IOrganizationRepository OrganizationRepository { get; }
    public ILocationRepository LocationRepository { get; }


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
