namespace Api.Shared.Services.Models;

public enum TeamMemberRole
{
    Owner,
    Administrator,
    Member
}

public static class TeamMemberRoleConstants
{
    public const string Owner = "OWNER";
    public const string Administrator = "ADMINISTRATOR";
    public const string Member = "MEMBER";
}
