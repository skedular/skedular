using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Composite;

namespace Organization.Api.GraphQL.Member;

[GraphQLName("CustomerDetails")]
[EntityKey("id")]
public class CustomerDetails(string id) : Node(id);
