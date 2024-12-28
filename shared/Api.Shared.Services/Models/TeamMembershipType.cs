namespace Api.Shared.Services.Models;

public enum TeamMembershipType
{
    Owner,
    Administrator,
    Member
}

public static class TeamMembershipTypeConstants
{
    public const string Owner = "OWNER";
    public const string Administrator = "ADMINISTRATOR";
    public const string Member = "MEMBER";
}
