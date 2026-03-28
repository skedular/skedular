using Enterprise.Shared.Sanitization;
using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Stripe;

[MutationType]
public class RootMutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<OrganizationStripeConnectAccountPayload> AddOrganizationStripeConnectAccountAsync(
        AddOrganizationStripeConnectAccountInput input,
        [Service] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            OrganizationStripeConnectAccount = mapper.MapTo(
                await organizationStripeConnectAccountService.AddAsync(
                    input.Id,
                    input.OrganizationId,
                    input.OrganizationCustomDomain,
                    input.Name,
                    input.RedirectUrl,
                    cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationStripeConnectAccountPayload> UpdateOrganizationStripeConnectAccountAsync(
        UpdateOrganizationStripeConnectAccountInput input,
        [Service] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            OrganizationStripeConnectAccount =
                mapper.MapTo(await organizationStripeConnectAccountService.UpdateAsync(input.Id, input.Name, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationStripeConnectAccountPayload> DeleteOrganizationStripeConnectAccountAsync(
        DeleteOrganizationStripeConnectAccountInput input,
        [Service] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            OrganizationStripeConnectAccount =
                mapper.MapTo(await organizationStripeConnectAccountService.DeleteAsync(input.Id, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationStripeConnectAccountsPayload> DeleteOrganizationStripeConnectAccountsAsync(
        DeleteOrganizationStripeConnectAccountsInput input,
        [Service] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        CancellationToken cancellationToken)
    {
        var accounts = await organizationStripeConnectAccountService.DeleteAsync(input.Ids.RemoveInvalidIds().ToList(), cancellationToken);
        return new OrganizationStripeConnectAccountsPayload
        {
            ClientMutationId = input.ClientMutationId, OrganizationStripeConnectAccounts = accounts.Select(item => mapper.MapTo(item)!)
        };
    }

    [UseResolverScope]
    public async Task<OrganizationStripeConnectAccountPayload> SetOrganizationStripeConnectAccountAsDefaultAsync(
        SetOrganizationStripeConnectAccountAsDefaultInput input,
        [Service] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        CancellationToken cancellationToken)
    {
        var account = await organizationStripeConnectAccountService.SetAsDefaultAsync(input.Id, cancellationToken);
        return new OrganizationStripeConnectAccountPayload
        {
            ClientMutationId = input.ClientMutationId, OrganizationStripeConnectAccount = mapper.MapTo(account)!
        };
    }
}
