using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Composite;

namespace Team.Api.GraphQL.Member;

[GraphQLName("CustomerDetails")]
[Shareable]
public class CustomerDetails(string id) : Node(id);
