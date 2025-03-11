namespace Api.Shared.Services.Models;

public enum LocationMemberRole
{
    Owner,
    Administrator,
    Member
}

public static class LocationMemberRoleConstants
{
    public const string Owner = "OWNER";
    public const string Administrator = "ADMINISTRATOR";
    public const string Member = "MEMBER";
}

public static class LocationMemberRoleExtensions
{
    public static LocationMemberRole ToLocationMemberRole(this string src) =>
        src switch
        {
            LocationMemberRoleConstants.Owner => LocationMemberRole.Owner,
            LocationMemberRoleConstants.Administrator => LocationMemberRole.Administrator,
            LocationMemberRoleConstants.Member => LocationMemberRole.Member,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static LocationMemberRole? ToNullableLocationMemberRole(this string? src) =>
        string.IsNullOrWhiteSpace(src)
            ? null
            : src switch
            {
                LocationMemberRoleConstants.Owner => LocationMemberRole.Owner,
                LocationMemberRoleConstants.Administrator => LocationMemberRole.Administrator,
                LocationMemberRoleConstants.Member => LocationMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string ToLocationMemberRole(this LocationMemberRole src) =>
        src switch
        {
            LocationMemberRole.Owner => LocationMemberRoleConstants.Owner,
            LocationMemberRole.Administrator => LocationMemberRoleConstants.Administrator,
            LocationMemberRole.Member => LocationMemberRoleConstants.Member,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToNullableLocationMemberRole(this LocationMemberRole? src) =>
        src is null
            ? string.Empty
            : src switch
            {
                LocationMemberRole.Owner => LocationMemberRoleConstants.Owner,
                LocationMemberRole.Administrator => LocationMemberRoleConstants.Administrator,
                LocationMemberRole.Member => LocationMemberRoleConstants.Member,
                _ => throw new ArgumentOutOfRangeException()
            };
}
