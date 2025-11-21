using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Team.Shared.Database;
using Team.Shared.Models;
using Customer = Team.Shared.Database.Entities.Customer;
using JoinInvitation = Team.Shared.Database.Entities.JoinInvitation;

namespace Team.Shared.Repositories;

public interface IJoinInvitationRepository : IRepository<JoinInvitation>
{
    Task<int> PendingInvitationsCountAsync(string inviteeId, CancellationToken cancellationToken);
    Task<JoinInvitation?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<JoinInvitation>> GetByTeamIdAsync(string teamId, InvitationStatus status, CancellationToken cancellationToken);
    JoinInvitation Add(JoinInvitation joinInvitation);
    JoinInvitation Update(JoinInvitation joinInvitation);
    JoinInvitation Remove(JoinInvitation joinInvitation);

    Task<(PaginatedInfo, ICollection<Edge<JoinInvitation>>, int)> GetPaginatedJoinInvitationsUntrackedAsync(
        PaginationInputParam paginationInputParam,
        JoinInvitationSearchCriteria searchCriteria,
        ICollection<JoinTeamInvitationOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class JoinInvitationExtensions
{
    extension(IQueryable<JoinInvitation> originalQuery)
    {
        internal IIncludableQueryable<JoinInvitation, Customer?> AddDependentObjects(bool isTracked) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
            .Include(query => query.Team)
            .Include(query => query.CreatedBy)
            .Include(query => query.Invitee);

        internal IQueryable<JoinInvitation> AddSearchCriteria(JoinInvitationSearchCriteria searchCriteria)
        {
            originalQuery = originalQuery.Where(item => !item.DeletedAt.HasValue);

            if (!string.IsNullOrWhiteSpace(searchCriteria.InviteeId))
            {
                originalQuery = originalQuery.Where(item =>
                    item.Invitee != null && item.Invitee.Id == searchCriteria.InviteeId);
            }

            if (!string.IsNullOrWhiteSpace(searchCriteria.OrganizationUniqueAlphanumericName))
            {
                originalQuery = originalQuery.Where(item =>
                    item.Team.Organization.UniqueAlphanumericName != null &&
                    item.Team.Organization.UniqueAlphanumericName == searchCriteria.OrganizationUniqueAlphanumericName);
            }

            if (!string.IsNullOrWhiteSpace(searchCriteria.TeamId))
            {
                originalQuery = originalQuery.Where(item => item.Team.Id == searchCriteria.TeamId);
            }

            if (searchCriteria.Status is not null)
            {
                originalQuery = originalQuery.Where(item => item.Status == searchCriteria.Status.Value.ToInvitationStatus());
            }

            return originalQuery;
        }

        internal IQueryable<JoinInvitation> AddSortingOrders(ICollection<JoinTeamInvitationOrder> orderByFields)
        {
            if (orderByFields.Count == 0)
            {
                return originalQuery.OrderBy(query => query.CreatedBy).ThenBy(query => query.Id);
            }

            var orderByField = orderByFields.First();
            return orderByFields.Skip(1).Aggregate(orderByField.Field switch
            {
                JoinTeamInvitationOrderField.CreatedAt => orderByField.Direction == OrderDirection.Ascending
                    ? originalQuery.OrderBy(x => x.CreatedAt)
                    : originalQuery.OrderByDescending(x => x.CreatedAt),
                JoinTeamInvitationOrderField.Status => orderByField.Direction == OrderDirection.Ascending
                    ? originalQuery.OrderBy(x => x.Status)
                    : originalQuery.OrderByDescending(x => x.Status),
                _ => throw new ArgumentOutOfRangeException()
            }, (query, orderField) =>
                orderField.Field switch
                {
                    JoinTeamInvitationOrderField.CreatedAt => orderField.Direction == OrderDirection.Ascending
                        ? query.ThenBy(x => x.CreatedAt)
                        : query.ThenByDescending(x => x.CreatedAt),
                    JoinTeamInvitationOrderField.Status => orderField.Direction == OrderDirection.Ascending
                        ? query.ThenBy(x => x.Status)
                        : query.ThenByDescending(x => x.Status),
                    _ => throw new ArgumentOutOfRangeException()
                }).ThenBy(query => query.Id);
        }
    }
}

public class JoinInvitationRepository(TeamDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<TeamDbContext, JoinInvitation>(dbContext, timeProvider), IJoinInvitationRepository
{
    public async Task<int> PendingInvitationsCountAsync(string inviteeId, CancellationToken cancellationToken) =>
        await DbContext.JoinInvitation
            .CountAsync(
                query => query.DeletedAt == null && query.Invitee != null && query.Invitee.Id == inviteeId,
                cancellationToken);

    public async Task<JoinInvitation?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.JoinInvitation
            .AddDependentObjects(true)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<JoinInvitation>> GetByTeamIdAsync(string teamId, InvitationStatus status, CancellationToken cancellationToken) =>
        await DbContext.JoinInvitation
            .Where(query => !query.DeletedAt.HasValue && query.Team.Id == teamId && query.Status == status.ToInvitationStatus())
            .AddDependentObjects(true)
            .ToListAsync(cancellationToken);

    public JoinInvitation Add(JoinInvitation joinInvitation)
    {
        var now = TimeProvider.GetUtcNow();
        joinInvitation.CreatedAt = now;
        return DbContext.JoinInvitation.Add(joinInvitation).Entity;
    }

    public JoinInvitation Update(JoinInvitation joinInvitation)
    {
        var now = TimeProvider.GetUtcNow();
        joinInvitation.ModifiedAt = now;
        return DbContext.JoinInvitation.Update(joinInvitation).Entity;
    }

    public JoinInvitation Remove(JoinInvitation joinInvitation)
    {
        var now = TimeProvider.GetUtcNow();
        joinInvitation.DeletedAt = now;
        return DbContext.JoinInvitation.Update(joinInvitation).Entity;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<JoinInvitation>>, int)> GetPaginatedJoinInvitationsUntrackedAsync(
        PaginationInputParam paginationInputParam,
        JoinInvitationSearchCriteria searchCriteria,
        ICollection<JoinTeamInvitationOrder> orderByFields,
        CancellationToken cancellationToken) =>
        (await DbContext.JoinInvitation
            .AddSearchCriteria(searchCriteria)
            .AddSortingOrders(orderByFields)
            .AddDependentObjects(false)
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);
}
