using HotChocolate;

namespace Customer.Api.GraphQL.Settings;

[GraphQLName("CustomersByPreferredLocationWhereInput")]
public class CustomersByPreferredLocationWhereInput
{
    [GraphQLName("locationId")] public string LocationId { get; set; } = string.Empty;
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}
