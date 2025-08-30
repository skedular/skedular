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
    public static LocationType ToLocationType(this string src) =>
        src switch
        {
            LocationTypeConstants.Private => LocationType.Private,
            LocationTypeConstants.Marketplace => LocationType.Marketplace,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToLocationType(this LocationType src) =>
        src switch
        {
            LocationType.Private => LocationTypeConstants.Private,
            LocationType.Marketplace => LocationTypeConstants.Marketplace,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToLocationTypeName(this LocationType src) =>
        src switch
        {
            LocationType.Private => "Private",
            LocationType.Marketplace => "Marketplace",
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToLocationTypeName(this string src) =>
        src switch
        {
            LocationTypeConstants.Private => "Private",
            LocationTypeConstants.Marketplace => "Marketplace",
            _ => throw new ArgumentOutOfRangeException()
        };
}
