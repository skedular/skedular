using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Organization.Api.GraphQL.Member;

[GraphQLName("OrganizationMemberConnection")]
public class OrganizationMemberConnection : Connection<OrganizationMemberEdge>;
