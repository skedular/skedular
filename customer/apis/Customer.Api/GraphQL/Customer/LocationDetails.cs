using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Composite;

namespace Customer.Api.GraphQL.Customer;

[GraphQLName("LocationDetails")]
[Shareable]
public class LocationDetails(string id) : Node(id);
