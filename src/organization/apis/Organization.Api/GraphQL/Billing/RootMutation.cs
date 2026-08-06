using HotChocolate;
using HotChocolate.Types;
using Organization.Api.GraphQL.Organization;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Billing;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<OrganizationPayload> UpdateOrganizationBillingDetailsAsync(
        UpdateOrganizationBillingDetailsInput input,
        [Service]
        IOrganizationBillingService organizationBillingService,
        CancellationToken cancellationToken)
    {
        var organizationBillingDetails = await organizationBillingService.UpdatePatchAsync(graphQlMapper.MapTo(input), cancellationToken);

        return new OrganizationPayload
        {
            ClientMutationId = input.ClientMutationId,
            Organization = graphQlMapper.MapTo(organizationBillingDetails)!,
        };
    }
}
