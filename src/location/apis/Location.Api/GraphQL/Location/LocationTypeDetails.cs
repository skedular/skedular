using Api.Shared.Services.Models;
using HotChocolate;

namespace Location.Api.GraphQL.Location;

[GraphQLName("LocationTypeDetails")]
public class LocationTypeDetails
{
    [GraphQLName("type")]
    public LocationType Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}
