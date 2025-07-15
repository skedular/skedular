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
    Task<ICollection<JoinInvitation>> GetPendingByEmailAsync(ICollection<string> emails, CancellationToken cancellationToken);
    JoinInvitation Add(JoinInvitation joinInvitation);
    JoinInvitation Update(JoinInvitation joinInvitation);
    JoinInvitation Remove(JoinInvitation joinInvitation);

    Task<(PaginatedInfo, ICollection<Edge<JoinInvitation>>, int)> GetPaginatedJoinInvitationsAsync(
        PaginationInputParam paginationInputParam,
        JoinInvitationSearchCriteria searchCriteria,
        ICollection<JoinTeamInvitationOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class JoinInvitationExtensions
{
    internal static IIncludableQueryable<JoinInvitation, Customer?> AddDependentObjects(
        this IQueryable<JoinInvitation> originalQuery) =>
        originalQuery
            .Include(query => query.Team)
            .Include(query => query.CreatedBy)
            .Include(query => query.Invitee);

    internal static IQueryable<JoinInvitation> AddSearchCriteria(this IQueryable<JoinInvitation> query, JoinInvitationSearchCriteria searchCriteria)
    {
        query = query.Where(item => !item.DeletedAt.HasValue);

        if (!string.IsNullOrWhiteSpace(searchCriteria.InviteeId))
        {
            query = query.Where(item =>
                item.Invitee != null && item.Invitee.Id == searchCriteria.InviteeId);
        }

        if (!string.IsNullOrWhiteSpace(searchCriteria.OrganizationId))
        {
            query = query.Where(item => item.Team.Organization.Id == searchCriteria.OrganizationId);
        }

        if (!string.IsNullOrWhiteSpace(searchCriteria.TeamId))
        {
            query = query.Where(item => item.Team.Id == searchCriteria.TeamId);
        }

        if (searchCriteria.Status is not null)
        {
            query = query.Where(item => item.Status == searchCriteria.Status.Value.ToInvitationStatus());
        }

        return query;
    }

    internal static IQueryable<JoinInvitation> AddSortingOrders(
        this IQueryable<JoinInvitation> originalQuery,
        ICollection<JoinTeamInvitationOrder> orderByFields)
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
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<JoinInvitation>> GetPendingByEmailAsync(ICollection<string> emails, CancellationToken cancellationToken) =>
        await DbContext.JoinInvitation
            .Where(query => !query.DeletedAt.HasValue && query.Status == InvitationStatusConstants.Pending &&
                            emails.Any(email => query.Invitee == null && query.Email != null && EF.Functions.ILike(query.Email, email)))
            .AddDependentObjects()
            .OrderBy(query => query.Id)
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

    public async Task<(PaginatedInfo, ICollection<Edge<JoinInvitation>>, int)> GetPaginatedJoinInvitationsAsync(
        PaginationInputParam paginationInputParam,
        JoinInvitationSearchCriteria searchCriteria,
        ICollection<JoinTeamInvitationOrder> orderByFields,
        CancellationToken cancellationToken) =>
        (await DbContext.JoinInvitation
            .AddSearchCriteria(searchCriteria)
            .AddSortingOrders(orderByFields)
            .AddDependentObjects()
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);
}
