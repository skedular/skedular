using Enterprise.Shared.Pagination;
using HotChocolate;
using Organization.Shared.Models;

namespace Organization.Api.GraphQL.Stripe;

[GraphQLName("OrganizationStripeConnectAccountOrderInput")]
public class OrganizationStripeConnectAccountOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }
    [GraphQLName("field")] public OrganizationStripeConnectAccountOrderField Field { get; set; }
}
