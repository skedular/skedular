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
        [Service] IOrganizationPaymentService organizationPaymentService,
        CancellationToken cancellationToken)
    {
        var clientSecret = await organizationPaymentService.AddPaymentMethodIntentAsync(input.OrganizationId, cancellationToken);
        return new AddOrganizationPaymentMethodIntentResponse
        {
            ClientMutationId = input.ClientMutationId, ClientSecret = clientSecret, PublishedKeys = stripeConfiguration.PublishableKey
        };
    }

    [UseResolverScope]
    public async Task<RemoveOrganizationPaymentMethodResponse?> RemoveOrganizationPaymentMethodAsync(
        RemoveOrganizationPaymentMethodInput input,
        [Service] IOrganizationPaymentService organizationPaymentService,
        CancellationToken cancellationToken)
    {
        await organizationPaymentService.RemovePaymentMethodAsync(input.Id, cancellationToken);
        return new RemoveOrganizationPaymentMethodResponse { ClientMutationId = input.ClientMutationId };
    }

    [UseResolverScope]
    public async Task<AddOrganizationStripeConnectAccountResponse?> AddOrganizationStripeConnectAccountAsync(
        AddOrganizationStripeConnectAccountInput input,
        [Service] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        CancellationToken cancellationToken)
    {
        await organizationStripeConnectAccountService.AddAsync(input.OrganizationId, input.Name, cancellationToken);
        return new AddOrganizationStripeConnectAccountResponse { ClientMutationId = input.ClientMutationId };
    }
}
