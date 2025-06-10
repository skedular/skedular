using Enterprise.Shared.Payment.Configurations;
using Enterprise.Shared.Sanitization;
using HotChocolate;
using HotChocolate.Types;
using Payment.Api.Mappers;
using Payment.Api.Services;

namespace Payment.Api.GraphQL;

[MutationType]
public class Mutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<OrganizationStripeConnectAccountPayload?> AddOrganizationStripeConnectAccountAsync(
        AddOrganizationStripeConnectAccountInput input,
        [Service] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        CancellationToken cancellationToken)
    {
        var account = await organizationStripeConnectAccountService.AddAsync(input.Id, input.OrganizationId, input.Name, input.RedirectUrl,
            cancellationToken);
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
