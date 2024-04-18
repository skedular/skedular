using Api.Shared.Services.GraphQL.UnityHub.V1.Payment;
using Enterprise.Shared.Context;
using Payment.Api.Services;
using Payment.Shared.Configurations;

namespace Payment.Api.GraphQL;

public class PaymentMutation : Mutation
{
    public override async Task<AddOrganizationPaymentMethodIntentResponse?> AddOrganizationPaymentMethodIntentAsync(
        AddOrganizationPaymentMethodIntentInput input,
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var stripeConfiguration = scope.ServiceProvider.GetRequiredService<StripeConfiguration>();
        var service = scope.ServiceProvider.GetRequiredService<IPaymentService>();
        var clientSecret =
            await service.AddOrganizationPaymentMethodIntentAsync(input.OrganizationId, cancellationToken);
        return new AddOrganizationPaymentMethodIntentResponse
        {
            ClientMutationId = input.ClientMutationId,
            ClientSecret = clientSecret,
            PublishedKeys = stripeConfiguration.PublishableKey
        };
    }

    public override async Task<RemoveOrganizationPaymentMethodResponse?> RemoveOrganizationPaymentMethodAsync(
        RemoveOrganizationPaymentMethodInput input, IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IPaymentService>();
        await service.RemoveOrganizationPaymentMethodAsync(input.Id, cancellationToken);
        return new RemoveOrganizationPaymentMethodResponse { ClientMutationId = input.ClientMutationId };
    }
}
