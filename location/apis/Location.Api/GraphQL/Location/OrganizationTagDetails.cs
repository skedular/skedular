using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Location.Api.GraphQL.Location;

[GraphQLName("OrganizationTagDetails")]
public class OrganizationTagDetails(string id) : Node(id);
