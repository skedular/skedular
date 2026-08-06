using Api.Shared.Services.Models;
using HotChocolate;
using Location.Api.Models;
using Location.Shared.Models;

namespace Location.Api.GraphQL.Location;

[GraphQLName("UpdateLocationInput")]
public class UpdateLocationInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("id")]
    public string Id { get; set; } = string.Empty;

    [GraphQLName("fieldsToUpdate")]
    public HashSet<LocationPatchField> FieldsToUpdate { get; set; } = [];

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;

    [GraphQLName("timezone")]
    public string? Timezone { get; set; }

    [GraphQLName("type")]
    public LocationType Type { get; set; }

    [GraphQLName("tagIds")]
    public IEnumerable<string> TagIds { get; set; } = [];

    [GraphQLName("featureImages")]
    public IEnumerable<CdnImageFile>? FeatureImages { get; set; } = [];

    [GraphQLName("extraMetadata")]
    public LocationExtraMetadata? ExtraMetadata { get; set; }

    [GraphQLName("listingMetadata")]
    public ListingMetadata? ListingMetadata { get; set; } = ListingMetadata.Empty;
}
