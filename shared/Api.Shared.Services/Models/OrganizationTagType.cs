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
    ResourceOthers,

    LocationSpaceTypeCarParkSpace,
    LocationSpaceTypeEventSpace,
    LocationSpaceTypeMeetingSpace,
    LocationSpaceTypeOfficeSpace,
    LocationSpaceTypeRetailSpace,
    LocationSpaceTypeStorageSpace,
    LocationSpaceTypeStudioSpace,
    LocationSpaceTypeCommercialKitchen,
    LocationSpaceTypeShootLocation,
    LocationSpaceTypeOthers
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

    public const string LocationSpaceTypeCarParkSpace = "LOCATION_SPACE_TYPE_CARPARK_SPACE";
    public const string LocationSpaceTypeEventSpace = "LOCATION_SPACE_TYPE_EVENT_SPACE";
    public const string LocationSpaceTypeMeetingSpace = "LOCATION_SPACE_TYPE_MEETING_SPACE";
    public const string LocationSpaceTypeOfficeSpace = "LOCATION_SPACE_TYPE_OFFICE_SPACE";
    public const string LocationSpaceTypeRetailSpace = "LOCATION_SPACE_TYPE_RETAIL_SPACE";
    public const string LocationSpaceTypeStorageSpace = "LOCATION_SPACE_TYPE_STORAGE_SPACE";
    public const string LocationSpaceTypeStudioSpace = "LOCATION_SPACE_TYPE_STUDIO_SPACE";
    public const string LocationSpaceTypeCommercialKitchen = "LOCATION_SPACE_TYPE_COMMERCIAL_KITCHEN";
    public const string LocationSpaceTypeShootLocation = "LOCATION_SPACE_TYPE_SHOOT_LOCATION";
    public const string LocationSpaceTypeOthers = "LOCATION_SPACE_TYPE_OTHERS";

    public static readonly ICollection<OrganizationTagType> ResourceTypes =
    [
        OrganizationTagType.ResourceDesk,
        OrganizationTagType.ResourceRoom,
        OrganizationTagType.ResourceParking,
        OrganizationTagType.ResourceOthers
    ];

    public static readonly ICollection<OrganizationTagType> LocationSpaceTypes =
    [
        OrganizationTagType.LocationSpaceTypeCarParkSpace,
        OrganizationTagType.LocationSpaceTypeEventSpace,
        OrganizationTagType.LocationSpaceTypeMeetingSpace,
        OrganizationTagType.LocationSpaceTypeOfficeSpace,
        OrganizationTagType.LocationSpaceTypeRetailSpace,
        OrganizationTagType.LocationSpaceTypeStorageSpace,
        OrganizationTagType.LocationSpaceTypeStudioSpace,
        OrganizationTagType.LocationSpaceTypeCommercialKitchen,
        OrganizationTagType.LocationSpaceTypeShootLocation,
        OrganizationTagType.LocationSpaceTypeOthers
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

            OrganizationTagTypeConstants.LocationSpaceTypeCarParkSpace => OrganizationTagType.LocationSpaceTypeCarParkSpace,
            OrganizationTagTypeConstants.LocationSpaceTypeEventSpace => OrganizationTagType.LocationSpaceTypeEventSpace,
            OrganizationTagTypeConstants.LocationSpaceTypeMeetingSpace => OrganizationTagType.LocationSpaceTypeMeetingSpace,
            OrganizationTagTypeConstants.LocationSpaceTypeOfficeSpace => OrganizationTagType.LocationSpaceTypeOfficeSpace,
            OrganizationTagTypeConstants.LocationSpaceTypeRetailSpace => OrganizationTagType.LocationSpaceTypeRetailSpace,
            OrganizationTagTypeConstants.LocationSpaceTypeStorageSpace => OrganizationTagType.LocationSpaceTypeStorageSpace,
            OrganizationTagTypeConstants.LocationSpaceTypeStudioSpace => OrganizationTagType.LocationSpaceTypeStudioSpace,
            OrganizationTagTypeConstants.LocationSpaceTypeCommercialKitchen => OrganizationTagType.LocationSpaceTypeCommercialKitchen,
            OrganizationTagTypeConstants.LocationSpaceTypeShootLocation => OrganizationTagType.LocationSpaceTypeShootLocation,
            OrganizationTagTypeConstants.LocationSpaceTypeOthers => OrganizationTagType.LocationSpaceTypeOthers,

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

                OrganizationTagTypeConstants.LocationSpaceTypeCarParkSpace => OrganizationTagType.LocationSpaceTypeCarParkSpace,
                OrganizationTagTypeConstants.LocationSpaceTypeEventSpace => OrganizationTagType.LocationSpaceTypeEventSpace,
                OrganizationTagTypeConstants.LocationSpaceTypeMeetingSpace => OrganizationTagType.LocationSpaceTypeMeetingSpace,
                OrganizationTagTypeConstants.LocationSpaceTypeOfficeSpace => OrganizationTagType.LocationSpaceTypeOfficeSpace,
                OrganizationTagTypeConstants.LocationSpaceTypeRetailSpace => OrganizationTagType.LocationSpaceTypeRetailSpace,
                OrganizationTagTypeConstants.LocationSpaceTypeStorageSpace => OrganizationTagType.LocationSpaceTypeStorageSpace,
                OrganizationTagTypeConstants.LocationSpaceTypeStudioSpace => OrganizationTagType.LocationSpaceTypeStudioSpace,
                OrganizationTagTypeConstants.LocationSpaceTypeCommercialKitchen => OrganizationTagType.LocationSpaceTypeCommercialKitchen,
                OrganizationTagTypeConstants.LocationSpaceTypeShootLocation => OrganizationTagType.LocationSpaceTypeShootLocation,
                OrganizationTagTypeConstants.LocationSpaceTypeOthers => OrganizationTagType.LocationSpaceTypeOthers,
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

            OrganizationTagType.LocationSpaceTypeCarParkSpace => OrganizationTagTypeConstants.LocationSpaceTypeCarParkSpace,
            OrganizationTagType.LocationSpaceTypeEventSpace => OrganizationTagTypeConstants.LocationSpaceTypeEventSpace,
            OrganizationTagType.LocationSpaceTypeMeetingSpace => OrganizationTagTypeConstants.LocationSpaceTypeMeetingSpace,
            OrganizationTagType.LocationSpaceTypeOfficeSpace => OrganizationTagTypeConstants.LocationSpaceTypeOfficeSpace,
            OrganizationTagType.LocationSpaceTypeRetailSpace => OrganizationTagTypeConstants.LocationSpaceTypeRetailSpace,
            OrganizationTagType.LocationSpaceTypeStorageSpace => OrganizationTagTypeConstants.LocationSpaceTypeStorageSpace,
            OrganizationTagType.LocationSpaceTypeStudioSpace => OrganizationTagTypeConstants.LocationSpaceTypeStudioSpace,
            OrganizationTagType.LocationSpaceTypeCommercialKitchen => OrganizationTagTypeConstants.LocationSpaceTypeCommercialKitchen,
            OrganizationTagType.LocationSpaceTypeShootLocation => OrganizationTagTypeConstants.LocationSpaceTypeShootLocation,
            OrganizationTagType.LocationSpaceTypeOthers => OrganizationTagTypeConstants.LocationSpaceTypeOthers,
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

                OrganizationTagType.LocationSpaceTypeCarParkSpace => OrganizationTagTypeConstants.LocationSpaceTypeCarParkSpace,
                OrganizationTagType.LocationSpaceTypeEventSpace => OrganizationTagTypeConstants.LocationSpaceTypeEventSpace,
                OrganizationTagType.LocationSpaceTypeMeetingSpace => OrganizationTagTypeConstants.LocationSpaceTypeMeetingSpace,
                OrganizationTagType.LocationSpaceTypeOfficeSpace => OrganizationTagTypeConstants.LocationSpaceTypeOfficeSpace,
                OrganizationTagType.LocationSpaceTypeRetailSpace => OrganizationTagTypeConstants.LocationSpaceTypeRetailSpace,
                OrganizationTagType.LocationSpaceTypeStorageSpace => OrganizationTagTypeConstants.LocationSpaceTypeStorageSpace,
                OrganizationTagType.LocationSpaceTypeStudioSpace => OrganizationTagTypeConstants.LocationSpaceTypeStudioSpace,
                OrganizationTagType.LocationSpaceTypeCommercialKitchen => OrganizationTagTypeConstants.LocationSpaceTypeCommercialKitchen,
                OrganizationTagType.LocationSpaceTypeShootLocation => OrganizationTagTypeConstants.LocationSpaceTypeShootLocation,
                OrganizationTagType.LocationSpaceTypeOthers => OrganizationTagTypeConstants.LocationSpaceTypeOthers,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string ToOrganizationTagTypeName(this OrganizationTagType src) =>
        src switch
        {
            OrganizationTagType.Custom => "Custom",
            OrganizationTagType.Zone => "Zone",
            OrganizationTagType.Product => "Product",
            OrganizationTagType.Location => "Location",

            OrganizationTagType.ResourceDesk => "Desk",
            OrganizationTagType.ResourceRoom => "Room",
            OrganizationTagType.ResourceParking => "Parking",
            OrganizationTagType.ResourceOthers => "Others",

            OrganizationTagType.LocationSpaceTypeCarParkSpace => "Car Park Space",
            OrganizationTagType.LocationSpaceTypeEventSpace => "Event Space",
            OrganizationTagType.LocationSpaceTypeMeetingSpace => "Meeting Space",
            OrganizationTagType.LocationSpaceTypeOfficeSpace => "Office Space",
            OrganizationTagType.LocationSpaceTypeRetailSpace => "Retail Space",
            OrganizationTagType.LocationSpaceTypeStorageSpace => "Storage Space",
            OrganizationTagType.LocationSpaceTypeStudioSpace => "Studio Space",
            OrganizationTagType.LocationSpaceTypeCommercialKitchen => "Commercial Kitchen",
            OrganizationTagType.LocationSpaceTypeShootLocation => "Shoot Location",
            OrganizationTagType.LocationSpaceTypeOthers => "Others",
            _ => throw new ArgumentOutOfRangeException()
        };
}
