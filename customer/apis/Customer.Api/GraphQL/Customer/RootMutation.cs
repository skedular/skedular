using Customer.Api.Mappers;
using Customer.Api.Services;
using HotChocolate;
using HotChocolate.Types;

namespace Customer.Api.GraphQL.Customer;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<CustomerPayload> UpdateMyCustomerDetailsAsync(
        UpdateMyCustomerDetailsInput input,
        [Service] ICustomerDetailsService customerDetailsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerDetailsService.UpdateMyCustomerDetailsAsync(
            input.Timezone,
            input.Designation,
            input.Title,
            input.Name,
            input.GivenName,
            input.MiddleName,
            input.FamilyName,
            input.PhoneNumber,
            input.PersonalInformationVisibility,
            cancellationToken);
        return new CustomerPayload { ClientMutationId = input.ClientMutationId, Customer = graphQlMapper.MapTo(customer) };
    }

    [UseResolverScope]
    public async Task<CustomerPayload> UpdateCustomerDetailsAsync(
        UpdateCustomerDetailsInput input,
        [Service] ICustomerDetailsService customerDetailsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerDetailsService.UpdateCustomerDetailsAsync(
            input.Id,
            input.Timezone,
            input.Designation,
            input.Title,
            input.Name,
            input.GivenName,
            input.MiddleName,
            input.FamilyName,
            input.PhoneNumber,
            input.PersonalInformationVisibility,
            cancellationToken);

        return new CustomerPayload { ClientMutationId = input.ClientMutationId, Customer = graphQlMapper.MapTo(customer) };
    }
}
