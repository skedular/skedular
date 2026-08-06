using Api.Shared.Services.Offering;
using HotChocolate;

namespace Organization.Api.GraphQL.Pricing;

[GraphQLName("SpacesSubscriptionStatusDetails")]
public sealed class SpacesSubscriptionStatusDetails
{
    [GraphQLName("type")]
    public SpacesSubscriptionStatus Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}
