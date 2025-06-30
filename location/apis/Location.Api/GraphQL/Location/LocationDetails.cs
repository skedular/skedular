using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;
using Location.Api.GraphQL.Resource;

namespace Location.Api.GraphQL.Location;

[GraphQLName("LocationDetails")]
public class LocationDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("about")] public string? About { get; set; }
    [GraphQLName("organization")] public OrganizationDetails Organization { get; set; } = new();
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("openingHours")] public OpeningHours OpeningHours { get; set; } = new();
    [GraphQLName("deskCapacity")] public int DeskCapacity { get; set; }
    [GraphQLName("roomCapacity")] public int RoomCapacity { get; set; }
    [GraphQLName("hasFutureBooking")] public bool HasFutureBooking { get; set; }
    [GraphQLName("canModify")] public bool CanModify { get; set; }
    [GraphQLName("canDelete")] public bool CanDelete { get; set; }
    [GraphQLName("canViewAnalytics")] public bool CanViewAnalytics { get; set; }
    [GraphQLName("resources")] public IEnumerable<ResourceDetails> Resources { get; set; } = [];
    [GraphQLName("physicalAddress")] public AddressDetails PhysicalAddress { get; set; } = new();
    [GraphQLName("customTags")] public IEnumerable<OrganizationTagDetails> CustomTags { get; set; } = [];
    [GraphQLName("zones")] public IEnumerable<OrganizationTagDetails> Zones { get; set; } = [];
    [GraphQLName("resourceTypes")] public IEnumerable<OrganizationTagDetails> ResourceTypes { get; set; } = [];
    [GraphQLName("locationTags")] public IEnumerable<OrganizationTagDetails> LocationTags { get; set; } = [];
    [GraphQLName("contactEmail")] public string? ContactEmail { get; set; }
    [GraphQLName("contactPhone")] public string? ContactPhone { get; set; }
    [GraphQLName("primaryFeatureImage")] public CdnImageFile? PrimaryFeatureImage { get; set; }
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}
