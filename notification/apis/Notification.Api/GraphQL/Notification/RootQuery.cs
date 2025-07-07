using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Notification.Api.Mappers;
using Notification.Api.Services;
using Notification.Shared.Models;

namespace Notification.Api.GraphQL.Notification;

[QueryType]
public class RootQuery(IMapper mapper)
{
    [UseResolverScope]
    public async Task<int> PendingInvitationsCountAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] INotificationService notificationService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken)
            ? await notificationService.PendingInvitationsCountAsync(cancellationToken)
            : 0;

    [UseResolverScope]
    public async Task<Connection<NotificationEdge>> MyNotificationsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        MyNotificationWhereInput where,
        IEnumerable<NotificationOrderInput>? orderBy,
        [Service] INotificationService notificationService,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await notificationService.GetMyPaginatedNotificationsAsync(
            new PaginationInputParam(after, first, before, last),
            new NotificationSearchCriteria(where.OrganizationId),
            orderBy.ToSafeCollection().Select(item => new NotificationOrder(item.Direction, item.Field)).ToList(),
            cancellationToken);

        return new Connection<NotificationEdge>
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor,
                EndCursor = paginatedInfo.EndCursor
            },
            Edges = edges.Select(mapper.MapTo),
            TotalCount = totalCount
        };
    }
}
