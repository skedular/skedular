using Api.Shared.Services.Models;
using Booking.Api.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("UpdateMarketplaceBookingInput")]
public class UpdateMarketplaceBookingInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("id")]
    public required string Id { get; set; }

    [GraphQLName("fieldsToUpdate")]
    public HashSet<MarketplaceBookingPatchField> FieldsToUpdate { get; set; } = [];

    [GraphQLName("customerIds")]
    public IEnumerable<string> CustomerIds { get; set; } = [];

    [GraphQLName("organizationIds")]
    public IEnumerable<string>? OrganizationIds { get; set; }

    [GraphQLName("organizationCustomDomains")]
    public IEnumerable<string>? OrganizationCustomDomains { get; set; }

    [GraphQLName("teamIds")]
    public IEnumerable<string>? TeamIds { get; set; } = [];

    [GraphQLName("notes")]
    public string? Notes { get; set; }

    [GraphQLName("category")]
    public BookingCategory? Category { get; set; }
}
