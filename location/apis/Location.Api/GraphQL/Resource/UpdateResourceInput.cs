using HotChocolate;
using Location.Api.Models;

namespace Location.Api.GraphQL.Resource;

[GraphQLName("UpdateResourceInput")]
public class UpdateResourceInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("fieldsToUpdate")] public HashSet<ResourcePatchField> FieldsToUpdate { get; set; } = [];
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
