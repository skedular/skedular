using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Composite;

namespace Team.Api.GraphQL.Member;

[GraphQLName("CustomerDetails")]
[EntityKey("id")]
public class CustomerDetails(string id) : Node(id);
