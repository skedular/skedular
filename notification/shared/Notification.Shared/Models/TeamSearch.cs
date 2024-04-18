using Enterprise.Shared.Pagination;

namespace Notification.Shared.Models;

public class NotificationSearchCriteria
{
    public string? InviteeId { get; set; }
}

public record NotificationOrder(OrderDirection Direction, NotificationOrderField Field);

public enum NotificationOrderField
{
    EventRaisedAt,
    Type
}
