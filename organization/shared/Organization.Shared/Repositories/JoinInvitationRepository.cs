using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization.Shared.Database;
using Organization.Shared.Models;
using Customer = Organization.Shared.Database.Entities.Customer;
using JoinInvitation = Organization.Shared.Database.Entities.JoinInvitation;

namespace Organization.Shared.Repositories;

public interface IJoinInvitationRepository : IRepository<JoinInvitation>
{
    Task<int> PendingInvitationsCountAsync(string inviteeId, CancellationToken cancellationToken);
    Task<JoinInvitation?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<JoinInvitation?> GetByOrganizationInviterInviteeIdAsync(
        string organizationId,
        string inviterId,
        string inviteeId,
        CancellationToken cancellationToken);

    Task<ICollection<JoinInvitation>> GetByOrganizationIdOrOrganizationUniqueAlphanumericNameAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        InvitationStatus status,
        CancellationToken cancellationToken);

    JoinInvitation Add(JoinInvitation joinInvitation);
    JoinInvitation Update(JoinInvitation joinInvitation);

    Task<(PaginatedInfo, ICollection<Edge<JoinInvitation>>, int)> GetPaginatedJoinInvitationsUntrackedAsync(
        PaginationInputParam paginationInputParam,
        JoinInvitationSearchCriteria searchCriteria,
        ICollection<JoinOrganizationInvitationOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class JoinInvitationExtensions
{
    extension(IQueryable<JoinInvitation> originalQuery)
    {
        internal IIncludableQueryable<JoinInvitation, Customer?> AddDependentObjects(bool isTracked) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
            .Include(query => query.Organization)
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
                    item.Organization.UniqueAlphanumericName != null &&
                    item.Organization.UniqueAlphanumericName == searchCriteria.OrganizationUniqueAlphanumericName);
            }

            if (searchCriteria.Status is not null)
            {
                originalQuery = originalQuery.Where(item => item.Status == searchCriteria.Status.Value.ToInvitationStatus());
            }

            return originalQuery;
        }

        internal IQueryable<JoinInvitation> AddSortingOrders(ICollection<JoinOrganizationInvitationOrder> orderByFields)
        {
            if (orderByFields.Count == 0)
            {
                return originalQuery.OrderBy(query => query.CreatedBy).ThenBy(query => query.Id);
            }

            var orderByField = orderByFields.First();
            return orderByFields.Skip(1).Aggregate(orderByField.Field switch
            {
                JoinOrganizationInvitationOrderField.CreatedAt => orderByField.Direction == OrderDirection.Ascending
                    ? originalQuery.OrderBy(x => x.CreatedAt)
                    : originalQuery.OrderByDescending(x => x.CreatedAt),
                JoinOrganizationInvitationOrderField.Status => orderByField.Direction == OrderDirection.Ascending
                    ? originalQuery.OrderBy(x => x.Status)
                    : originalQuery.OrderByDescending(x => x.Status),
                _ => throw new ArgumentOutOfRangeException()
            }, (query, orderField) =>
                orderField.Field switch
                {
                    JoinOrganizationInvitationOrderField.CreatedAt => orderField.Direction == OrderDirection.Ascending
                        ? query.ThenBy(x => x.CreatedAt)
                        : query.ThenByDescending(x => x.CreatedAt),
                    JoinOrganizationInvitationOrderField.Status => orderField.Direction == OrderDirection.Ascending
                        ? query.ThenBy(x => x.Status)
                        : query.ThenByDescending(x => x.Status),
                    _ => throw new ArgumentOutOfRangeException()
                }).ThenBy(query => query.Id);
        }
    }
}

public class JoinInvitationRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, JoinInvitation>(dbContext, timeProvider), IJoinInvitationRepository
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

    public async Task<JoinInvitation?> GetByOrganizationInviterInviteeIdAsync(
        string organizationId,
        string inviterId,
        string inviteeId,
        CancellationToken cancellationToken)
    {
        // Build the query with eager loading and tracking enabled
        var query = DbContext.JoinInvitation
            .AddDependentObjects(true)
            .Where(query => query.Organization.Id == organizationId
                            && query.CreatedBy.Id == inviterId
                            && query.Invitee != null && query.Invitee.Id == inviteeId
                            && query.Status == InvitationStatus.Pending.ToInvitationStatus());

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ICollection<JoinInvitation>> GetByOrganizationIdOrOrganizationUniqueAlphanumericNameAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        InvitationStatus status,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(organizationId))
        {
            return await DbContext.JoinInvitation
                .Where(query => !query.DeletedAt.HasValue && query.Organization.Id == organizationId && query.Status == status.ToInvitationStatus())
                .AddDependentObjects(true)
                .ToListAsync(cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(organizationUniqueAlphanumericName))
        {
            return await DbContext.JoinInvitation
                .Where(query => !query.DeletedAt.HasValue && query.Organization.UniqueAlphanumericName == organizationUniqueAlphanumericName &&
                                query.Status == status.ToInvitationStatus())
                .AddDependentObjects(true)
                .ToListAsync(cancellationToken);
        }

        throw new InvalidOperationException("Either id or uniqueAlphanumericName must be provided.");
    }

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

    public async Task<(PaginatedInfo, ICollection<Edge<JoinInvitation>>, int)> GetPaginatedJoinInvitationsUntrackedAsync(
        PaginationInputParam paginationInputParam,
        JoinInvitationSearchCriteria searchCriteria,
        ICollection<JoinOrganizationInvitationOrder> orderByFields,
        CancellationToken cancellationToken) =>
        (await DbContext.JoinInvitation
            .AddSearchCriteria(searchCriteria)
            .AddSortingOrders(orderByFields)
            .AddDependentObjects(false)
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);
}
