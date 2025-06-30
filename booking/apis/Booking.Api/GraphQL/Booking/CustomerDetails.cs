using HotChocolate;
using HotChocolate.Types.Relay;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("Booking_CustomerDetails")]
public class CustomerDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("designation")] public string? Designation { get; set; }
    [GraphQLName("title")] public string? Title { get; set; }
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("givenName")] public string? GivenName { get; set; }
    [GraphQLName("middleName")] public string? MiddleName { get; set; }
    [GraphQLName("familyName")] public string? FamilyName { get; set; }
    [GraphQLName("photoUrl")] public string? PhotoUrl { get; set; }
    [GraphQLName("photoUrl24")] public string? PhotoUrl24 { get; set; }
    [GraphQLName("photoUrl32")] public string? PhotoUrl32 { get; set; }
    [GraphQLName("photoUrl48")] public string? PhotoUrl48 { get; set; }
    [GraphQLName("photoUrl72")] public string? PhotoUrl72 { get; set; }
    [GraphQLName("photoUrl192")] public string? PhotoUrl192 { get; set; }
    [GraphQLName("photoUrl512")] public string? PhotoUrl512 { get; set; }
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("locale")] public string? Locale { get; set; }
    [GraphQLName("phoneNumber")] public string? PhoneNumber { get; set; }
}
