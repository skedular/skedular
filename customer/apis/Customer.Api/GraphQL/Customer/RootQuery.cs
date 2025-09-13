using Customer.Api.Mappers;
using Customer.Api.Services;
using HotChocolate;
using HotChocolate.Fusion.SourceSchema.Types;
using HotChocolate.Types;

namespace Customer.Api.GraphQL.Customer;

[QueryType]
public class RootQuery(IMapper mapper)
{
    [UseResolverScope]
    public async Task<CustomerDetails> MeAsync([Service] ICustomerService customerService, CancellationToken cancellationToken) =>
        mapper.MapTo(await customerService.GetMeAsync(true, cancellationToken));

    [UseResolverScope]
    public async Task<CustomerDetails?> CustomerAsync(string id, [Service] ICustomerService customerService, CancellationToken cancellationToken) =>
        mapper.MapTo(await customerService.GetByIdAsync(id, false, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<CustomerDetails?> CustomerByIdAsync(
        string id,
        [Service] ICustomerService customerService,
        CancellationToken cancellationToken) =>
        await CustomerAsync(id, customerService, cancellationToken);
}
