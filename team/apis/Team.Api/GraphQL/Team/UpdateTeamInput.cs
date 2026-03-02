using Api.Shared.Services.Models;
using HotChocolate;

namespace Team.Api.GraphQL.Team;

[GraphQLName("UpdateTeamInput")]
public class UpdateTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("about")] public string? About { get; set; }
    [GraphQLName("primaryLocationId")] public string? PrimaryLocationId { get; set; }
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("featureImages")] public IEnumerable<CdnImageFile> FeatureImages { get; set; } = [];
}
