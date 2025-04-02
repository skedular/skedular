namespace Api.Shared.Services.Models;

public enum InvitationStatus
{
    Pending,
    Accepted,
    Rejected,
    Cancelled
}

public static class InvitationStatusConstants
{
    public const string Pending = "PENDING";
    public const string Accepted = "ACCEPTED";
    public const string Rejected = "REJECTED";
    public const string Cancelled = "CANCELLED";
}

public static class InvitationStatusExtensions
{
    public static InvitationStatus ToInvitationStatus(this string src) =>
        src switch
        {
            InvitationStatusConstants.Pending => InvitationStatus.Pending,
            InvitationStatusConstants.Accepted => InvitationStatus.Accepted,
            InvitationStatusConstants.Rejected => InvitationStatus.Rejected,
            InvitationStatusConstants.Cancelled => InvitationStatus.Cancelled,
            _ => throw new ArgumentOutOfRangeException()
        };
}
