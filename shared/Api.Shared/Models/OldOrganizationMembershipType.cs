namespace Api.Shared.Models;

public enum OldOrganizationMembershipType
{
    Owner = 0,
    Administrator = 1,
    Member = 2
}

public class OrganizationMembershipType
{
    public const string Owner = "OWNER";
    public const string Administrator = "ADMINISTRATOR";
    public const string Member = "MEMBER";
}
