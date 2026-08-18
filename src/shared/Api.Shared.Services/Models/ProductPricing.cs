using System.Text.Json;
using System.Text.Json.Serialization;
using HotChocolate.Types.Composite;

namespace Api.Shared.Services.Models;

[Shareable]
public record ProductPricing(
    string Id,
    int Index,
    ListingMetadata ListingMetadata,
    ProductPricingCadence PurchaseCadence,
    ProductPricingCadence BookingCadence,
    decimal Price,
    bool IsTaxInclusive,
    bool SupportsSubscriptionAutoRenewal,
    IReadOnlyList<PaymentMethod> AcceptedPaymentMethods,
    ProductPricingBillingMode BillingMode,
    int? MinDurationMinutes,
    int? MaxDurationMinutes,
    int MaxAllowedResourcesLockTimePaidViaCard,
    int MaxAllowedResourcesLockTimePaidViaBankTransfer,
    int NumberOfResourcesToBook,
    ProductPricingCancellationPolicyType CancellationPolicyType,
    IReadOnlyList<ProductPricingCancellationRefundRule> CancellationRefundRules,
    [property: JsonConverter(typeof(DayOfWeekListJsonConverter))]
    IReadOnlyList<DayOfWeek>? AvailableDays = null,
    int? RequiredDaysPerWeek = null,
    ProductPricingFulfillmentType FulfillmentType = ProductPricingFulfillmentType.Reservation,
    int? EntitlementCreditQuantity = null,
    int? EntitlementValidityDays = null)
{
    public static ProductPricing Empty(string id) =>
        new(
            id,
            int.MinValue,
            ListingMetadata.Empty,
            ProductPricingCadence.NotSet,
            ProductPricingCadence.NotSet,
            int.MinValue,
            false,
            false,
            [],
            ProductPricingBillingMode.NotSet,
            null,
            null,
            int.MinValue,
            int.MinValue,
            int.MinValue,
            ProductPricingCancellationPolicyType.NotSet,
            []);
}

/// <summary>Persists price availability as readable JSON strings while retaining enum models.</summary>
public sealed class DayOfWeekListJsonConverter : JsonConverter<IReadOnlyList<DayOfWeek>>
{
    public override IReadOnlyList<DayOfWeek> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var days = JsonSerializer.Deserialize<List<string>>(ref reader, options) ?? [];
        return [.. days.Select(Enum.Parse<DayOfWeek>)];
    }

    public override void Write(Utf8JsonWriter writer, IReadOnlyList<DayOfWeek> value, JsonSerializerOptions options) =>
        JsonSerializer.Serialize(writer, value.Select(item => item.ToString()).ToList(), options);
}
