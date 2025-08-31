using Api.Shared.Services.Models;
using HotChocolate;

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
    [GraphQLName("locationTagIds")] public IEnumerable<string> LocationTagIds { get; set; } = [];
    [GraphQLName("contactEmail")] public string? ContactEmail { get; set; }
    [GraphQLName("contactPhone")] public string? ContactPhone { get; set; }
    [GraphQLName("primaryFeatureImage")] public CdnImageFile? PrimaryFeatureImage { get; set; }
    [GraphQLName("physicalAddress")] public LocationPhysicalAddressInput? PhysicalAddress { get; set; }
}
