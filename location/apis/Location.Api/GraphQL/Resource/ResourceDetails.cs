using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using Location.Api.GraphQL.Location;

namespace Location.Api.GraphQL.Resource;

[GraphQLName("ResourceDetails")]
public class ResourceDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("inactive")] public bool Inactive { get; set; }

    [GraphQLName("requireBookingApproval")]
    public bool RequireBookingApproval { get; set; }

    [GraphQLName("color")] public string? Color { get; set; }
    [GraphQLName("capacity")] public int Capacity { get; set; }
    [GraphQLName("customTagIds")] public IEnumerable<string> CustomTagIds { get; set; } = [];
    [GraphQLName("zoneIds")] public IEnumerable<string> ZoneIds { get; set; } = [];
    [GraphQLName("productTagIds")] public IEnumerable<string> ProductTagIds { get; set; } = [];
    [GraphQLName("resourceTypeId")] public string ResourceTypeId { get; set; } = string.Empty;

    [GraphQLName("isAvailableHoursOverridden")]
    public bool IsAvailableHoursOverridden { get; set; }

    [GraphQLName("availableHours")] public OpeningHours? AvailableHours { get; set; }
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}

[ObjectType<ResourceDetails>]
public static partial class ResourceDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<ResourceDetails> descriptor)
    {
        descriptor.Ignore(item => item.CustomTagIds);
        descriptor.Ignore(item => item.ZoneIds);
        descriptor.Ignore(item => item.ProductTagIds);
        descriptor.Ignore(item => item.ResourceTypeId);
    }

    public static IEnumerable<OrganizationTagDetails> GetCustomTags([Parent] ResourceDetails item) =>
        item.CustomTagIds.Select(id => new OrganizationTagDetails(id));

    public static IEnumerable<OrganizationTagDetails> GetZones([Parent] ResourceDetails item) =>
        item.ZoneIds.Select(id => new OrganizationTagDetails(id));

    public static IEnumerable<OrganizationTagDetails> GetProductTags([Parent] ResourceDetails item) =>
        item.ProductTagIds.Select(id => new OrganizationTagDetails(id));

    public static OrganizationTagDetails GetResourceType([Parent] ResourceDetails item) => new(item.ResourceTypeId);
}
