namespace Api.Shared.Models;

public enum OldLocationMembershipType
{
    Owner = 0,
    Administrator = 1,
    Member = 2
}

public class LocationMembershipType
{
    public const string Owner = "OWNER";
    public const string Administrator = "ADMINISTRATOR";
    public const string Member = "MEMBER";
}
