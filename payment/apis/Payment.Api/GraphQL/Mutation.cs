using Enterprise.Shared.Sanitization;
using HotChocolate;
using HotChocolate.Types;
using Payment.Api.Mappers;
using Payment.Api.Services;
using Payment.Shared.Configurations;

namespace Payment.Api.GraphQL;

[MutationType]
public class Mutation(StripeConfiguration stripeConfiguration, IMapper mapper)
{
    [UseResolverScope]
    public async Task<AddOrganizationPaymentMethodIntentPayload?> AddOrganizationPaymentMethodIntentAsync(
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

    [UseResolverScope]
    public async Task<AddCustomerPaymentMethodIntentPayload?> AddMyPaymentMethodIntentAsync(
        AddMyPaymentMethodIntentInput input,
        [Service] ICustomerPaymentService customerPaymentService,
        CancellationToken cancellationToken)
    {
        var clientSecret = await customerPaymentService.AddPaymentMethodIntentAsync(cancellationToken);
        return new AddCustomerPaymentMethodIntentPayload
        {
            ClientMutationId = input.ClientMutationId, ClientSecret = clientSecret, PublishedKeys = stripeConfiguration.PublishableKey
        };
    }

    [UseResolverScope]
    public async Task<RemoveCustomerPaymentMethodPayload?> RemoveMyPaymentMethodAsync(
        RemoveMyPaymentMethodInput input,
        [Service] ICustomerPaymentService customerPaymentService,
        CancellationToken cancellationToken)
    {
        await customerPaymentService.RemovePaymentMethodAsync(input.Id, cancellationToken);
        return new RemoveCustomerPaymentMethodPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseResolverScope]
    public async Task<OrganizationStripeConnectAccountPayload?> AddOrganizationStripeConnectAccountAsync(
        AddOrganizationStripeConnectAccountInput input,
        [Service] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        CancellationToken cancellationToken)
    {
        var account = await organizationStripeConnectAccountService.AddAsync(input.Id, input.OrganizationId, input.Name, cancellationToken);
        return new OrganizationStripeConnectAccountPayload { ClientMutationId = input.ClientMutationId, Account = mapper.MapTo(account)! };
    }

    [UseResolverScope]
    public async Task<OrganizationStripeConnectAccountPayload?> UpdateOrganizationStripeConnectAccountAsync(
        UpdateOrganizationStripeConnectAccountInput input,
        [Service] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        CancellationToken cancellationToken)
    {
        var account = await organizationStripeConnectAccountService.UpdateAsync(input.Id, input.Name, cancellationToken);
        return new OrganizationStripeConnectAccountPayload { ClientMutationId = input.ClientMutationId, Account = mapper.MapTo(account)! };
    }

    [UseResolverScope]
    public async Task<OrganizationStripeConnectAccountPayload?> DeleteOrganizationStripeConnectAccountAsync(
        DeleteOrganizationStripeConnectAccountInput input,
        [Service] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        CancellationToken cancellationToken)
    {
        var account = await organizationStripeConnectAccountService.DeleteAsync(input.Id, cancellationToken);
        return new OrganizationStripeConnectAccountPayload { ClientMutationId = input.ClientMutationId, Account = mapper.MapTo(account)! };
    }

    [UseResolverScope]
    public async Task<OrganizationStripeConnectAccountsPayload?> DeleteOrganizationStripeConnectAccountsAsync(
        DeleteOrganizationStripeConnectAccountsInput input,
        [Service] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        CancellationToken cancellationToken)
    {
        var accounts = await organizationStripeConnectAccountService.DeleteAsync(input.Ids.RemoveInvalidIds()!.ToList(), cancellationToken);
        return new OrganizationStripeConnectAccountsPayload
        {
            ClientMutationId = input.ClientMutationId, Accounts = accounts.Select(item => mapper.MapTo(item)!)
        };
    }
}
