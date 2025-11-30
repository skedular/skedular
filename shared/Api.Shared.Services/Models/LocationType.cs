namespace Api.Shared.Services.Models;

public enum LocationType
{
    Private,
    Marketplace
}

public static class LocationTypeConstants
{
    public const string Private = "PRIVATE";
    public const string Marketplace = "MARKETPLACE";
}

public static class LocationTypeExtensions
{
    extension(LocationType? src)
    {
        public string? ToNullableLocationType() =>
            src is null
                ? null
                : src switch
                {
                    LocationType.Private => LocationTypeConstants.Private,
                    LocationType.Marketplace => LocationTypeConstants.Marketplace,
                    _ => throw new ArgumentOutOfRangeException()
                };
    }

    extension(LocationType src)
    {
        public string ToLocationType() =>
            src switch
            {
                LocationType.Private => LocationTypeConstants.Private,
                LocationType.Marketplace => LocationTypeConstants.Marketplace,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToLocationTypeName() =>
            src switch
            {
                LocationType.Private => "Private",
                LocationType.Marketplace => "Marketplace",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(string? src)
    {
        public LocationType? ToNullableLocationType() =>
            src is null
                ? null
                : src switch
                {
                    LocationTypeConstants.Private => LocationType.Private,
                    LocationTypeConstants.Marketplace => LocationType.Marketplace,
                    _ => throw new ArgumentOutOfRangeException()
                };
    }

    extension(string src)
    {
        public string ToLocationTypeName() =>
            src switch
            {
                LocationTypeConstants.Private => "Private",
                LocationTypeConstants.Marketplace => "Marketplace",
                _ => throw new ArgumentOutOfRangeException()
            };

        public LocationType ToLocationType() =>
            src switch
            {
                LocationTypeConstants.Private => LocationType.Private,
                LocationTypeConstants.Marketplace => LocationType.Marketplace,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
