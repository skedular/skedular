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
    extension(string src)
    {
        public InvitationStatus ToInvitationStatus() =>
            src switch
            {
                InvitationStatusConstants.Pending => InvitationStatus.Pending,
                InvitationStatusConstants.Accepted => InvitationStatus.Accepted,
                InvitationStatusConstants.Rejected => InvitationStatus.Rejected,
                InvitationStatusConstants.Cancelled => InvitationStatus.Cancelled,
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(InvitationStatus src)
    {
        public string ToInvitationStatus() =>
            src switch
            {
                InvitationStatus.Pending => InvitationStatusConstants.Pending,
                InvitationStatus.Accepted => InvitationStatusConstants.Accepted,
                InvitationStatus.Rejected => InvitationStatusConstants.Rejected,
                InvitationStatus.Cancelled => InvitationStatusConstants.Cancelled,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToInvitationStatusName() =>
            src switch
            {
                InvitationStatus.Pending => "Pending",
                InvitationStatus.Accepted => "Accepted",
                InvitationStatus.Rejected => "Rejected",
                InvitationStatus.Cancelled => "Cancelled",
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
