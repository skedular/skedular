using Billing.Api.Mappers;
using Billing.Api.Services;
using HotChocolate;
using HotChocolate.Types;

namespace Billing.Api.GraphQL;

[MutationType]
public class Mutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<OrganizationBillingInfoPayload?> SetOrganizationBillingInfoAsync(
        SetOrganizationBillingInfoInput input,
        [Service] IOrganizationBillingService organizationBillingService,
        CancellationToken cancellationToken)
    {
        var organization = await organizationBillingService.SetBillingInfoAsync(
            input.OrganizationId,
            input.Email,
            input.AddressLine1,
            input.AddressLine2,
            input.Suburb,
            input.City,
            input.Province,
            input.Zipcode,
            input.Country,
            cancellationToken);

        return mapper.MapTo(organization, input.ClientMutationId);
    }
}
