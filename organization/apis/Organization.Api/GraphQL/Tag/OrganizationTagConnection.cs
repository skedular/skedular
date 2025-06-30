using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("OrganizationTagConnection")]
public class OrganizationTagConnection : Connection<OrganizationTagEdge>;
