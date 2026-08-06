using HotChocolate;

namespace Organization.Api.GraphQL.Pricing;

[GraphQLName("UpdateOrganizationTeamsSubscriptionPayload")]
public class UpdateOrganizationTeamsSubscriptionPayload
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("organizationTeamsSubscription")]
    public OrganizationTeamsSubscriptionDetails OrganizationTeamsSubscription { get; set; } = new();
}
