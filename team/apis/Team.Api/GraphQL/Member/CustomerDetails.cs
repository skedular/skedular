using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Team.Api.GraphQL.Member;

[GraphQLName("CustomerDetails")]
public class CustomerDetails(string id) : Node(id);
