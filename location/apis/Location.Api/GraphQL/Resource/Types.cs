using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types.Pagination;
using HotChocolate.Types.Relay;
using Location.Api.GraphQL.Location;
using Location.Shared.Models;

// ReSharper disable ClassNeverInstantiated.Global

namespace Location.Api.GraphQL.Resource;

[GraphQLName("AddResourceInput")]
public class AddResourceInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("inactive")] public bool Inactive { get; set; }
    [GraphQLName("locationId")] public string LocationId { get; set; } = string.Empty;
    [GraphQLName("customTagIds")] public IEnumerable<string> CustomTagIds { get; set; } = [];
    [GraphQLName("zoneIds")] public IEnumerable<string> ZoneIds { get; set; } = [];
    [GraphQLName("productTagIds")] public IEnumerable<string> ProductTagIds { get; set; } = [];

    [GraphQLName("requireBookingApproval")]
    public bool RequireBookingApproval { get; set; }

    [GraphQLName("color")] public string? Color { get; set; }
    [GraphQLName("capacity")] public int Capacity { get; set; }

    [GraphQLName("organizationResourceTypeId")]
    public string OrganizationResourceTypeId { get; set; } = string.Empty;
}

[GraphQLName("UpdateResourceInput")]
public class UpdateResourceInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("inactive")] public bool Inactive { get; set; }

    [GraphQLName("requireBookingApproval")]
    public bool RequireBookingApproval { get; set; }

    [GraphQLName("color")] public string? Color { get; set; }
    [GraphQLName("capacity")] public int Capacity { get; set; }
    [GraphQLName("customTagIds")] public IEnumerable<string> CustomTagIds { get; set; } = [];
    [GraphQLName("zoneIds")] public IEnumerable<string> ZoneIds { get; set; } = [];
    [GraphQLName("productTagIds")] public IEnumerable<string> ProductTagIds { get; set; } = [];

    [GraphQLName("organizationResourceTypeId")]
    public string OrganizationResourceTypeId { get; set; } = string.Empty;
}

[GraphQLName("DeleteResourceInput")]
public class DeleteResourceInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}

[GraphQLName("ActivateResourcesInput")]
public class ActivateResourcesInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public IEnumerable<string> Ids { get; set; } = [];
}

[GraphQLName("DeactivateResourcesInput")]
public class DeactivateResourcesInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public IEnumerable<string> Ids { get; set; } = [];
}

[GraphQLName("DeleteResourcesInput")]
public class DeleteResourcesInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public IEnumerable<string> Ids { get; set; } = [];
}

[GraphQLName("ResourcesPayload")]
public class ResourcesPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("resources")] public IEnumerable<ResourceDetails> Resources { get; set; } = [];
}

[GraphQLName("ResourceConnection")]
public class ResourceConnection : Enterprise.Shared.GraphQL.Types.Connection<ResourceEdge>;

[GraphQLName("ResourceDetails")]
public class ResourceDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("inactive")] public bool Inactive { get; set; }

    [GraphQLName("requireBookingApproval")]
    public bool RequireBookingApproval { get; set; }

    [GraphQLName("color")] public string? Color { get; set; }
    [GraphQLName("capacity")] public int Capacity { get; set; }
    [GraphQLName("customTags")] public IEnumerable<OrganizationTagDetails> CustomTags { get; set; } = [];
    [GraphQLName("zones")] public IEnumerable<OrganizationTagDetails> Zones { get; set; } = [];
    [GraphQLName("productTags")] public IEnumerable<OrganizationTagDetails> ProductTags { get; set; } = [];
    [GraphQLName("resourceType")] public OrganizationTagDetails ResourceType { get; set; } = new();

    [GraphQLName("isAvailableHoursOverridden")]
    public bool IsAvailableHoursOverridden { get; set; }

    [GraphQLName("availableHours")] public OpeningHours? AvailableHours { get; set; }
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}

[GraphQLName("ResourceEdge")]
public class ResourceEdge(ResourceDetails node, string cursor) : Edge<ResourceDetails>(node, cursor);

[GraphQLName("ResourceOrderInput")]
public class ResourceOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }
    [GraphQLName("field")] public ResourceOrderField Field { get; set; }
}

[GraphQLName("ResourcePayload")]
public class ResourcePayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("resource")] public ResourceDetails Resource { get; set; } = new();
}

[GraphQLName("ResourceWhereInput")]
public class ResourceWhereInput
{
    [GraphQLName("locationId")] public string LocationId { get; set; } = string.Empty;
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
    [GraphQLName("customTagIds")] public IEnumerable<string>? CustomTagIds { get; set; }
    [GraphQLName("zoneIds")] public IEnumerable<string>? ZoneIds { get; set; }
    [GraphQLName("productTagIds")] public IEnumerable<string>? ProductTagIds { get; set; }
    [GraphQLName("floorPlanId")] public string? FloorPlanId { get; set; }
}

[GraphQLName("UpdateLocationResourceAvailableHoursInput")]
public class UpdateLocationResourceAvailableHoursInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;

    [GraphQLName("overrideAvailableHours")]
    public bool OverrideAvailableHours { get; set; }

    [GraphQLName("availableHours")] public WeekOpeningHours? AvailableHours { get; set; }
}
