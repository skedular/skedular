namespace Api.Shared.Services.Models;

public enum ProductPricingFulfillmentType
{
    Reservation = 0,
    Entitlement = 1,
}

public static class ProductPricingFulfillmentTypeExtensions
{
    public static string ToPersistedValue(this ProductPricingFulfillmentType value) => value switch
    {
        ProductPricingFulfillmentType.Reservation => "RESERVATION",
        ProductPricingFulfillmentType.Entitlement => "ENTITLEMENT",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
    };

    public static ProductPricingFulfillmentType FromPersistedValue(this string value) => value switch
    {
        "RESERVATION" => ProductPricingFulfillmentType.Reservation,
        "ENTITLEMENT" => ProductPricingFulfillmentType.Entitlement,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported fulfillment type value."),
    };

    public static string ToProductPricingFulfillmentTypeName(this ProductPricingFulfillmentType value) => value switch
    {
        ProductPricingFulfillmentType.Reservation => "Reservation",
        ProductPricingFulfillmentType.Entitlement => "Entitlement",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
    };
}
