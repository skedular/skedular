namespace Api.Shared.Services.Models;

public enum OrganizationMemberVisibilityPolicy
{
    FullAccess,
    LimitedAccess
}

public static class OrganizationMemberVisibilityPolicyConstants
{
    public const string FullAccess = "FULL_ACCESS";
    public const string LimitedAccess = "LIMITED_ACCESS";
}

public static class OrganizationMemberVisibilityPolicyExtensions
{
    public static OrganizationMemberVisibilityPolicy ToOrganizationMemberVisibilityPolicy(this string src) =>
        src switch
        {
            OrganizationMemberVisibilityPolicyConstants.FullAccess => OrganizationMemberVisibilityPolicy.FullAccess,
            OrganizationMemberVisibilityPolicyConstants.LimitedAccess => OrganizationMemberVisibilityPolicy.LimitedAccess,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToOrganizationMemberVisibilityPolicy(this OrganizationMemberVisibilityPolicy src) =>
        src switch
        {
            OrganizationMemberVisibilityPolicy.FullAccess => OrganizationMemberVisibilityPolicyConstants.FullAccess,
            OrganizationMemberVisibilityPolicy.LimitedAccess => OrganizationMemberVisibilityPolicyConstants.LimitedAccess,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToOrganizationMemberVisibilityPolicyName(this OrganizationMemberVisibilityPolicy src) =>
        src switch
        {
            OrganizationMemberVisibilityPolicy.FullAccess => "Full Access",
            OrganizationMemberVisibilityPolicy.LimitedAccess => "Limited Access",
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToOrganizationMemberVisibilityPolicyName(this string src) =>
        src switch
        {
            OrganizationMemberVisibilityPolicyConstants.FullAccess => "Full Access",
            OrganizationMemberVisibilityPolicyConstants.LimitedAccess => "Limited Access",
            _ => throw new ArgumentOutOfRangeException()
        };
}
