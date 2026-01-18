using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("OrganizationTagDetails")]
public class OrganizationTagDetails(string id) : Node(id);
