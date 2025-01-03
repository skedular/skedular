namespace Api.Shared.Services.Models;

public enum OrganizationMemberRole
{
    Owner,
    Administrator,
    Member
}

public static class OrganizationMemberRoleConstants
{
    public const string Owner = "OWNER";
    public const string Administrator = "ADMINISTRATOR";
    public const string Member = "MEMBER";
}
