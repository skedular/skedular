using System.Reflection;
using Enterprise.Shared.Context;
using Enterprise.Shared.Pagination;
using Notification.Api.Mappers;
using Notification.Api.Services;
using Notification.Shared.Models;

namespace Notification.Api.GraphQL;

public class NotificationQuery(IServiceProvider serviceProvider, IMapper mapper)
{
    public Task<Version> NotificationVersionAsync(CancellationToken cancellationToken)
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return Task.FromResult(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }

    public async Task<bool> NotificationCustomerRecordSyncedAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        return await service.DoesCustomerExistAsync(cancellationToken);
    }

    public async Task<NotificationConnection?> MyNotificationsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        NotificationOrderInput[]? orderBy,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var cachedCustomerService = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var service = scope.ServiceProvider.GetRequiredService<INotificationService>();
        var (paginatedInfo, edges, totalCount) =
            await service.GetMyPaginatedNotificationsAsync(
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
