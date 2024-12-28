namespace Api.Shared.Services.Models;

public enum OrganizationMembershipType
{
    Owner,
    Administrator,
    Member
}

public static class OrganizationMembershipTypeConstants
{
    public const string Owner = "OWNER";
    public const string Administrator = "ADMINISTRATOR";
    public const string Member = "MEMBER";
}
