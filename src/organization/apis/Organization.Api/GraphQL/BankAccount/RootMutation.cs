using Enterprise.Shared.Sanitization;
using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.BankAccount;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<OrganizationBankAccountPayload> AddOrganizationBankAccountAsync(
        AddOrganizationBankAccountInput input,
        [Service] IOrganizationBankAccountService organizationBankAccountService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            OrganizationBankAccount =
                graphQlMapper.MapTo(await organizationBankAccountService.AddAsync(graphQlMapper.MapTo(input), cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationBankAccountPayload> UpdateOrganizationBankAccountAsync(
        UpdateOrganizationBankAccountInput input,
        [Service] IOrganizationBankAccountService organizationBankAccountService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            OrganizationBankAccount =
                graphQlMapper.MapTo(await organizationBankAccountService.UpdatePatchAsync(graphQlMapper.MapTo(input), cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationBankAccountPayload> DeleteOrganizationBankAccountAsync(
        DeleteOrganizationBankAccountInput input,
        [Service] IOrganizationBankAccountService organizationBankAccountService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            OrganizationBankAccount = graphQlMapper.MapTo(await organizationBankAccountService.DeleteAsync(input.Id, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationBankAccountsPayload> DeleteOrganizationBankAccountsAsync(
        DeleteOrganizationBankAccountsInput input,
        [Service] IOrganizationBankAccountService organizationBankAccountService,
        CancellationToken cancellationToken)
    {
        var resources = await organizationBankAccountService.DeleteAsync(input.Ids.RemoveInvalidIds().ToList(), cancellationToken);
        return new OrganizationBankAccountsPayload
        {
            ClientMutationId = input.ClientMutationId, OrganizationBankAccounts = resources.Select(graphQlMapper.MapTo)!
        };
    }

    [UseResolverScope]
    public async Task<OrganizationBankAccountPayload> SetOrganizationBankAccountAsDefaultAsync(
        SetOrganizationBankAccountAsDefaultInput input,
        [Service] IOrganizationBankAccountService organizationBankAccountService,
        CancellationToken cancellationToken)
    {
        var account = await organizationBankAccountService.SetAsDefaultAsync(input.Id, cancellationToken);
        return new OrganizationBankAccountPayload
        {
            ClientMutationId = input.ClientMutationId, OrganizationBankAccount = graphQlMapper.MapTo(account)!
        };
    }
}
