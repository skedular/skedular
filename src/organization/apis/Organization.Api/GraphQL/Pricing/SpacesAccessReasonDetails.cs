using Api.Shared.Services.Offering;
using HotChocolate;

namespace Organization.Api.GraphQL.Pricing;

[GraphQLName("SpacesAccessReasonDetails")]
public sealed class SpacesAccessReasonDetails
{
    [GraphQLName("type")] public SpacesAccessReasonCode Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
