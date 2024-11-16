using System.Reflection;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Notification.Api.Mappers;
using Notification.Api.Services;
using Notification.Shared.Models;

namespace Notification.Api.GraphQL;

public class NotificationQuery
{
    [UseServiceScope]
    public Version NotificationVersion()
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        };
    }

    [UseServiceScope]
    public async Task<bool> NotificationCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);

    [UseServiceScope]
    public async Task<NotificationConnection?> MyNotificationsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        NotificationOrderInput[]? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] INotificationService notificationService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var (paginatedInfo, edges, totalCount) =
            await notificationService.GetMyPaginatedNotificationsAsync(
                new PaginationInputParam(after, first, before, last),
                new NotificationSearchCriteria(),
                orderBy is null
                    ? []
                    : orderBy.Select(item =>
                    {
                        var direction = item.Direction == OrderDirection.Ascending
                            ? Enterprise.Shared.Pagination.OrderDirection.Ascending
                            : Enterprise.Shared.Pagination.OrderDirection.Descending;
                        var field = item.Field switch
                        {
                            NotificationOrderField.eventRaisedAt => Shared.Models.NotificationOrderField.EventRaisedAt,
                            NotificationOrderField.notificationType => Shared.Models.NotificationOrderField.Type,
                            _ => throw new ArgumentOutOfRangeException()
                        };

                        return new NotificationOrder(direction, field);
                    }).ToList(),
                cancellationToken);

        return new NotificationConnection
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor,
                EndCursor = paginatedInfo.EndCursor
            },
            Edges = edges.Select(mapper.MapTo).ToArray(),
            TotalCount = totalCount
        };
    }
}
