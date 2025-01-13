using System.Reflection;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Notification.Api.Mappers;
using Notification.Api.Services;
using Notification.Shared.Models;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Notification.Api.GraphQL;

[QueryType]
public class Query(IMapper mapper)
{
    [UseResolverScope]
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

    [UseResolverScope]
    public async Task<bool> NotificationCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);

    [UseResolverScope]
    public async Task<int> PendingInvitationsCountAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] INotificationService notificationService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken)
            ? await notificationService.PendingInvitationsCountAsync(cancellationToken)
            : 0;

    [UseResolverScope]
    public async Task<NotificationConnection?> MyNotificationsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        MyNotificationWhereInput where,
        NotificationOrderInput[]? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] INotificationService notificationService,
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var (paginatedInfo, edges, totalCount) =
            await notificationService.GetMyPaginatedNotificationsAsync(
                new PaginationInputParam(after, first, before, last),
                new NotificationSearchCriteria(where.OrganizationId),
                orderBy is null
                    ? []
                    : orderBy.Select(item =>
                    {
                        var direction = item.Direction == OrderDirection.Ascending
                            ? OrderDirection.Ascending
                            : OrderDirection.Descending;
                        return new NotificationOrder(direction, item.Field);
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
