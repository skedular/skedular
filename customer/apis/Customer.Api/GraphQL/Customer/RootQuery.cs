using Api.Shared.Services.Models;
using Customer.Api.Mappers;
using Customer.Api.Services;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Composite;
using HotChocolate.Types.Relay;

namespace Customer.Api.GraphQL.Customer;

[QueryType]
public class RootQuery(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public IEnumerable<PersonalInformationVisibilityDetails> PersonalInformationVisibilityTypes() =>
    [
        new() { Type = PersonalInformationVisibility.Visible, Name = PersonalInformationVisibility.Visible.ToPersonalInformationVisibilityName() },
        new() { Type = PersonalInformationVisibility.Redacted, Name = PersonalInformationVisibility.Redacted.ToPersonalInformationVisibilityName() }
    ];

    [UseResolverScope]
    public async Task<CustomerDetails> MeAsync([Service] ICustomerService customerService, CancellationToken cancellationToken) =>
        graphQlMapper.MapTo(await customerService.GetMeAsync(true, cancellationToken));

    [UseResolverScope]
    public async Task<CustomerDetails?> CustomerAsync(string id, [Service] ICustomerService customerService, CancellationToken cancellationToken) =>
        graphQlMapper.MapTo(await customerService.GetByIdAsync(id, false, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<CustomerDetails?> CustomerByIdAsync(
        [ID] string id,
        [Service] ICustomerService customerService,
        CancellationToken cancellationToken) =>
        graphQlMapper.MapTo(await customerService.GetByIdAsync(id, true, cancellationToken));
}
