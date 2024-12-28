namespace Api.Shared.Services.Models;

public enum LocationMembershipType
{
    Owner,
    Administrator,
    Member
}

public static class LocationMembershipTypeConstants
{
    public const string Owner = "OWNER";
    public const string Administrator = "ADMINISTRATOR";
    public const string Member = "MEMBER";
}
