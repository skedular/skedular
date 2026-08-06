using HotChocolate;
using Location.Shared.Models;

namespace Location.Api.GraphQL.Location;

[GraphQLName("LocationRestrictedInformationCategoryDetails")]
public class LocationRestrictedInformationCategoryDetails
{
    [GraphQLName("category")]
    public LocationRestrictedInformationCategory Category { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}
