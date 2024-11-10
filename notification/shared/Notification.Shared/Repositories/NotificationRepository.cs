using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Notification.Shared.Database;
using Notification.Shared.Models;
using NotificationOrder = Notification.Shared.Models.NotificationOrder;
using NotificationOrderField = Notification.Shared.Models.NotificationOrderField;
using Team = Notification.Shared.Database.Entities.Team;

namespace Notification.Shared.Repositories;

public interface INotificationRepository : IRepository<Database.Entities.Notification>
{
    Task<Database.Entities.Notification?> GetBySourceIdAsync(string sourceId, CancellationToken cancellationToken);
    Database.Entities.Notification Add(Database.Entities.Notification notification);
    Database.Entities.Notification Update(Database.Entities.Notification notification);
    Database.Entities.Notification Remove(Database.Entities.Notification notification);

    Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Notification>>, int)>
        GetPaginatedNotificationsAsync(
            PaginationInputParam paginationInputParam,
            NotificationSearchCriteria searchCriteria,
            ICollection<NotificationOrder> orderByFields,
            CancellationToken cancellationToken);
}

internal static class NotificationExtensions
{
    internal static IIncludableQueryable<Database.Entities.Notification, Team> AddDependentObjects(
        this IQueryable<Database.Entities.Notification> originalQuery) =>
        originalQuery
            .Include(query => query.InvitedBy)
            .ThenInclude(query => query.Identities)
            .Include(query => query.Invitee)
            .ThenInclude(query => query.Identities)
            .Include(query => query.Organization)
            .Include(query => query.Location)
            .Include(query => query.Team);

    internal static IQueryable<Database.Entities.Notification> AddSearchCriteria(
        this IQueryable<Database.Entities.Notification> query,
        NotificationSearchCriteria searchCriteria)
    {
        query = query.Where(item => !item.DeletedAt.HasValue);

        if (!string.IsNullOrWhiteSpace(searchCriteria.InviteeId))
        {
            query = query.Where(item =>
                item.Invitee != null && item.Invitee.Id == searchCriteria.InviteeId);
        }

        return query;
    }

    internal static IQueryable<Database.Entities.Notification> AddSortingOrders(
        this IQueryable<Database.Entities.Notification> originalQuery,
        ICollection<NotificationOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return originalQuery.OrderBy(query => query.EventRaisedAt).ThenBy(query => query.Id);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            NotificationOrderField.EventRaisedAt => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.EventRaisedAt)
                : originalQuery.OrderByDescending(x => x.EventRaisedAt),
            NotificationOrderField.Type => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Type)
                : originalQuery.OrderByDescending(x => x.Type),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                NotificationOrderField.EventRaisedAt => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.EventRaisedAt)
                    : query.ThenByDescending(x => x.EventRaisedAt),
                NotificationOrderField.Type => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Type)
                    : query.ThenByDescending(x => x.Type),
                _ => throw new ArgumentOutOfRangeException()
            }).ThenBy(query => query.Id);
    }
}

public class NotificationRepository(NotificationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<NotificationDbContext, Database.Entities.Notification>(dbContext), INotificationRepository
{
    public async Task<Database.Entities.Notification?> GetBySourceIdAsync(string sourceId,
        CancellationToken cancellationToken) =>
        await DbContext.Notification
            .Where(query => query.SourceId == sourceId)
            .AddDependentObjects()
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public Database.Entities.Notification Add(Database.Entities.Notification notification)
    {
        var now = timeProvider.GetUtcNow();
        notification.CreatedAt = now;
        return DbContext.Notification.Add(notification).Entity;
    }

    public Database.Entities.Notification Update(Database.Entities.Notification notification)
    {
        var now = timeProvider.GetUtcNow();
        notification.ModifiedAt = now;
        return DbContext.Notification.Update(notification).Entity;
    }

    public Database.Entities.Notification Remove(Database.Entities.Notification notification)
    {
        var now = timeProvider.GetUtcNow();
        notification.DeletedAt = now;
        return DbContext.Notification.Update(notification).Entity;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Notification>>, int)>
        GetPaginatedNotificationsAsync(
            PaginationInputParam paginationInputParam,
            NotificationSearchCriteria searchCriteria,
            ICollection<NotificationOrder> orderByFields,
            CancellationToken cancellationToken) =>
        (await DbContext.Notification
            .AddSearchCriteria(searchCriteria)
            .AddSortingOrders(orderByFields)
            .AddDependentObjects()
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);
}
