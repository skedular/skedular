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

    public static InvitationStatus? ToNullableInvitationStatus(this string? src) =>
        string.IsNullOrWhiteSpace(src)
            ? null
            : src switch
            {
                InvitationStatusConstants.Pending => InvitationStatus.Pending,
                InvitationStatusConstants.Accepted => InvitationStatus.Accepted,
                InvitationStatusConstants.Rejected => InvitationStatus.Rejected,
                InvitationStatusConstants.Cancelled => InvitationStatus.Cancelled,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string ToInvitationStatus(this InvitationStatus src) =>
        src switch
        {
            InvitationStatus.Pending => InvitationStatusConstants.Pending,
            InvitationStatus.Accepted => InvitationStatusConstants.Accepted,
            InvitationStatus.Rejected => InvitationStatusConstants.Rejected,
            InvitationStatus.Cancelled => InvitationStatusConstants.Cancelled,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToNullableInvitationStatus(this InvitationStatus? src) =>
        src is null
            ? string.Empty
            : src switch
            {
                InvitationStatus.Pending => InvitationStatusConstants.Pending,
                InvitationStatus.Accepted => InvitationStatusConstants.Accepted,
                InvitationStatus.Rejected => InvitationStatusConstants.Rejected,
                InvitationStatus.Cancelled => InvitationStatusConstants.Cancelled,
                _ => throw new ArgumentOutOfRangeException()
            };
}
