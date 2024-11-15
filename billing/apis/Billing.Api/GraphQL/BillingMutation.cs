using Billing.Api.Mappers;
using Billing.Api.Services;
using Enterprise.Shared.Context;

namespace Billing.Api.GraphQL;

public class BillingMutation(IServiceProvider serviceProvider, IMapper mapper)
{
    public async Task<OrganizationBillingInfoPayload?> SetOrganizationBillingInfoAsync(
        SetOrganizationBillingInfoInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationBillingService>();
        var organization = await service.SetBillingInfoAsync(
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
