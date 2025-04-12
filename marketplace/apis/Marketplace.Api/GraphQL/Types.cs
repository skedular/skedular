using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types.Pagination;
using HotChocolate.Types.Relay;
using Marketplace.Shared.Models;

namespace Marketplace.Api.GraphQL;

[GraphQLName("CurrencyDetails")]
public class CurrencyDetails
{
    [GraphQLName("type")] public Currency Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}

[GraphQLName("PriceUnitDetails")]
public class PriceUnitDetails
{
    [GraphQLName("type")] public PriceUnit Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}

[GraphQLName("ProductWhereInput")]
public class ProductWhereInput
{
    [GraphQLName("organizationIds")] public IEnumerable<string>? OrganizationIds { get; set; }
    [GraphQLName("productIds")] public IEnumerable<string>? ProductIds { get; set; } = [];
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
    [GraphQLName("includeInactive")] public bool IncludeInactive { get; set; }
}

[GraphQLName("ProductOrderInput")]
public class ProductOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }
    [GraphQLName("field")] public ProductOrderField Field { get; set; }
}

[GraphQLName("ProductConnection")]
public class ProductConnection : Enterprise.Shared.GraphQL.Types.Connection<ProductEdge>;

[GraphQLName("ProductEdge")]
public class ProductEdge(ProductDetails node, string cursor) : Edge<ProductDetails>(node, cursor);

[GraphQLName("ProductPayload")]
public class ProductPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("product")] public ProductDetails Product { get; set; }
}

[GraphQLName("ProductDetails")]
public class ProductDetails : Node
{
    [GraphQLName("inactive")] public bool Inactive { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("description")] public string? Description { get; set; }
    [GraphQLName("price")] public required string Price { get; set; }
    [GraphQLName("priceToDisplay")] public required string PriceToDisplay { get; set; }
    [GraphQLName("priceUnit")] public PriceUnitDetails PriceUnit { get; set; }
    [GraphQLName("currency")] public CurrencyDetails Currency { get; set; }
    [GraphQLName("minDurationMinutes")] public int? MinDurationMinutes { get; set; }
    [GraphQLName("maxDurationMinutes")] public int? MaxDurationMinutes { get; set; }

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
    public required string LatestProductVersionId { get; set; }

    [GraphQLName("organization")] public OrganizationDetails Organization { get; set; }
    [GraphQLName("id")] [ID] public required string Id { get; set; }
}

[GraphQLName("Marketplace_OrganizationDetails")]
public class OrganizationDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
}

[GraphQLName("Marketplace_OrganizationTagDetails")]
public class OrganizationTagDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("tagType")] public string? TagType { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
}

[GraphQLName("AddProductInput")]
public class AddProductInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("description")] public string? Description { get; set; }
    [GraphQLName("price")] public required string Price { get; set; }
    [GraphQLName("priceUnit")] public PriceUnit PriceUnit { get; set; }
    [GraphQLName("currency")] public Currency Currency { get; set; }
    [GraphQLName("minDurationMinutes")] public int? MinDurationMinutes { get; set; }
    [GraphQLName("maxDurationMinutes")] public int? MaxDurationMinutes { get; set; }

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
}

[GraphQLName("UpdateProductInput")]
public class UpdateProductInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("description")] public string? Description { get; set; }
    [GraphQLName("price")] public required string Price { get; set; }
    [GraphQLName("priceUnit")] public PriceUnit PriceUnit { get; set; }
    [GraphQLName("currency")] public Currency Currency { get; set; }
    [GraphQLName("minDurationMinutes")] public int? MinDurationMinutes { get; set; }
    [GraphQLName("maxDurationMinutes")] public int? MaxDurationMinutes { get; set; }

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
}

[GraphQLName("DeleteProductsInput")]
public class DeleteProductsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public required IEnumerable<string> Ids { get; set; }
}

[GraphQLName("ActivateProductsInput")]
public class ActivateProductsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public required IEnumerable<string> Ids { get; set; }
}

[GraphQLName("DeactivateProductsInput")]
public class DeactivateProductsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public required IEnumerable<string> Ids { get; set; }
}

[GraphQLName("ProductsPayload")]
public class ProductsPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("products")] public IEnumerable<ProductDetails> Products { get; set; } = [];
}
