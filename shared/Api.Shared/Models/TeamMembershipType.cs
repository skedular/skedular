namespace Api.Shared.Models;

public enum OldTeamMembershipType
{
    Owner = 0,
    Administrator = 1,
    Member = 2
}

public class TeamMembershipType
{
    public const string Owner = "OWNER";
    public const string Administrator = "ADMINISTRATOR";
    public const string Member = "MEMBER";
}
