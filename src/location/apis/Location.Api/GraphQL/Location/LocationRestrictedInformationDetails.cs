using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using Location.Shared.Models;

namespace Location.Api.GraphQL.Location;

[GraphQLName("LocationRestrictedInformationDetails")]
public class LocationRestrictedInformationDetails : Node
{
    [GraphQLName("title")]
    public string Title { get; set; } = string.Empty;

    [GraphQLName("category")]
    public LocationRestrictedInformationCategory Category { get; set; }

    [GraphQLName("content")]
    public string Content { get; set; } = string.Empty;

    [GraphQLName("active")]
    public bool Active { get; set; }

    [GraphQLName("sortOrder")]
    public int SortOrder { get; set; }

    [GraphQLName("locationId")]
    public string LocationId { get; set; } = string.Empty;
}
