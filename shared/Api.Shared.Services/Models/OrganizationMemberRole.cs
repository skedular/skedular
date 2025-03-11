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

public static class OrganizationMemberRoleExtensions
{
    public static OrganizationMemberRole ToOrganizationMemberRole(this string src) =>
        src switch
        {
            OrganizationMemberRoleConstants.Owner => OrganizationMemberRole.Owner,
            OrganizationMemberRoleConstants.Administrator => OrganizationMemberRole.Administrator,
            OrganizationMemberRoleConstants.Member => OrganizationMemberRole.Member,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static OrganizationMemberRole? ToNullableOrganizationMemberRole(this string? src) =>
        string.IsNullOrWhiteSpace(src)
            ? null
            : src switch
            {
                OrganizationMemberRoleConstants.Owner => OrganizationMemberRole.Owner,
                OrganizationMemberRoleConstants.Administrator => OrganizationMemberRole.Administrator,
                OrganizationMemberRoleConstants.Member => OrganizationMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string ToOrganizationMemberRole(this OrganizationMemberRole src) =>
        src switch
        {
            OrganizationMemberRole.Owner => OrganizationMemberRoleConstants.Owner,
            OrganizationMemberRole.Administrator => OrganizationMemberRoleConstants.Administrator,
            OrganizationMemberRole.Member => OrganizationMemberRoleConstants.Member,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToNullableOrganizationMemberRole(this OrganizationMemberRole? src) =>
        src is null
            ? string.Empty
            : src switch
            {
                OrganizationMemberRole.Owner => OrganizationMemberRoleConstants.Owner,
                OrganizationMemberRole.Administrator => OrganizationMemberRoleConstants.Administrator,
                OrganizationMemberRole.Member => OrganizationMemberRoleConstants.Member,
                _ => throw new ArgumentOutOfRangeException()
            };
}
