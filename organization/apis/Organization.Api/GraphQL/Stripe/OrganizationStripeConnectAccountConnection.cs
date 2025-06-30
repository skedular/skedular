using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Organization.Api.GraphQL.Stripe;

[GraphQLName("OrganizationStripeConnectAccountConnection")]
public class OrganizationStripeConnectAccountConnection : Connection<OrganizationStripeConnectAccountEdge>;
