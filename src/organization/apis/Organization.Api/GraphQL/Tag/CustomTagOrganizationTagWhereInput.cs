using HotChocolate;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("CustomTagOrganizationTagWhereInput")]
public class CustomTagOrganizationTagWhereInput
{
    [GraphQLName("nameContains")]
    public string? NameContains { get; set; }
}
