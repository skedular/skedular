using Enterprise.Shared.Pagination;

namespace Notification.Shared.Models;

public class NotificationSearchCriteria(string? organizationId)
{
    public string? InviteeId { get; set; }
    public string? OrganizationId { get; set; } = organizationId;
}

public record NotificationOrder(OrderDirection Direction, NotificationOrderField Field);

public enum NotificationOrderField
{
    Date,
    Type
}
