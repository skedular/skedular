using HotChocolate;

namespace Organization.Api.GraphQL;

[GraphQLName("OrganizationWhereInput")]
public class OrganizationWhereInput
{
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}
