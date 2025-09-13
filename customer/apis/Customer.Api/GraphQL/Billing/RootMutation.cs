using Customer.Api.GraphQL.Customer;
using Customer.Api.Mappers;
using Customer.Api.Services;
using HotChocolate;
using HotChocolate.Types;

namespace Customer.Api.GraphQL.Billing;

[MutationType]
public class RootMutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<CustomerPayload> AddMyBillingDetailsAsync(
        AddMyBillingDetailsInput input,
        [Service] IBillingService billingService,
        CancellationToken cancellationToken)
    {
        var customerBillingDetails = await billingService.AddAsync(mapper.MapTo(input), cancellationToken);

        return new CustomerPayload { ClientMutationId = input.ClientMutationId, Customer = mapper.MapTo(customerBillingDetails) };
    }

    [UseResolverScope]
    public async Task<CustomerPayload> UpdateMyBillingDetailsAsync(
        UpdateMyBillingDetailsInput input,
        [Service] IBillingService billingService,
        CancellationToken cancellationToken)
    {
        var customerBillingDetails = await billingService.UpdateAsync(mapper.MapTo(input), cancellationToken);

        return new CustomerPayload { ClientMutationId = input.ClientMutationId, Customer = mapper.MapTo(customerBillingDetails) };
    }
}
