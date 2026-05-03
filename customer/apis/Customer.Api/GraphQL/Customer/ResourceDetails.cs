using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Composite;

namespace Customer.Api.GraphQL.Customer;

[GraphQLName("ResourceDetails")]
[EntityKey("id")]
public class ResourceDetails(string id) : Node(id);
