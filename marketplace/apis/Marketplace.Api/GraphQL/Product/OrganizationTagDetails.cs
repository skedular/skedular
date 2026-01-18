using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("OrganizationTagDetails")]
public class OrganizationTagDetails(string id) : Node(id);
