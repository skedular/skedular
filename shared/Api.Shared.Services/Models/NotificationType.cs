namespace Api.Shared.Services.Models;

public enum NotificationType
{
    InvitationToJoinOrganization,
    InvitationToJoinTeam
}

public static class NotificationTypeConstants
{
    public const string InvitationToJoinOrganization = "INVITATION_TO_JOIN_ORGANIZATION";
    public const string InvitationToJoinTeam = "INVITATION_TO_JOIN_TEAM";
}

public static class NotificationTypeExtensions
{
    public static NotificationType ToNotificationType(this string src) =>
        src switch
        {
            NotificationTypeConstants.InvitationToJoinOrganization => NotificationType.InvitationToJoinOrganization,
            NotificationTypeConstants.InvitationToJoinTeam => NotificationType.InvitationToJoinTeam,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToNotificationType(this NotificationType src) =>
        src switch
        {
            NotificationType.InvitationToJoinOrganization => NotificationTypeConstants.InvitationToJoinOrganization,
            NotificationType.InvitationToJoinTeam => NotificationTypeConstants.InvitationToJoinTeam,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToNotificationTypeName(this NotificationType src) =>
        src switch
        {
            NotificationType.InvitationToJoinOrganization => "InvitationToJoinOrganization",
            NotificationType.InvitationToJoinTeam => "InvitationToJoinTeam",
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToNotificationTypeName(this string src) =>
        src switch
        {
            NotificationTypeConstants.InvitationToJoinOrganization => "InvitationToJoinOrganization",
            NotificationTypeConstants.InvitationToJoinTeam => "InvitationToJoinTeam",
            _ => throw new ArgumentOutOfRangeException()
        };
}
