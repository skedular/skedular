using Api.Shared.Services.Models;
using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("UpdateProductInput")]
public class UpdateProductInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("description")] public string? Description { get; set; }
    [GraphQLName("price")] public string Price { get; set; } = string.Empty;
    [GraphQLName("priceUnit")] public PriceUnit PriceUnit { get; set; }
    [GraphQLName("isPriceTaxInclusive")] public bool IsPriceTaxInclusive { get; set; }
    [GraphQLName("currency")] public Currency Currency { get; set; }
    [GraphQLName("minDurationMinutes")] public int? MinDurationMinutes { get; set; }
    [GraphQLName("maxDurationMinutes")] public int? MaxDurationMinutes { get; set; }

    [GraphQLName("maxAllowedResourcesLockTimePaidViaCard")]
    public int MaxAllowedResourcesLockTimePaidViaCard { get; set; }

    [GraphQLName("maxAllowedResourcesLockTimePaidViaBankTransfer")]
    public int MaxAllowedResourcesLockTimePaidViaBankTransfer { get; set; }

    [GraphQLName("acceptedBookingPaymentMethods")]
    public IEnumerable<PaymentMethod> AcceptedBookingPaymentMethods { get; set; } = [];

    [GraphQLName("bookAllLocationResources")]
    public bool BookAllLocationResources { get; set; }

    [GraphQLName("numberOfResourcesToBook")]
    public int NumberOfResourcesToBook { get; set; }

    [GraphQLName("productTagIds")] public IEnumerable<string> ProductTagIds { get; set; } = [];
    [GraphQLName("locationTagIds")] public IEnumerable<string> LocationTagIds { get; set; } = [];
    [GraphQLName("featureImages")] public ICollection<CdnImageFile> FeatureImages { get; set; } = [];
}
