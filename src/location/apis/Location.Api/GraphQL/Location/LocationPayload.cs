using HotChocolate;

namespace Location.Api.GraphQL.Location;

[GraphQLName("LocationPayload")]
public class LocationPayload
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("location")]
    public LocationDetails Location { get; set; } = new();
}
