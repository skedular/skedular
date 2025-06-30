using Customer.Api.Mappers;
using Customer.Api.Services;
using HotChocolate;
using HotChocolate.Types;

namespace Customer.Api.GraphQL.Customer;

[QueryType]
public class RootQuery(IMapper mapper)
{
    [UseResolverScope]
    public async Task<CustomerDetails> MeAsync([Service] ICustomerService customerService, CancellationToken cancellationToken) =>
        mapper.MapTo(await customerService.GetMeAsync(true, cancellationToken));

    [UseResolverScope]
    public async Task<CustomerDetails?> CustomerAsync(string id, [Service] ICustomerService customerService, CancellationToken cancellationToken)
    {
        var customer = mapper.MapTo(await customerService.GetByIdAsync(id, false, cancellationToken));

        customer.PaymentMethods = [];

        return customer;
    }
}
