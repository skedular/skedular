using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Organization.Api.GraphQL.Organization;

[GraphQLName("OrganizationConnection")]
public class OrganizationConnection : Connection<OrganizationEdge>;
