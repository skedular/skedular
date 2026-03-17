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
    Task<int> PendingInvitationsCountAsync(string inviteeId, ICollection<string> customerEmails, CancellationToken cancellationToken);
    Task<JoinInvitation?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<ICollection<JoinInvitation>> GetByOrganizationIdOrOrganizationCustomDomainAsync(
        string? organizationId,
        string? organizationCustomDomain,
        InvitationStatus status,
        CancellationToken cancellationToken);

    JoinInvitation Add(JoinInvitation joinInvitation);
    JoinInvitation Update(JoinInvitation joinInvitation);

    Task<(PaginatedInfo, ICollection<Edge<JoinInvitation>>, int)> GetPaginatedJoinInvitationsUntrackedAsync(
        PaginationInputParam paginationInputParam,
        JoinInvitationSearchCriteria searchCriteria,
        ICollection<JoinOrganizationInvitationOrder> orderByFields,
        CancellationToken cancellationToken);

    Task<ICollection<JoinInvitation>> GetPendingInvitationsWithoutInviteeMatchingEmailsAsync(
        ICollection<string> emails,
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
            if (!string.IsNullOrWhiteSpace(searchCriteria.InviteeId) ||
                (searchCriteria.CustomerEmails != null && searchCriteria.CustomerEmails.Count != 0))
            {
                originalQuery = originalQuery.Where(item =>
                    (item.Invitee != null && searchCriteria.InviteeId != null && item.Invitee.Id == searchCriteria.InviteeId) ||
                    (item.Email != null && searchCriteria.CustomerEmails != null && searchCriteria.CustomerEmails.Contains(item.Email)));
            }

            if (!string.IsNullOrWhiteSpace(searchCriteria.OrganizationCustomDomain))
            {
                originalQuery = originalQuery.Where(item =>
                    item.Organization.CustomDomain != null &&
                    item.Organization.CustomDomain == searchCriteria.OrganizationCustomDomain);
            }

            if (searchCriteria.Status is not null)
            {
                originalQuery = originalQuery.Where(item => item.Status == searchCriteria.Status.Value.ToInvitationStatus());
            }

            return originalQuery;
        }
    }
}

public class JoinInvitationRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, JoinInvitation>(dbContext, timeProvider), IJoinInvitationRepository
{
    public async Task<int> PendingInvitationsCountAsync(string inviteeId, ICollection<string> customerEmails, CancellationToken cancellationToken) =>
        await DbContext.JoinInvitation.CountAsync(
            query => query.Status == InvitationStatusConstants.Pending && ((query.Invitee != null && query.Invitee.Id == inviteeId) ||
                                                                           (query.Email != null && customerEmails.Contains(query.Email))),
            cancellationToken);

    public async Task<JoinInvitation?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.JoinInvitation
            .AddDependentObjects(true)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<JoinInvitation>> GetByOrganizationIdOrOrganizationCustomDomainAsync(
        string? organizationId,
        string? organizationCustomDomain,
        InvitationStatus status,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(organizationId))
        {
            return await DbContext.JoinInvitation
                .Where(query => query.Organization.Id == organizationId && query.Status == status.ToInvitationStatus())
                .AddDependentObjects(true)
                .ToListAsync(cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(organizationCustomDomain))
        {
            return await DbContext.JoinInvitation
                .Where(query => query.Organization.CustomDomain == organizationCustomDomain &&
                                query.Status == status.ToInvitationStatus())
                .AddDependentObjects(true)
                .ToListAsync(cancellationToken);
        }

        throw new InvalidOperationException("Either id or customDomain must be provided.");
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
        await DbContext.JoinInvitation
            .AddSearchCriteria(searchCriteria)
            .AddDependentObjects(false)
            .ToPaginatedAsync(paginationInputParam, GetPaginationFields(orderByFields), cancellationToken);

    public async Task<ICollection<JoinInvitation>> GetPendingInvitationsWithoutInviteeMatchingEmailsAsync(
        ICollection<string> emails,
        CancellationToken cancellationToken) =>
        await DbContext.JoinInvitation
            .Where(query =>
                query.Status == InvitationStatusConstants.Pending &&
                query.Invitee == null &&
                emails.Any(email => query.Email != null && EF.Functions.ILike(query.Email, email)))
            .ToListAsync(cancellationToken);

    private static List<KeysetPaginationField<JoinInvitation>> GetPaginationFields(ICollection<JoinOrganizationInvitationOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return
            [
                KeysetPaginationField<JoinInvitation>.Create(
                    $"{nameof(JoinInvitation.CreatedBy)}{nameof(Customer.Id)}",
                    query => query.CreatedBy.Id,
                    OrderDirection.Ascending)
            ];
        }

        return orderByFields.Select(orderField => orderField.Field switch
            {
                JoinOrganizationInvitationOrderField.CreatedAt => KeysetPaginationField<JoinInvitation>.Create(
                    nameof(JoinInvitation.CreatedAt),
                    query => query.CreatedAt,
                    orderField.Direction),
                JoinOrganizationInvitationOrderField.Status => KeysetPaginationField<JoinInvitation>.Create(
                    nameof(JoinInvitation.Status),
                    query => query.Status,
                    orderField.Direction),
                _ => throw new ArgumentOutOfRangeException()
            })
            .ToList();
    }
}
