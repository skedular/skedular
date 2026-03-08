using Api.Shared.Services.Models;
using HotChocolate;
using Location.Shared.Models;

// ReSharper disable ClassNeverInstantiated.Global

namespace Location.Api.GraphQL.Location;

[GraphQLName("AddLocationInput")]
public class AddLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("about")] public string? About { get; set; }
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("organizationUniqueAlphanumericName")]
    public string? OrganizationUniqueAlphanumericName { get; set; }

    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("type")] public LocationType Type { get; set; }
    [GraphQLName("tagIds")] public IEnumerable<string> TagIds { get; set; } = [];
    [GraphQLName("featureImages")] public IEnumerable<CdnImageFile> FeatureImages { get; set; } = [];
    [GraphQLName("physicalAddress")] public LocationPhysicalAddressInput? PhysicalAddress { get; set; }
    [GraphQLName("extraMetadata")] public LocationExtraMetadata? ExtraMetadata { get; set; }
    [GraphQLName("weekOpeningHours")] public WeekOpeningHours? WeekOpeningHours { get; set; }
}
