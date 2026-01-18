using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Customer.Api.GraphQL.Customer;

[GraphQLName("LocationDetails")]
public class LocationDetails(string id) : Node(id);
