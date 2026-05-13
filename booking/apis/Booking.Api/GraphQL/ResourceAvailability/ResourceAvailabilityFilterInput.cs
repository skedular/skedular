using Booking.Shared.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.ResourceAvailability;

[GraphQLName("ResourceAvailabilityFilterInput")]
public class ResourceAvailabilityFilterInput
{
    [GraphQLName("date")] public required DateOnly Date { get; set; }

    [GraphQLName("organizationCustomDomain")]
    public string OrganizationCustomDomain { get; set; } = string.Empty;

    [GraphQLName("locationIds")] public IEnumerable<string> LocationIds { get; set; } = [];
    [GraphQLName("floorId")] public string? FloorId { get; set; }
    [GraphQLName("zoneId")] public string? ZoneId { get; set; }
    [GraphQLName("resourceType")] public string? ResourceType { get; set; }
    [GraphQLName("statuses")] public IEnumerable<ResourceAvailabilityClassification> Statuses { get; set; } = [];
}
