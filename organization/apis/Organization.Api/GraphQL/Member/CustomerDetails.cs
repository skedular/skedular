using HotChocolate;
using HotChocolate.Types.Relay;

namespace Organization.Api.GraphQL.Member;

[GraphQLName("Organization_CustomerDetails")]
public class CustomerDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("email")] public string? Email { get; set; }
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
    [GraphQLName("phoneNumber")] public string? PhoneNumber { get; set; }
}
