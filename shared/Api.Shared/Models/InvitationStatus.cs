namespace Api.Shared.Models;

public enum OldInvitationStatus
{
    Pending = 0,
    Accepted = 1,
    Rejected = 2,
    Cancelled = 3
}

public class InvitationStatus
{
    public const string Pending = "PENDING";
    public const string Accepted = "ACCEPTED";
    public const string Rejected = "REJECTED";
    public const string Cancelled = "CANCELLED";
}
