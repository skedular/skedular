using Customer.Api.Mappers;
using Customer.Api.Services;
using HotChocolate;

namespace Customer.Api.GraphQL;

public class CustomerMutation
{
    public async Task<CustomerPayload?> CompleteOrganizationOnboardingAsync(
        CompleteOrganizationOnboardingInput input,
        [Service] ICustomerSettingsService customerSettingsService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var customer = await customerSettingsService.CompleteOrganizationOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> CompleteLocationOnboardingAsync(
        CompleteLocationOnboardingInput input,
        [Service] ICustomerSettingsService customerSettingsService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var customer = await customerSettingsService.CompleteLocationOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> CompleteTeamOnboardingAsync(
        CompleteTeamOnboardingInput input,
        [Service] ICustomerSettingsService customerSettingsService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var customer = await customerSettingsService.CompleteTeamOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> CompleteDefaultOrganizationOnboardingAsync(
        CompleteDefaultOrganizationOnboardingInput input,
        [Service] ICustomerSettingsService customerSettingsService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var customer = await customerSettingsService.CompleteDefaultOrganizationOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> CompleteDefaultLocationOnboardingAsync(
        CompleteDefaultLocationOnboardingInput input,
        [Service] ICustomerSettingsService customerSettingsService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var customer = await customerSettingsService.CompleteDefaultLocationOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> CompletePreferredZoneOnboardingAsync(
        CompletePreferredZoneOnboardingInput input,
        [Service] ICustomerSettingsService customerSettingsService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var customer = await customerSettingsService.CompletePreferredZoneOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> CompletePreferredDeskOnboardingAsync(
        CompletePreferredDeskOnboardingInput input,
        [Service] ICustomerSettingsService customerSettingsService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var customer = await customerSettingsService.CompletePreferredDeskOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> AddCustomerDefaultLocationAsync(
        AddCustomerDefaultLocationInput input,
        [Service] ICustomerLocationSettingsService customerLocationSettingsService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var customer = await customerLocationSettingsService.AddCustomerDefaultLocationAsync(
            input.LocationId,
            null,
            false,
            cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> RemoveCustomerDefaultLocationAsync(
        RemoveCustomerDefaultLocationInput input,
        [Service] ICustomerLocationSettingsService customerLocationSettingsService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var customer = await customerLocationSettingsService.RemoveCustomerDefaultLocationAsync(
            input.LocationId,
            null,
            cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> AddCustomerDefaultTeamAsync(
        AddCustomerDefaultTeamInput input,
        [Service] ICustomerTeamSettingsService customerTeamSettingsService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var customer = await customerTeamSettingsService.AddCustomerDefaultTeamAsync(
            input.TeamId,
            null,
            false,
            cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> RemoveCustomerDefaultTeamAsync(
        RemoveCustomerDefaultTeamInput input,
        [Service] ICustomerTeamSettingsService customerTeamSettingsService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var customer = await customerTeamSettingsService.RemoveCustomerDefaultTeamAsync(
            input.TeamId,
            null,
            cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> SetCustomerDefaultOrganizationAsync(
        SetCustomerDefaultOrganizationInput input,
        [Service] ICustomerOrganizationSettingsService customerOrganizationSettingsService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var customer =
            await customerOrganizationSettingsService.SetCustomerDefaultOrganizationAsync(
                input.OrganizationId,
                null,
                false,
                cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> ClearCustomerDefaultOrganizationAsync(
        ClearCustomerDefaultOrganizationInput input,
        [Service] ICustomerOrganizationSettingsService customerOrganizationSettingsService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var customer = await customerOrganizationSettingsService.ClearCustomerDefaultOrganizationAsync(
            null,
            cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> AddCustomerDefaultLocationTagAsync(
        AddCustomerDefaultLocationTagInput input,
        [Service] ICustomerLocationTagSettingsService customerLocationTagSettingsService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var customer = await customerLocationTagSettingsService.AddCustomerDefaultLocationTagAsync(
            input.LocationTagId,
            null,
            cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> RemoveCustomerDefaultLocationTagAsync(
        RemoveCustomerDefaultLocationTagInput input,
        [Service] ICustomerLocationTagSettingsService customerLocationTagSettingsService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var customer =
            await customerLocationTagSettingsService.RemoveCustomerDefaultLocationTagAsync(
                input.LocationTagId,
                null,
                cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> AddCustomerDefaultDeskAsync(
        AddCustomerDefaultDeskInput input,
        [Service] ICustomerDeskSettingsService customerDeskSettingsService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var customer = await customerDeskSettingsService.AddCustomerDefaultDeskAsync(
            input.DeskId,
            null,
            cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> RemoveCustomerDefaultDeskAsync(
        RemoveCustomerDefaultDeskInput input,
        [Service] ICustomerDeskSettingsService customerDeskSettingsService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var customer = await customerDeskSettingsService.RemoveCustomerDefaultDeskAsync(
            input.DeskId,
            null,
            cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<SubmitCustomerFeedbackPayload?> SubmitCustomerFeedbackAsync(
        SubmitCustomerFeedbackInput input,
        [Service] ICustomerFeedbackService customerFeedbackService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var customerFeedback = await customerFeedbackService.SubmitFeedbackAsync(
            mapper.MapTo(input),
            cancellationToken);
        return mapper.MapTo(customerFeedback, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> UpdateMyCustomerDetailsAsync(
        UpdateMyCustomerDetailsInput input,
        [Service] ICustomerDetailsService customerDetailsService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var customerFeedback = await customerDetailsService.UpdateMyCustomerDetailsAsync(
            input.Timezone,
            input.Designation,
            input.Title,
            input.Name,
            input.GivenName,
            input.MiddleName,
            input.FamilyName,
            cancellationToken);
        return mapper.MapTo(customerFeedback, input.ClientMutationId);
    }
}
