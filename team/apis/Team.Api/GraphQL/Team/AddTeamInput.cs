using Api.Shared.Services.Models;
using HotChocolate;

namespace Team.Api.GraphQL.Team;

[GraphQLName("AddTeamInput")]
public class AddTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("about")] public string? About { get; set; }
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("organizationCustomDomain")]
    public string? OrganizationCustomDomain { get; set; }

    [GraphQLName("primaryLocationId")] public string? PrimaryLocationId { get; set; }
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("customerIds")] public IEnumerable<string> CustomerIds { get; set; } = [];
    [GraphQLName("organizationMemberIds")] public IEnumerable<string> OrganizationMemberIds { get; set; } = [];
    [GraphQLName("featureImages")] public IEnumerable<CdnImageFile>? FeatureImages { get; set; } = [];
}
