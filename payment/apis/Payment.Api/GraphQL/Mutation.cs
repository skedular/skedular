using HotChocolate;
using HotChocolate.Types;
using Payment.Api.Services;
using Payment.Shared.Configurations;

namespace Payment.Api.GraphQL;

[MutationType]
public class Mutation(StripeConfiguration stripeConfiguration)
{
    [UseResolverScope]
    public async Task<AddOrganizationPaymentMethodIntentResponse?> AddOrganizationPaymentMethodIntentAsync(
        AddOrganizationPaymentMethodIntentInput input,
        [Service] IPaymentService paymentService,
        CancellationToken cancellationToken)
    {
        var clientSecret = await paymentService.AddOrganizationPaymentMethodIntentAsync(
            input.OrganizationId,
            cancellationToken);
        return new AddOrganizationPaymentMethodIntentResponse
        {
            ClientMutationId = input.ClientMutationId, ClientSecret = clientSecret, PublishedKeys = stripeConfiguration.PublishableKey
        };
    }

    [UseResolverScope]
    public async Task<RemoveOrganizationPaymentMethodResponse?> RemoveOrganizationPaymentMethodAsync(
        RemoveOrganizationPaymentMethodInput input,
        [Service] IPaymentService paymentService,
        CancellationToken cancellationToken)
    {
        await paymentService.RemoveOrganizationPaymentMethodAsync(input.Id, cancellationToken);
        return new RemoveOrganizationPaymentMethodResponse { ClientMutationId = input.ClientMutationId };
    }
}
