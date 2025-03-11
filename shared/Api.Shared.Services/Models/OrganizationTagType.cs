namespace Api.Shared.Services.Models;

public enum OrganizationTagType
{
    Custom,
    Zone,
    Desk,
    Room
}

public static class OrganizationTagTypeConstants
{
    public const string Custom = "CUSTOM";
    public const string Zone = "ZONE";
    public const string Desk = "DESK";
    public const string Room = "ROOM";
}

public static class OrganizationTagTypeExtensions
{
    public static OrganizationTagType ToOrganizationTagType(this string src) =>
        src switch
        {
            OrganizationTagTypeConstants.Custom => OrganizationTagType.Custom,
            OrganizationTagTypeConstants.Zone => OrganizationTagType.Zone,
            OrganizationTagTypeConstants.Desk => OrganizationTagType.Desk,
            OrganizationTagTypeConstants.Room => OrganizationTagType.Room,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static OrganizationTagType? ToNullableOrganizationTagType(this string? src) =>
        string.IsNullOrWhiteSpace(src)
            ? null
            : src switch
            {
                OrganizationTagTypeConstants.Custom => OrganizationTagType.Custom,
                OrganizationTagTypeConstants.Zone => OrganizationTagType.Zone,
                OrganizationTagTypeConstants.Desk => OrganizationTagType.Desk,
                OrganizationTagTypeConstants.Room => OrganizationTagType.Room,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string ToOrganizationTagType(this OrganizationTagType src) =>
        src switch
        {
            OrganizationTagType.Custom => OrganizationTagTypeConstants.Custom,
            OrganizationTagType.Zone => OrganizationTagTypeConstants.Zone,
            OrganizationTagType.Desk => OrganizationTagTypeConstants.Desk,
            OrganizationTagType.Room => OrganizationTagTypeConstants.Room,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToNullableOrganizationTagType(this OrganizationTagType? src) =>
        src is null
            ? string.Empty
            : src switch
            {
                OrganizationTagType.Custom => OrganizationTagTypeConstants.Custom,
                OrganizationTagType.Zone => OrganizationTagTypeConstants.Zone,
                OrganizationTagType.Desk => OrganizationTagTypeConstants.Desk,
                OrganizationTagType.Room => OrganizationTagTypeConstants.Room,
                _ => throw new ArgumentOutOfRangeException()
            };
}
