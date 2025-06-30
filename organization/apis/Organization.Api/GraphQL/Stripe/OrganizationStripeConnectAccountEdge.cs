using HotChocolate;
using HotChocolate.Types.Pagination;

namespace Organization.Api.GraphQL.Stripe;

[GraphQLName("OrganizationStripeConnectAccountEdge")]
public class OrganizationStripeConnectAccountEdge(OrganizationStripeConnectAccountDetails node, string cursor)
    : Edge<OrganizationStripeConnectAccountDetails>(node, cursor);
