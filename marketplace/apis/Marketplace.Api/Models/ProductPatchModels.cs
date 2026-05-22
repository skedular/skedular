using HotChocolate;
using Marketplace.Shared.Models;

namespace Marketplace.Api.Models;

[GraphQLName("ProductPatchField")]
public enum ProductPatchField
{
    Type,
    Currency,
    Tags,
    FeatureImages,
    PricingOptions,
    ListingMetadata
}

public record ProductPatchRequest(
    string Id,
    IReadOnlySet<ProductPatchField> FieldsToUpdate,
    ProductVersion ProductVersion);
