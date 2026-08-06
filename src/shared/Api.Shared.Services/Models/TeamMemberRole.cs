namespace Api.Shared.Services.Models;

public enum TeamMemberRole
{
    Owner,
    Administrator,
    Member,
}

public static class TeamMemberRoleConstants
{
    public const string Owner = "OWNER";
    public const string Administrator = "ADMINISTRATOR";
    public const string Member = "MEMBER";
}

public static class TeamMemberRoleExtensions
{
    extension(string src)
    {
        public TeamMemberRole ToTeamMemberRole() =>
            src switch
            {
                TeamMemberRoleConstants.Owner => TeamMemberRole.Owner,
                TeamMemberRoleConstants.Administrator => TeamMemberRole.Administrator,
                TeamMemberRoleConstants.Member => TeamMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input."),
            };
    }

    extension(TeamMemberRole src)
    {
        public string ToTeamMemberRole() =>
            src switch
            {
                TeamMemberRole.Owner => TeamMemberRoleConstants.Owner,
                TeamMemberRole.Administrator => TeamMemberRoleConstants.Administrator,
                TeamMemberRole.Member => TeamMemberRoleConstants.Member,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input."),
            };

        public string ToTeamMemberRoleName() =>
            src switch
            {
                TeamMemberRole.Owner => "Owner",
                TeamMemberRole.Administrator => "Administrator",
                TeamMemberRole.Member => "Member",
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input."),
            };
    }

    extension(TeamMemberRole? src)
    {
        public string ToNullableTeamMemberRole() =>
            src is null
                ? string.Empty
                : src switch
                {
                    TeamMemberRole.Owner => TeamMemberRoleConstants.Owner,
                    TeamMemberRole.Administrator => TeamMemberRoleConstants.Administrator,
                    TeamMemberRole.Member => TeamMemberRoleConstants.Member,
                    _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                        $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input."),
                };
    }
}
