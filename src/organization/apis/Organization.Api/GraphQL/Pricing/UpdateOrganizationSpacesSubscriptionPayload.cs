using HotChocolate;

namespace Organization.Api.GraphQL.Pricing;

[GraphQLName("UpdateOrganizationSpacesSubscriptionPayload")]
public class UpdateOrganizationSpacesSubscriptionPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("organizationSpacesSubscription")]
    public OrganizationSpacesSubscriptionDetails OrganizationSpacesSubscription { get; set; } = new();
}
