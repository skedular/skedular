using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("ProductDetails")]
public class ProductDetails : Node
{
    [GraphQLName("inactive")] public bool Inactive { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("description")] public string? Description { get; set; }
    [GraphQLName("price")] public string Price { get; set; } = string.Empty;
    [GraphQLName("priceToDisplay")] public string PriceToDisplay { get; set; } = string.Empty;
    [GraphQLName("currencyToDisplay")] public string CurrencyToDisplay { get; set; } = string.Empty;
    [GraphQLName("priceUnit")] public PriceUnitDetails PriceUnit { get; set; } = new();
    [GraphQLName("currency")] public CurrencyDetails Currency { get; set; } = new();
    [GraphQLName("minDurationMinutes")] public int? MinDurationMinutes { get; set; }
    [GraphQLName("maxDurationMinutes")] public int? MaxDurationMinutes { get; set; }

    [GraphQLName("maxAllowedResourcesLockTimePaidViaCard")]
    public int MaxAllowedResourcesLockTimePaidViaCard { get; set; }

    [GraphQLName("maxAllowedResourcesLockTimePaidViaBankTransfer")]
    public int MaxAllowedResourcesLockTimePaidViaBankTransfer { get; set; }

    [GraphQLName("acceptedBookingPaymentMethods")]
    public IEnumerable<PaymentMethodTypeDetails> AcceptedBookingPaymentMethods { get; set; } = [];

    [GraphQLName("bookAllLocationResources")]
    public bool BookAllLocationResources { get; set; }

    [GraphQLName("recurrenceWindowDays")] public int RecurrenceWindowDays { get; set; }

    [GraphQLName("requireConsecutiveDays")]
    public bool RequireConsecutiveDays { get; set; }

    [GraphQLName("maxBookingSpreadDays")] public int? MaxBookingSpreadDays { get; set; }

    [GraphQLName("numberOfResourcesToBook")]
    public int NumberOfResourcesToBook { get; set; }

    [GraphQLName("productTags")] public IEnumerable<OrganizationTagDetails> ProductTags { get; set; } = [];
    [GraphQLName("locationTags")] public IEnumerable<OrganizationTagDetails> LocationTags { get; set; } = [];

    [GraphQLName("latestProductVersionId")]
    public string LatestProductVersionId { get; set; } = string.Empty;

    [GraphQLName("organization")] public OrganizationDetails Organization { get; set; } = new();
    [GraphQLName("primaryFeatureImage")] public CdnImageFile? PrimaryFeatureImage { get; set; }
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}
