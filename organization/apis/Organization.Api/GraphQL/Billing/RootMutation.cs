using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Billing;

[MutationType]
public class RootMutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<OrganizationPayload> AddOrganizationBillingDetailsAsync(
        AddOrganizationBillingDetailsInput input,
        [Service] IOrganizationBillingService organizationBillingService,
        CancellationToken cancellationToken)
    {
        var organizationBillingDetails = await organizationBillingService.AddAsync(mapper.MapTo(input), cancellationToken);

        return new OrganizationPayload { ClientMutationId = input.ClientMutationId, Organization = mapper.MapTo(organizationBillingDetails)! };
    }

    [UseResolverScope]
    public async Task<OrganizationPayload> UpdateOrganizationBillingDetailsAsync(
        UpdateOrganizationBillingDetailsInput input,
        [Service] IOrganizationBillingService organizationBillingService,
        CancellationToken cancellationToken)
    {
        var organizationBillingDetails = await organizationBillingService.UpdateAsync(mapper.MapTo(input), cancellationToken);

        return new OrganizationPayload { ClientMutationId = input.ClientMutationId, Organization = mapper.MapTo(organizationBillingDetails)! };
    }
}
