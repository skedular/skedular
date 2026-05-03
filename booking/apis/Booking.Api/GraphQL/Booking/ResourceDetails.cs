using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Composite;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("ResourceDetails")]
[EntityKey("id")]
[Shareable]
public class ResourceDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("inactive")] public bool Inactive { get; set; }

    [GraphQLName("requireBookingApproval")]
    public bool RequireBookingApproval { get; set; }

    [GraphQLName("color")] public string? Color { get; set; }
    [GraphQLName("capacity")] public int Capacity { get; set; }

    [GraphQLName("isAvailableHoursOverridden")]
    public bool IsAvailableHoursOverridden { get; set; }

    [GraphQLName("customTags")] public IEnumerable<OrganizationTagDetails> CustomTags { get; set; } = [];
    [GraphQLName("zones")] public IEnumerable<OrganizationTagDetails> Zones { get; set; } = [];
    [GraphQLName("productTags")] public IEnumerable<OrganizationTagDetails> ProductTags { get; set; } = [];
    [GraphQLName("resourceType")] public OrganizationTagDetails ResourceType { get; set; } = new();
}
