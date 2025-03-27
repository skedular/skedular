namespace Api.Shared.Services.Models;

public enum OrganizationType
{
    Private,
    Marketplace
}

public static class OrganizationTypeConstants
{
    public const string Private = "PRIVATE";
    public const string Marketplace = "MARKETPLACE";
}

public static class OrganizationTypeExtensions
{
    public static OrganizationType ToOrganizationType(this string src) =>
        src switch
        {
            OrganizationTypeConstants.Private => OrganizationType.Private,
            OrganizationTypeConstants.Marketplace => OrganizationType.Marketplace,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static OrganizationType? ToNullableOrganizationType(this string? src) =>
        string.IsNullOrWhiteSpace(src)
            ? null
            : src switch
            {
                OrganizationTypeConstants.Private => OrganizationType.Private,
                OrganizationTypeConstants.Marketplace => OrganizationType.Marketplace,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string ToOrganizationType(this OrganizationType src) =>
        src switch
        {
            OrganizationType.Private => OrganizationTypeConstants.Private,
            OrganizationType.Marketplace => OrganizationTypeConstants.Marketplace,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToNullableOrganizationType(this OrganizationType? src) =>
        src is null
            ? string.Empty
            : src switch
            {
                OrganizationType.Private => OrganizationTypeConstants.Private,
                OrganizationType.Marketplace => OrganizationTypeConstants.Marketplace,
                _ => throw new ArgumentOutOfRangeException()
            };
}
