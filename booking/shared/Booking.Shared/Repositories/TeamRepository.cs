using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface ITeamRepository : IRepository<Team>
{
    Task<Team> UpsertNakedAsync(string id, Organization? organization, CancellationToken cancellationToken);
    Task<Team?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Team Add(Team team);
    Team Update(Team team);
    Team Remove(Team team);
    Task<ICollection<Team>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken);
}

public class TeamRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, Team>(dbContext), ITeamRepository
{
    public async Task<Team>
        UpsertNakedAsync(string id, Organization? organization, CancellationToken cancellationToken)
    {
        var existing = await GetByIdAsync(id, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var now = timeProvider.GetUtcNow();
        return DbContext.Team.Add(new Team { Id = id, CreatedAt = now, Organization = organization }).Entity;
    }

    public async Task<Team?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Team
            .Where(query => query.Id == id)
            .Include(query =>
                query.TeamMembers.Where(teamMember => !teamMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .Include(query => query.Organization)
            .ThenInclude(query =>
                query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .Include(query => query.DefaultedByCustomers)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public Team Add(Team team)
    {
        var now = timeProvider.GetUtcNow();
        team.CreatedAt = now;
        return DbContext.Team.Add(team).Entity;
    }

    public Team Update(Team team)
    {
        var now = timeProvider.GetUtcNow();
        team.ModifiedAt = now;
        return DbContext.Team.Update(team).Entity;
    }

    public Team Remove(Team team)
    {
        var now = timeProvider.GetUtcNow();
        team.DeletedAt = now;
        return DbContext.Team.Update(team).Entity;
    }

    public async Task<ICollection<Team>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken) =>
        await DbContext.Team
            .Where(query => !query.DeletedAt.HasValue &&
                            ((query.Organization == null && query.TeamMembers.Any(teamMember =>
                                 !teamMember.DeletedAt.HasValue && teamMember.Customer.Id == customerId)) ||
                             (query.Organization != null && query.Organization.OrganizationMembers.Any(
                                 organizationMember =>
                                     !organizationMember.DeletedAt.HasValue &&
                                     organizationMember.Customer.Id == customerId))))
            .ToListAsync(cancellationToken);
}
