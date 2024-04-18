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
            return originalQuery.OrderBy(query => query.CreatedAt);
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
            });
    }

    public static IQueryable<Database.Entities.Notification> ApplyPaginationFilters(
        this IQueryable<Database.Entities.Notification> query,
        PaginationInputParam paginationInputParam,
        ICollection<NotificationOrder> orderByFields)
    {
        var orderByField = orderByFields.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(paginationInputParam.After))
        {
            query = orderByField?.Field switch
            {
                NotificationOrderField.EventRaisedAt => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.EventRaisedAt.CompareTo(paginationInputParam.After.FromCursorToDateTimeOffset()) > 0)
                    : query.Where(item =>
                        item.EventRaisedAt.CompareTo(paginationInputParam.After.FromCursorToDateTimeOffset()) < 0),
                NotificationOrderField.Type => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Type.CompareTo(
                            paginationInputParam.After.FromCursorToEnum<NotificationType>()) > 0)
                    : query.Where(item =>
                        item.Type.CompareTo(
                            paginationInputParam.After.FromCursorToEnum<NotificationType>()) < 0),
                null => query.Where(item =>
                    item.CreatedAt.CompareTo(paginationInputParam.After.FromCursorToDateTimeOffset()) > 0),
                _ => query.Where(item =>
                    item.CreatedAt.CompareTo(paginationInputParam.After.FromCursorToDateTimeOffset()) > 0)
            };
        }
        else if (!string.IsNullOrWhiteSpace(paginationInputParam.Before))
        {
            query = orderByField?.Field switch
            {
                NotificationOrderField.EventRaisedAt => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.EventRaisedAt.CompareTo(paginationInputParam.Before.FromCursorToDateTimeOffset()) < 0)
                    : query.Where(item =>
                        item.EventRaisedAt.CompareTo(paginationInputParam.Before.FromCursorToDateTimeOffset()) > 0),
                NotificationOrderField.Type => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Type.CompareTo(
                            paginationInputParam.Before.FromCursorToEnum<NotificationType>()) < 0)
                    : query.Where(item =>
                        item.Type.CompareTo(
                            paginationInputParam.Before.FromCursorToEnum<NotificationType>()) > 0),
                null => query.Where(item =>
                    item.CreatedAt.CompareTo(paginationInputParam.Before.FromCursorToDateTimeOffset()) < 0),
                _ => query.Where(item =>
                    item.CreatedAt.CompareTo(paginationInputParam.Before.FromCursorToDateTimeOffset()) < 0)
            };
        }

        if (paginationInputParam.First is not null)
        {
            query = query.Take(paginationInputParam.First.Value + 1);
        }
        else if (paginationInputParam.Last is not null)
        {
            query = query.Take(paginationInputParam.Last.Value + 1);
        }

        return query;
    }

    public static ICollection<Edge<Database.Entities.Notification>> ToEdges(
        this ICollection<Database.Entities.Notification> items,
        ICollection<NotificationOrder> orderByFields) =>
        items.Select(item => orderByFields.FirstOrDefault()?.Field switch
        {
            NotificationOrderField.EventRaisedAt => new Edge<Database.Entities.Notification>(
                item.EventRaisedAt.ToCursor(), item),
            NotificationOrderField.Type => new Edge<Database.Entities.Notification>(item.Type.ToCursor(), item),
            null => new Edge<Database.Entities.Notification>(item.CreatedAt.ToCursor(), item),
            _ => new Edge<Database.Entities.Notification>(item.CreatedAt.ToCursor(), item)
        }).ToList();
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
            CancellationToken cancellationToken)
    {
        var totalCount = await DbContext.Notification.AsQueryable().AddSearchCriteria(searchCriteria)
            .CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return (new PaginatedInfo(false, false, null, null), [], totalCount);
        }

        var (paginatedInfo, edges) = (await DbContext.Notification
                .AsQueryable()
                .AddSearchCriteria(searchCriteria)
                .AddSortingOrders(orderByFields)
                .ApplyPaginationFilters(paginationInputParam, orderByFields)
                .AddDependentObjects()
                .ToListAsync(cancellationToken))
            .ToEdges(orderByFields)
            .GetPaginatedInfo(paginationInputParam);
        return (paginatedInfo, edges, totalCount);
    }
}
