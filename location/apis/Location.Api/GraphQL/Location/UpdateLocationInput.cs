using Api.Shared.Services.Models;
using HotChocolate;
using Location.Shared.Models;

namespace Location.Api.GraphQL.Location;

[GraphQLName("UpdateLocationInput")]
public class UpdateLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("about")] public string? About { get; set; }
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("type")] public LocationType Type { get; set; }
    [GraphQLName("locationTagIds")] public IEnumerable<string> LocationTagIds { get; set; } = [];
    [GraphQLName("featureImages")] public IEnumerable<CdnImageFile> FeatureImages { get; set; } = [];
    [GraphQLName("extraMetadata")] public LocationExtraMetadata? ExtraMetadata { get; set; }
}
