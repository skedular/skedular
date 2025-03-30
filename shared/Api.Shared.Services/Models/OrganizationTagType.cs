namespace Api.Shared.Services.Models;

public enum OrganizationTagType
{
    Custom,
    Zone,
    Product,
    Location,
    ResourceDesk,
    ResourceRoom,
    ResourceParking,
    ResourceOthers
}

public static class OrganizationTagTypeConstants
{
    public const string Custom = "CUSTOM";
    public const string Zone = "ZONE";
    public const string Product = "PRODUCT";
    public const string Location = "LOCATION";
    public const string ResourceDesk = "RESOURCE_DESK";
    public const string ResourceRoom = "RESOURCE_ROOM";
    public const string ResourceParking = "RESOURCE_PARKING";
    public const string ResourceOthers = "RESOURCE_OTHERS";

    public static readonly ICollection<OrganizationTagType> ResourceTypes =
    [
        OrganizationTagType.ResourceDesk,
        OrganizationTagType.ResourceRoom,
        OrganizationTagType.ResourceParking,
        OrganizationTagType.ResourceOthers
    ];
}

public static class OrganizationTagTypeExtensions
{
    public static OrganizationTagType ToOrganizationTagType(this string src) =>
        src switch
        {
            OrganizationTagTypeConstants.Custom => OrganizationTagType.Custom,
            OrganizationTagTypeConstants.Zone => OrganizationTagType.Zone,
            OrganizationTagTypeConstants.Product => OrganizationTagType.Product,
            OrganizationTagTypeConstants.Location => OrganizationTagType.Location,
            OrganizationTagTypeConstants.ResourceDesk => OrganizationTagType.ResourceDesk,
            OrganizationTagTypeConstants.ResourceRoom => OrganizationTagType.ResourceRoom,
            OrganizationTagTypeConstants.ResourceParking => OrganizationTagType.ResourceParking,
            OrganizationTagTypeConstants.ResourceOthers => OrganizationTagType.ResourceOthers,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static OrganizationTagType? ToNullableOrganizationTagType(this string? src) =>
        string.IsNullOrWhiteSpace(src)
            ? null
            : src switch
            {
                OrganizationTagTypeConstants.Custom => OrganizationTagType.Custom,
                OrganizationTagTypeConstants.Zone => OrganizationTagType.Zone,
                OrganizationTagTypeConstants.Product => OrganizationTagType.Product,
                OrganizationTagTypeConstants.Location => OrganizationTagType.Location,
                OrganizationTagTypeConstants.ResourceDesk => OrganizationTagType.ResourceDesk,
                OrganizationTagTypeConstants.ResourceRoom => OrganizationTagType.ResourceRoom,
                OrganizationTagTypeConstants.ResourceParking => OrganizationTagType.ResourceParking,
                OrganizationTagTypeConstants.ResourceOthers => OrganizationTagType.ResourceOthers,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string ToOrganizationTagType(this OrganizationTagType src) =>
        src switch
        {
            OrganizationTagType.Custom => OrganizationTagTypeConstants.Custom,
            OrganizationTagType.Zone => OrganizationTagTypeConstants.Zone,
            OrganizationTagType.Product => OrganizationTagTypeConstants.Product,
            OrganizationTagType.Location => OrganizationTagTypeConstants.Location,
            OrganizationTagType.ResourceDesk => OrganizationTagTypeConstants.ResourceDesk,
            OrganizationTagType.ResourceRoom => OrganizationTagTypeConstants.ResourceRoom,
            OrganizationTagType.ResourceParking => OrganizationTagTypeConstants.ResourceParking,
            OrganizationTagType.ResourceOthers => OrganizationTagTypeConstants.ResourceOthers,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToNullableOrganizationTagType(this OrganizationTagType? src) =>
        src is null
            ? string.Empty
            : src switch
            {
                OrganizationTagType.Custom => OrganizationTagTypeConstants.Custom,
                OrganizationTagType.Zone => OrganizationTagTypeConstants.Zone,
                OrganizationTagType.Product => OrganizationTagTypeConstants.Product,
                OrganizationTagType.Location => OrganizationTagTypeConstants.Location,
                OrganizationTagType.ResourceDesk => OrganizationTagTypeConstants.ResourceDesk,
                OrganizationTagType.ResourceRoom => OrganizationTagTypeConstants.ResourceRoom,
                OrganizationTagType.ResourceParking => OrganizationTagTypeConstants.ResourceParking,
                OrganizationTagType.ResourceOthers => OrganizationTagTypeConstants.ResourceOthers,
                _ => throw new ArgumentOutOfRangeException()
            };
}
