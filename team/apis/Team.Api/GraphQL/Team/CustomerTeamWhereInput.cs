using HotChocolate;

namespace Team.Api.GraphQL.Team;

[GraphQLName("CustomerTeamWhereInput")]
public class CustomerTeamWhereInput
{
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("organizationUniqueAlphanumericName")]
    public string? OrganizationUniqueAlphanumericName { get; set; }

    [GraphQLName("customerId")] public string CustomerId { get; set; } = string.Empty;
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
    [GraphQLName("primaryLocationIds")] public IEnumerable<string>? PrimaryLocationIds { get; set; }
}
