using Api.Shared.Services.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("BookingWhereInput")]
public class BookingWhereInput
{
    [GraphQLName("fromGt")] public DateTimeOffset? FromGt { get; set; }
    [GraphQLName("fromGte")] public DateTimeOffset? FromGte { get; set; }
    [GraphQLName("fromLt")] public DateTimeOffset? FromLt { get; set; }
    [GraphQLName("fromLte")] public DateTimeOffset? FromLte { get; set; }
    [GraphQLName("toGt")] public DateTimeOffset? ToGt { get; set; }
    [GraphQLName("toGte")] public DateTimeOffset? ToGte { get; set; }
    [GraphQLName("toLt")] public DateTimeOffset? ToLt { get; set; }
    [GraphQLName("toLte")] public DateTimeOffset? ToLte { get; set; }
    [GraphQLName("notesContains")] public string? NotesContains { get; set; }
    [GraphQLName("type")] public BookingType? Type { get; set; }
    [GraphQLName("status")] public BookingStatus? Status { get; set; }
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
    [GraphQLName("organizationIds")] public IEnumerable<string>? OrganizationIds { get; set; }
    [GraphQLName("locationIds")] public IEnumerable<string>? LocationIds { get; set; }
    [GraphQLName("teamIds")] public IEnumerable<string>? TeamIds { get; set; }
    [GraphQLName("customerIds")] public IEnumerable<string>? CustomerIds { get; set; }
    [GraphQLName("includeMineOnly")] public bool? IncludeMineOnly { get; set; }

    [GraphQLName("includeFutureBookingsOnly")]
    public bool? IncludeFutureBookingsOnly { get; set; }
}
