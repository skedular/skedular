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
    extension(string src)
    {
        public NotificationType ToNotificationType() =>
            src switch
            {
                NotificationTypeConstants.InvitationToJoinOrganization => NotificationType.InvitationToJoinOrganization,
                NotificationTypeConstants.InvitationToJoinTeam => NotificationType.InvitationToJoinTeam,
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(NotificationType src)
    {
        public string ToNotificationType() =>
            src switch
            {
                NotificationType.InvitationToJoinOrganization => NotificationTypeConstants.InvitationToJoinOrganization,
                NotificationType.InvitationToJoinTeam => NotificationTypeConstants.InvitationToJoinTeam,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
