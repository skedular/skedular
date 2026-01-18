using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Customer.Api.GraphQL.Customer;

[GraphQLName("ResourceDetails")]
public class ResourceDetails(string id) : Node(id);
