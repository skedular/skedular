using System.Reflection;
using Api.Shared.Services.GraphQL.UnityHub.V1.Notification;
using Enterprise.Shared.Context;
using Enterprise.Shared.Pagination;
using Notification.Api.Mappers;
using Notification.Api.Services;
using Notification.Shared.Models;
using NotificationOrderInput = Api.Shared.Services.GraphQL.UnityHub.V1.Notification.NotificationOrderInput;
using NotificationOrderField = Api.Shared.Services.GraphQL.UnityHub.V1.Notification.NotificationOrderField;
using OrderDirection = Api.Shared.Services.GraphQL.UnityHub.V1.Notification.OrderDirection;
using Version = Api.Shared.Services.GraphQL.UnityHub.V1.Notification.Version;

namespace Notification.Api.GraphQL;

public class NotificationQuery(IMapper mapper) : Query
{
    public override Task<Version> NotificationVersionAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
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

    public override async Task<bool> NotificationCustomerRecordSyncedAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerService>();
        return await service.DoesCustomerExistAsync(cancellationToken);
    }

    public override async Task<NotificationConnection?> MyNotificationsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        NotificationOrderInput[]? orderBy,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>();
        if (!await customerService.DoesCustomerExistAsync(cancellationToken))
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
