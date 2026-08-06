using HotChocolate;

// ReSharper disable ClassNeverInstantiated.Global

namespace Location.Api.GraphQL.Resource;

[GraphQLName("BulkAddResourceRowInput")]
public class BulkAddResourceRowInput
{
    [GraphQLName("organizationResourceTypeTagId")]
    public string OrganizationResourceTypeTagId { get; set; } = string.Empty;

    [GraphQLName("baseName")]
    public string? BaseName { get; set; }

    [GraphQLName("quantity")]
    public int Quantity { get; set; }

    [GraphQLName("customTagIds")]
    public IReadOnlyList<string> CustomTagIds { get; set; } = [];

    [GraphQLName("zoneIds")]
    public IReadOnlyList<string> ZoneIds { get; set; } = [];

    [GraphQLName("productTagIds")]
    public IReadOnlyList<string> ProductTagIds { get; set; } = [];
}
