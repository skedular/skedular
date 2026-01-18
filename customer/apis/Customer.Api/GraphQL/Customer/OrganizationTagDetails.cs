using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Customer.Api.GraphQL.Customer;

[GraphQLName("OrganizationTagDetails")]
public class OrganizationTagDetails(string id) : Node(id);
