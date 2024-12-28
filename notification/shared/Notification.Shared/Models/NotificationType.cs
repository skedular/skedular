namespace Notification.Shared.Models;

public enum NotificationType
{
    InvitationToJoinOrganization,
    InvitationToJoinLocation,
    InvitationToJoinTeam
}

public static class NotificationTypeConstants
{
    public const string InvitationToJoinOrganization = "INVITATION_TO_JOIN_ORGANIZATION";
    public const string InvitationToJoinLocation = "INVITATION_TO_JOIN_LOCATION";
    public const string InvitationToJoinTeam = "INVITATION_TO_JOIN_TEAM";
}
