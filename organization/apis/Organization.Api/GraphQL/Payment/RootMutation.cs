using Enterprise.Shared.Payment.Configurations;
using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Payment;

[MutationType]
public class RootMutation(StripeConfiguration stripeConfiguration)
{
    [UseResolverScope]
    public async Task<AddOrganizationPaymentMethodIntentPayload> AddOrganizationPaymentMethodIntentAsync(
        AddOrganizationPaymentMethodIntentInput input,
        [Service] IPaymentService paymentService,
        CancellationToken cancellationToken)
    {
        var clientSecret =
            await paymentService.AddPaymentMethodIntentAsync(input.OrganizationId, input.OrganizationCustomDomain, cancellationToken);
        return new AddOrganizationPaymentMethodIntentPayload
        {
            ClientMutationId = input.ClientMutationId, ClientSecret = clientSecret, PublishedKeys = stripeConfiguration.PublishableKey
        };
    }

    [UseResolverScope]
    public async Task<RemoveOrganizationPaymentMethodPayload> RemoveOrganizationPaymentMethodAsync(
        RemoveOrganizationPaymentMethodInput input,
        [Service] IPaymentService paymentService,
        CancellationToken cancellationToken)
    {
        await paymentService.RemovePaymentMethodAsync(input.Id, cancellationToken);
        return new RemoveOrganizationPaymentMethodPayload { ClientMutationId = input.ClientMutationId };
    }
}
