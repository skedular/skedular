using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("OrganizationDetails")]
public class OrganizationDetails(string id, string uniqueAlphanumericName) : Node(id)
{
    [GraphQLName("uniqueAlphanumericName")]
    public string? UniqueAlphanumericName { get; set; } = uniqueAlphanumericName;
}
