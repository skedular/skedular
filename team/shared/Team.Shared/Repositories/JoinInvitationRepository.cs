using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
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
    Task<int> PendingInvitationsCountAsync(string inviteeId, IReadOnlyList<string> customerEmails, CancellationToken cancellationToken);
    Task<JoinInvitation?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<JoinInvitation>> GetByTeamIdAsync(string teamId, InvitationStatus status, CancellationToken cancellationToken);
    JoinInvitation Add(JoinInvitation joinInvitation);
    JoinInvitation Update(JoinInvitation joinInvitation);

    Task<(PaginatedInfo, IReadOnlyList<Edge<JoinInvitation>>, int)> GetPaginatedJoinInvitationsUntrackedAsync(
        PaginationInputParam paginationInputParam,
        JoinInvitationSearchCriteria searchCriteria,
        IEnumerable<JoinTeamInvitationOrder> orderByFields,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<JoinInvitation>> GetPendingInvitationsWithoutInviteeMatchingEmailsAsync(
        IReadOnlyList<string> emails,
        CancellationToken cancellationToken);
}

public static class JoinInvitationExtensions
{
    extension(IQueryable<JoinInvitation> originalQuery)
    {
        public IIncludableQueryable<JoinInvitation, Customer?> AddDependentObjects(bool isTracked) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
            .Include(query => query.Team)
            .Include(query => query.CreatedBy)
            .Include(query => query.Invitee);

        public IQueryable<JoinInvitation> AddSearchCriteria(JoinInvitationSearchCriteria searchCriteria)
        {
            if (!string.IsNullOrWhiteSpace(searchCriteria.InviteeId))
            {
                originalQuery = originalQuery.Where(item =>
                    item.Invitee != null && item.Invitee.Id == searchCriteria.InviteeId);
            }

            if (!string.IsNullOrWhiteSpace(searchCriteria.OrganizationUniqueCustomDomain))
            {
                originalQuery = originalQuery.Where(item =>
                    item.Team.Organization.CustomDomain != null &&
                    item.Team.Organization.CustomDomain == searchCriteria.OrganizationUniqueCustomDomain);
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
    }
}

public class JoinInvitationRepository(TeamDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<TeamDbContext, JoinInvitation>(dbContext, timeProvider), IJoinInvitationRepository
{
    public async Task<int> PendingInvitationsCountAsync(string inviteeId, IReadOnlyList<string> customerEmails,
        CancellationToken cancellationToken) =>
        await DbContext.JoinInvitation.CountAsync(
            query => query.Status == InvitationStatusConstants.Pending && ((query.Invitee != null && query.Invitee.Id == inviteeId) ||
                                                                           (query.Email != null && customerEmails.Contains(query.Email))),
            cancellationToken);

    public async Task<JoinInvitation?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.JoinInvitation
            .AddDependentObjects(true)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<IReadOnlyList<JoinInvitation>> GetByTeamIdAsync(string teamId, InvitationStatus status, CancellationToken cancellationToken) =>
        await DbContext.JoinInvitation
            .Where(query => query.Team.Id == teamId && query.Status == status.ToInvitationStatus())
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

    public async Task<(PaginatedInfo, IReadOnlyList<Edge<JoinInvitation>>, int)> GetPaginatedJoinInvitationsUntrackedAsync(
        PaginationInputParam paginationInputParam,
        JoinInvitationSearchCriteria searchCriteria,
        IEnumerable<JoinTeamInvitationOrder> orderByFields,
        CancellationToken cancellationToken) =>
        await DbContext.JoinInvitation
            .AddSearchCriteria(searchCriteria)
            .AddDependentObjects(false)
            .ToPaginatedAsync(paginationInputParam, GetPaginationFields(orderByFields), cancellationToken);

    public async Task<IReadOnlyList<JoinInvitation>> GetPendingInvitationsWithoutInviteeMatchingEmailsAsync(
        IReadOnlyList<string> emails,
        CancellationToken cancellationToken) =>
        await DbContext.JoinInvitation
            .Where(query =>
                query.Status == InvitationStatusConstants.Pending &&
                query.Invitee == null &&
                emails.Any(email => query.Email != null && EF.Functions.ILike(query.Email, email)))
            .ToListAsync(cancellationToken);

    private static List<KeysetPaginationField<JoinInvitation>> GetPaginationFields(IEnumerable<JoinTeamInvitationOrder> orderByFields)
    {
        if (!orderByFields.Any())
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
                JoinTeamInvitationOrderField.CreatedAt => KeysetPaginationField<JoinInvitation>.Create(
                    nameof(JoinInvitation.CreatedAt),
                    query => query.CreatedAt,
                    orderField.Direction),
                JoinTeamInvitationOrderField.Status => KeysetPaginationField<JoinInvitation>.Create(
                    nameof(JoinInvitation.Status),
                    query => query.Status,
                    orderField.Direction),
                _ => throw new ArgumentOutOfRangeException()
            })
            .ToList();
    }
}
