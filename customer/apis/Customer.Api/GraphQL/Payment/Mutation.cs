using Customer.Api.Services;
using Enterprise.Shared.Payment.Configurations;
using HotChocolate;
using HotChocolate.Types;

namespace Customer.Api.GraphQL.Payment;

[MutationType]
public class Mutation(StripeConfiguration stripeConfiguration)
{
    [UseResolverScope]
    public async Task<AddCustomerPaymentMethodIntentPayload> AddCustomerPaymentMethodIntentAsync(
        AddCustomerPaymentMethodIntentInput input,
        [Service] IPaymentService paymentService,
        CancellationToken cancellationToken)
    {
        var clientSecret = await paymentService.AddPaymentMethodIntentAsync(cancellationToken);
        return new AddCustomerPaymentMethodIntentPayload
        {
            ClientMutationId = input.ClientMutationId, ClientSecret = clientSecret, PublishedKeys = stripeConfiguration.PublishableKey
        };
    }

    [UseResolverScope]
    public async Task<RemoveCustomerPaymentMethodPayload> RemoveCustomerPaymentMethodAsync(
        RemoveCustomerPaymentMethodInput input,
        [Service] IPaymentService paymentService,
        CancellationToken cancellationToken)
    {
        await paymentService.RemovePaymentMethodAsync(input.Id, cancellationToken);
        return new RemoveCustomerPaymentMethodPayload { ClientMutationId = input.ClientMutationId };
    }
}
