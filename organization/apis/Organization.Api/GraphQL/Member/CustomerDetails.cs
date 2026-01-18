using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Organization.Api.GraphQL.Member;

[GraphQLName("CustomerDetails")]
public class CustomerDetails(string id) : Node(id);
