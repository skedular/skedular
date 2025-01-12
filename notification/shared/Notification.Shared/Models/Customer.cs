using Enterprise.Shared.Models;

namespace Notification.Shared.Models;

public class Customer : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public string? GivenName { get; set; }
    public string? MiddleName { get; set; }
    public string? FamilyName { get; set; }
    public string? PhotoUrl { get; set; }
    public string? PhotoUrl24 { get; set; }
    public string? PhotoUrl32 { get; set; }
    public string? PhotoUrl48 { get; set; }
    public string? PhotoUrl72 { get; set; }
    public string? PhotoUrl192 { get; set; }
    public string? PhotoUrl512 { get; set; }

    public ICollection<Identity> Identities { get; set; } = [];
    public ICollection<Notification> InvitedByNotifications { get; set; } = [];
    public ICollection<Notification> InviteeNotifications { get; set; } = [];
}
