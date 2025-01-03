namespace Api.Shared.Services.Models;

public enum LocationMemberRole
{
    Owner,
    Administrator,
    Member
}

public static class LocationRoleConstants
{
    public const string Owner = "OWNER";
    public const string Administrator = "ADMINISTRATOR";
    public const string Member = "MEMBER";
}
