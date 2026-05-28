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
    extension(string src)
    {
        public OrganizationMemberRole ToOrganizationMemberRole() =>
            src switch
            {
                OrganizationMemberRoleConstants.Owner => OrganizationMemberRole.Owner,
                OrganizationMemberRoleConstants.Administrator => OrganizationMemberRole.Administrator,
                OrganizationMemberRoleConstants.Member => OrganizationMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(string? src)
    {
        public OrganizationMemberRole? ToNullableOrganizationMemberRole() =>
            string.IsNullOrWhiteSpace(src)
                ? null
                : src switch
                {
                    OrganizationMemberRoleConstants.Owner => OrganizationMemberRole.Owner,
                    OrganizationMemberRoleConstants.Administrator => OrganizationMemberRole.Administrator,
                    OrganizationMemberRoleConstants.Member => OrganizationMemberRole.Member,
                    _ => throw new ArgumentOutOfRangeException()
                };
    }

    extension(OrganizationMemberRole src)
    {
        public string ToOrganizationMemberRole() =>
            src switch
            {
                OrganizationMemberRole.Owner => OrganizationMemberRoleConstants.Owner,
                OrganizationMemberRole.Administrator => OrganizationMemberRoleConstants.Administrator,
                OrganizationMemberRole.Member => OrganizationMemberRoleConstants.Member,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToOrganizationMemberRoleName() =>
            src switch
            {
                OrganizationMemberRole.Owner => "Owner",
                OrganizationMemberRole.Administrator => "Administrator",
                OrganizationMemberRole.Member => "Member",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(OrganizationMemberRole? src)
    {
        public string ToNullableOrganizationMemberRole() =>
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
}
