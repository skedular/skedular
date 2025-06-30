using Api.Shared.Services.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("AddBookingInput")]
public class AddBookingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("customerIds")] public IEnumerable<string> CustomerIds { get; set; } = [];
    [GraphQLName("organizationIds")] public IEnumerable<string> OrganizationIds { get; set; } = [];
    [GraphQLName("teamIds")] public IEnumerable<string> TeamIds { get; set; } = [];
    [GraphQLName("from")] public DateTimeOffset From { get; set; }
    [GraphQLName("until")] public DateTimeOffset Until { get; set; }
    [GraphQLName("notes")] public string? Notes { get; set; }
    [GraphQLName("type")] public BookingType Type { get; set; }
    [GraphQLName("resourceIds")] public IEnumerable<string> ResourceIds { get; set; } = [];
    [GraphQLName("lineItems")] public IEnumerable<LineItemInput> LineItems { get; set; } = [];
}
