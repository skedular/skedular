using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("ProductVersionDetails")]
public class ProductVersionDetails : Node
{
    [GraphQLName("inactive")] public bool Inactive { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("description")] public string? Description { get; set; }
    [GraphQLName("price")] public string Price { get; set; } = string.Empty;
    [GraphQLName("priceToDisplay")] public string PriceToDisplay { get; set; } = string.Empty;
    [GraphQLName("currencyToDisplay")] public string CurrencyToDisplay { get; set; } = string.Empty;
    [GraphQLName("priceUnit")] public PriceUnitDetails PriceUnit { get; set; } = new();
    [GraphQLName("isPriceTaxInclusive")] public bool IsPriceTaxInclusive { get; set; }
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

    [GraphQLName("productTagIds")] public IEnumerable<string> ProductTagIds { get; set; } = [];
    [GraphQLName("locationTagIds")] public IEnumerable<string> LocationTagIds { get; set; } = [];

    [GraphQLName("featureImages")] public ICollection<CdnImageFile> FeatureImages { get; set; } = [];
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}

[ObjectType<ProductVersionDetails>]
public static partial class ProductVersionDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<ProductVersionDetails> descriptor)
    {
        descriptor.Ignore(item => item.ProductTagIds);
        descriptor.Ignore(item => item.LocationTagIds);
    }

    public static IEnumerable<OrganizationTagDetails> GetProductTags([Parent] ProductVersionDetails item) =>
        item.ProductTagIds.Select(id => new OrganizationTagDetails(id));

    public static IEnumerable<OrganizationTagDetails> GetLocationTags([Parent] ProductVersionDetails item) =>
        item.LocationTagIds.Select(id => new OrganizationTagDetails(id));
}
