using Customer.Api.Mappers;
using Customer.Api.Services;
using HotChocolate;
using HotChocolate.Types;

namespace Customer.Api.GraphQL.Billing;

[MutationType]
public class Mutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<CustomerPayload?> AddMyBillingDetailsAsync(
        AddMyBillingDetailsInput input,
        [Service] IMyBillingService myBillingService,
        CancellationToken cancellationToken)
    {
        var customerBillingDetails = await myBillingService.AddAsync(mapper.MapTo(input), cancellationToken);

        return new CustomerPayload { ClientMutationId = input.ClientMutationId, Customer = mapper.MapTo(customerBillingDetails)! };
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> UpdateMyBillingDetailsAsync(
        UpdateMyBillingDetailsInput input,
        [Service] IMyBillingService myBillingService,
        CancellationToken cancellationToken)
    {
        var customerBillingDetails = await myBillingService.UpdateAsync(mapper.MapTo(input), cancellationToken);

        return new CustomerPayload { ClientMutationId = input.ClientMutationId, Customer = mapper.MapTo(customerBillingDetails)! };
    }
}
