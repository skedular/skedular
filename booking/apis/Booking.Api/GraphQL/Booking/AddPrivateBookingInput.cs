using Api.Shared.Services.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("AddPrivateBookingInput")]
public class AddPrivateBookingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("customerIds")] public IEnumerable<string> CustomerIds { get; set; } = [];
    [GraphQLName("organizationIds")] public IEnumerable<string>? OrganizationIds { get; set; }

    [GraphQLName("organizationUniqueAlphanumericNames")]
    public IEnumerable<string>? OrganizationUniqueAlphanumericNames { get; set; }

    [GraphQLName("teamIds")] public IEnumerable<string> TeamIds { get; set; } = [];
    [GraphQLName("from")] public DateTimeOffset From { get; set; }
    [GraphQLName("until")] public DateTimeOffset Until { get; set; }
    [GraphQLName("notes")] public string? Notes { get; set; }
    [GraphQLName("category")] public BookingCategory Category { get; set; }
    [GraphQLName("resourceIds")] public IEnumerable<string> ResourceIds { get; set; } = [];
}
