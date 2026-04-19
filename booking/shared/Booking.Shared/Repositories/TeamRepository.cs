using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Booking.Shared.Repositories;

public interface ITeamRepository : IRepository<Team>
{
    Task<Team> UpsertNakedAsync(string id, Organization? organization, CancellationToken cancellationToken);
    Task<Team?> GetByIdAsync(string id, bool includeDeletedTeamMembers, CancellationToken cancellationToken);
    Task<Team?> GetByIdUntrackedAsync(string id, bool includeDeletedTeamMembers, CancellationToken cancellationToken);
    Task<ICollection<Team>> GetByIdsAsync(ICollection<string> ids, bool includeDeletedTeamMembers, CancellationToken cancellationToken);
    Task<ICollection<Team>> GetActiveByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Team Update(Team team);
    Team Remove(Team team);
    Task<ICollection<Team>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken);
}

internal static class TeamExtensions
{
    extension(IQueryable<Team> originalQuery)
    {
        internal IIncludableQueryable<Team, Customer> AddDependentObjects(
            bool isTracked,
            bool includeDeletedTeamMembers) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
            .Include(query => query.TeamMembers.Where(teamMember => includeDeletedTeamMembers || !teamMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .Include(query => query.Organization)
            .ThenInclude(query => query!.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer);
    }
}

public class TeamRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, Team>(dbContext, timeProvider), ITeamRepository
{
    public async Task<Team> UpsertNakedAsync(string id, Organization? organization, CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Organization>(id, organization, cancellationToken);

        return (await GetByIdAsync(id, true, cancellationToken))!;
    }

    public async Task<Team?> GetByIdAsync(string id, bool includeDeletedTeamMembers, CancellationToken cancellationToken) =>
        await DbContext.Team
            .AddDependentObjects(true, includeDeletedTeamMembers)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<Team?> GetByIdUntrackedAsync(string id, bool includeDeletedTeamMembers, CancellationToken cancellationToken) =>
        await DbContext.Team
            .AddDependentObjects(false, includeDeletedTeamMembers)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<Team>> GetByIdsAsync(
        ICollection<string> ids,
        bool includeDeletedTeamMembers,
        CancellationToken cancellationToken) =>
        await DbContext.Team
            .Where(query => ids.Contains(query.Id))
            .AddDependentObjects(true, includeDeletedTeamMembers)
            .ToListAsync(cancellationToken);

    /// <summary>
    ///     Returns the active teams for the supplied identifiers with only the organization relationship loaded.
    /// </summary>
    /// <param name="ids">The team identifiers to resolve.</param>
    /// <param name="cancellationToken">The cancellation token for the database query.</param>
    /// <returns>The non-deleted teams that match the supplied identifiers.</returns>
    /// <remarks>
    ///     This lightweight authorization lookup replaces the heavier specification path and intentionally loads only the organization data needed by
    ///     booking access checks.
    /// </remarks>
    public async Task<ICollection<Team>> GetActiveByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        return await DbContext.Team
            .Where(query => !query.DeletedAt.HasValue && ids.Contains(query.Id))
            .AsNoTrackingWithIdentityResolution()
            .Include(query => query.Organization)
            .ToListAsync(cancellationToken);
    }

    public Team Update(Team team)
    {
        var now = TimeProvider.GetUtcNow();
        team.ModifiedAt = now;
        return DbContext.Team.Update(team).Entity;
    }

    public Team Remove(Team team)
    {
        var now = TimeProvider.GetUtcNow();
        team.DeletedAt = now;
        return DbContext.Team.Update(team).Entity;
    }

    public async Task<ICollection<Team>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken) =>
        await DbContext.Team
            .Where(query => !query.DeletedAt.HasValue && query.Organization != null && !query.Organization.DeletedAt.HasValue &&
                            query.Organization.OrganizationMembers.Any(organizationMember =>
                                !organizationMember.DeletedAt.HasValue && organizationMember.Customer.Id == customerId))
            .ToListAsync(cancellationToken);
}
