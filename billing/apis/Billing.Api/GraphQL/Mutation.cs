using Billing.Api.Mappers;
using Billing.Api.Services;
using HotChocolate;
using HotChocolate.Types;

namespace Billing.Api.GraphQL;

[MutationType]
public class Mutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<MyBillingContactDetailsPayload?> UpdateMyBillingContactDetailsAsync(
        UpdateMyBillingContactDetailsInput input,
        [Service] ICustomerBillingService customerBillingService,
        CancellationToken cancellationToken)
    {
        var customer = await customerBillingService.UpdateMyBillingInfoAsync(
            input.CompanyName,
            input.Email,
            input.AddressLine1,
            input.AddressLine2,
            input.Suburb,
            input.City,
            input.Province,
            input.Zipcode,
            input.Country,
            cancellationToken);

        return mapper.MapTo(customer, input.ClientMutationId);
    }
}
