namespace Api.Shared.Services.Models;

public enum OrganizationMemberStatus
{
    Active,
    Inactive
}

public static class OrganizationMemberStatusConstants
{
    public const string Active = "ACTIVE";
    public const string Inactive = "INACTIVE";
}

public static class OrganizationMemberStatusExtensions
{
    public static OrganizationMemberStatus ToOrganizationMemberStatus(this string src) =>
        src switch
        {
            OrganizationMemberStatusConstants.Active => OrganizationMemberStatus.Active,
            OrganizationMemberStatusConstants.Inactive => OrganizationMemberStatus.Inactive,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToOrganizationMemberStatus(this OrganizationMemberStatus src) =>
        src switch
        {
            OrganizationMemberStatus.Active => OrganizationMemberStatusConstants.Active,
            OrganizationMemberStatus.Inactive => OrganizationMemberStatusConstants.Inactive,
            _ => throw new ArgumentOutOfRangeException()
        };
}
