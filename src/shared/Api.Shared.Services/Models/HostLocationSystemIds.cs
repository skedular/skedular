namespace Api.Shared.Services.Models;

public static class HostLocationSystemIds
{
    public const string ProductTagPrefix = "host-location-";
    public const string ResourcePrefix = "host-location-resource-";
    public const string ProductPrefix = "host-location-product-";

    public static string ProductTag(string locationId) => $"{ProductTagPrefix}{locationId}";
    public static string Resource(string locationId) => $"{ResourcePrefix}{locationId}";
    public static string Product(string locationId) => $"{ProductPrefix}{locationId}";
    public static bool IsProductTag(string id) => id.StartsWith(ProductTagPrefix, StringComparison.Ordinal);
    public static bool IsProduct(string id) => id.StartsWith(ProductPrefix, StringComparison.Ordinal);
}
