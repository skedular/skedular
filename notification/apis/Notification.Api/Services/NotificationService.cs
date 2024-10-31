using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Notification.Api.Mappers;
using Notification.Shared.Models;
using Notification.Shared.Repositories;

namespace Notification.Api.Services;

public interface INotificationService
{
    Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Notification>>, int )> GetMyPaginatedNotificationsAsync(
        PaginationInputParam paginationInputParam,
        NotificationSearchCriteria searchCriteria,
        ICollection<NotificationOrder> orderByFields,
        CancellationToken cancellationToken);
}

public class NotificationService(
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    IMapper mapper) : INotificationService
{
    public async Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Notification>>, int)>
        GetMyPaginatedNotificationsAsync(
            PaginationInputParam paginationInputParam,
            NotificationSearchCriteria searchCriteria,
            ICollection<NotificationOrder> orderByFields,
            CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetCustomerAsync(cancellationToken);
        // Ensure we do not return other customer notification by forcing CustomerId as search criteria
        searchCriteria.InviteeId = customer.Id;

        var (paginatedInfo, edges, totalCount) =
            await repositoryFactory.NotificationRepository.GetPaginatedNotificationsAsync(
                paginationInputParam,
                searchCriteria,
                orderByFields,
                cancellationToken);

        return (paginatedInfo, edges.Select(mapper.MapTo).ToList(), totalCount);
    }
}
