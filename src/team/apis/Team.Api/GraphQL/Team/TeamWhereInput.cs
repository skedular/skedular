using HotChocolate;

namespace Team.Api.GraphQL.Team;

[GraphQLName("TeamWhereInput")]
public class TeamWhereInput
{
    [GraphQLName("organizationId")]
    public string? OrganizationId { get; set; }

    [GraphQLName("organizationCustomDomain")]
    public string? OrganizationCustomDomain { get; set; }

    [GraphQLName("nameContains")]
    public string? NameContains { get; set; }

    [GraphQLName("primaryLocationIds")]
    public IEnumerable<string>? PrimaryLocationIds { get; set; }
}
