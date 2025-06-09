using Enterprise.Shared.Payment.Configurations;
using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Payment;

[MutationType]
public class Mutation(StripeConfiguration stripeConfiguration)
{
    [UseResolverScope]
    public async Task<AddOrganizationPaymentMethodIntentPayload> AddOrganizationPaymentMethodIntentAsync(
        AddOrganizationPaymentMethodIntentInput input,
        [Service] IOrganizationPaymentService organizationPaymentService,
        CancellationToken cancellationToken)
    {
        var clientSecret = await organizationPaymentService.AddPaymentMethodIntentAsync(input.OrganizationId, cancellationToken);
        return new AddOrganizationPaymentMethodIntentPayload
        {
            ClientMutationId = input.ClientMutationId, ClientSecret = clientSecret, PublishedKeys = stripeConfiguration.PublishableKey
        };
    }

    [UseResolverScope]
    public async Task<RemoveOrganizationPaymentMethodPayload?> RemoveOrganizationPaymentMethodAsync(
        RemoveOrganizationPaymentMethodInput input,
        [Service] IOrganizationPaymentService organizationPaymentService,
        CancellationToken cancellationToken)
    {
        await organizationPaymentService.RemovePaymentMethodAsync(input.Id, cancellationToken);
        return new RemoveOrganizationPaymentMethodPayload { ClientMutationId = input.ClientMutationId };
    }
}
