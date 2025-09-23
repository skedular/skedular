using Customer.Api.GraphQL.Customer;
using Customer.Api.Mappers;
using Customer.Api.Services;
using HotChocolate;
using HotChocolate.Types;

namespace Customer.Api.GraphQL.Settings;

[MutationType]
public class RootMutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<CustomerPayload> CompleteOnboardingAsync(
        CompleteOrganizationOnboardingInput input,
        [Service] ICustomerSettingsService customerSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerSettingsService.CompleteOnboardingAsync(cancellationToken);
        return new CustomerPayload { ClientMutationId = input.ClientMutationId, Customer = mapper.MapTo(customer) };
    }

    [UseResolverScope]
    public async Task<CustomerPayload> AddCustomerPreferredLocationAsync(
        AddCustomerPreferredLocationInput input,
        [Service] ICustomerLocationSettingsService customerLocationSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerLocationSettingsService.AddCustomerPreferredLocationAsync(
            input.LocationId,
            null,
            false,
            cancellationToken);
        return new CustomerPayload { ClientMutationId = input.ClientMutationId, Customer = mapper.MapTo(customer) };
    }

    [UseResolverScope]
    public async Task<CustomerPayload> RemoveCustomerPreferredLocationAsync(
        RemoveCustomerPreferredLocationInput input,
        [Service] ICustomerLocationSettingsService customerLocationSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerLocationSettingsService.RemoveCustomerPreferredLocationAsync(
            input.LocationId,
            null,
            cancellationToken);
        return new CustomerPayload { ClientMutationId = input.ClientMutationId, Customer = mapper.MapTo(customer) };
    }

    [UseResolverScope]
    public async Task<CustomerPayload> SetCustomerDefaultOrganizationAsync(
        SetCustomerDefaultOrganizationInput input,
        [Service] ICustomerOrganizationSettingsService customerOrganizationSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerOrganizationSettingsService.SetCustomerDefaultOrganizationAsync(
            input.OrganizationId,
            input.OrganizationUniqueAlphanumericName,
            null,
            false,
            cancellationToken);
        return new CustomerPayload { ClientMutationId = input.ClientMutationId, Customer = mapper.MapTo(customer) };
    }

    [UseResolverScope]
    public async Task<CustomerPayload> ClearCustomerDefaultOrganizationAsync(
        ClearCustomerDefaultOrganizationInput input,
        [Service] ICustomerOrganizationSettingsService customerOrganizationSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerOrganizationSettingsService.ClearCustomerDefaultOrganizationAsync(null, cancellationToken);
        return new CustomerPayload { ClientMutationId = input.ClientMutationId, Customer = mapper.MapTo(customer) };
    }

    [UseResolverScope]
    public async Task<CustomerPayload> AddCustomerPreferredOrganizationTagAsync(
        AddCustomerPreferredOrganizationTagInput input,
        [Service] ICustomerOrganizationTagSettingsService customerOrganizationTagSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerOrganizationTagSettingsService.AddCustomerPreferredOrganizationTagAsync(
            input.OrganizationTagId,
            null,
            cancellationToken);
        return new CustomerPayload { ClientMutationId = input.ClientMutationId, Customer = mapper.MapTo(customer) };
    }

    [UseResolverScope]
    public async Task<CustomerPayload> RemoveCustomerPreferredOrganizationTagAsync(
        RemoveCustomerPreferredOrganizationTagInput input,
        [Service] ICustomerOrganizationTagSettingsService customerOrganizationTagSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerOrganizationTagSettingsService.RemoveCustomerPreferredOrganizationTagAsync(
            input.OrganizationTagId,
            null,
            cancellationToken);
        return new CustomerPayload { ClientMutationId = input.ClientMutationId, Customer = mapper.MapTo(customer) };
    }

    [UseResolverScope]
    public async Task<CustomerPayload> AddCustomerPreferredResourceAsync(
        AddCustomerPreferredResourceInput input,
        [Service] ICustomerResourceSettingsService customerResourceSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerResourceSettingsService.AddCustomerPreferredResourceAsync(input.ResourceId, null, cancellationToken);
        return new CustomerPayload { ClientMutationId = input.ClientMutationId, Customer = mapper.MapTo(customer) };
    }

    [UseResolverScope]
    public async Task<CustomerPayload> RemoveCustomerPreferredResourceAsync(
        RemoveCustomerPreferredResourceInput input,
        [Service] ICustomerResourceSettingsService customerResourceSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerResourceSettingsService.RemoveCustomerPreferredResourceAsync(input.ResourceId, null, cancellationToken);
        return new CustomerPayload { ClientMutationId = input.ClientMutationId, Customer = mapper.MapTo(customer) };
    }
}
