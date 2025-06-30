using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Organization.Api.GraphQL;

[GraphQLName("OrganizationConnection")]
public class OrganizationConnection : Connection<OrganizationEdge>;
