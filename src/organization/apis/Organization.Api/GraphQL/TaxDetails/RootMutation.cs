using HotChocolate;
using HotChocolate.Types;
using Organization.Api.GraphQL.Organization;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.TaxDetails;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<OrganizationPayload> UpdateOrganizationTaxDetailsAsync(
        UpdateOrganizationTaxDetailsInput input,
        [Service]
        IOrganizationTaxDetailsService organizationTaxDetailsService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Organization = graphQlMapper.MapTo(
                await organizationTaxDetailsService.UpdatePatchAsync(graphQlMapper.MapTo(input), cancellationToken))!,
        };

    [UseResolverScope]
    public async Task<OrganizationPayload> RemoveOrganizationTaxDetailsAsync(
        RemoveOrganizationTaxDetailsInput input,
        [Service]
        IOrganizationTaxDetailsService organizationTaxDetailsService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Organization = graphQlMapper.MapTo(await organizationTaxDetailsService.RemoveAsync(
                input.OrganizationId,
                input.OrganizationCustomDomain,
                cancellationToken))!,
        };
}
