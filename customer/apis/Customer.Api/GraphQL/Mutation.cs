using Customer.Api.Mappers;
using Customer.Api.Services;
using HotChocolate;
using HotChocolate.Types;

namespace Customer.Api.GraphQL;

[MutationType]
public class Mutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<CustomerPayload?> CompleteOrganizationOnboardingAsync(
        CompleteOrganizationOnboardingInput input,
        [Service] ICustomerSettingsService customerSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerSettingsService.CompleteOrganizationOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> CompleteLocationOnboardingAsync(
        CompleteLocationOnboardingInput input,
        [Service] ICustomerSettingsService customerSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerSettingsService.CompleteLocationOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> CompleteTeamOnboardingAsync(
        CompleteTeamOnboardingInput input,
        [Service] ICustomerSettingsService customerSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerSettingsService.CompleteTeamOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> CompleteDefaultOrganizationOnboardingAsync(
        CompleteDefaultOrganizationOnboardingInput input,
        [Service] ICustomerSettingsService customerSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerSettingsService.CompleteDefaultOrganizationOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> CompleteDefaultLocationOnboardingAsync(
        CompleteDefaultLocationOnboardingInput input,
        [Service] ICustomerSettingsService customerSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerSettingsService.CompleteDefaultLocationOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> CompletePreferredZoneOnboardingAsync(
        CompletePreferredZoneOnboardingInput input,
        [Service] ICustomerSettingsService customerSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerSettingsService.CompletePreferredZoneOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> CompletePreferredDeskOnboardingAsync(
        CompletePreferredDeskOnboardingInput input,
        [Service] ICustomerSettingsService customerSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerSettingsService.CompletePreferredDeskOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> AddCustomerDefaultLocationAsync(
        AddCustomerDefaultLocationInput input,
        [Service] ICustomerLocationSettingsService customerLocationSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerLocationSettingsService.AddCustomerDefaultLocationAsync(
            input.LocationId,
            null,
            false,
            cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> RemoveCustomerDefaultLocationAsync(
        RemoveCustomerDefaultLocationInput input,
        [Service] ICustomerLocationSettingsService customerLocationSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerLocationSettingsService.RemoveCustomerDefaultLocationAsync(
            input.LocationId,
            null,
            cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> AddCustomerDefaultTeamAsync(
        AddCustomerDefaultTeamInput input,
        [Service] ICustomerTeamSettingsService customerTeamSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerTeamSettingsService.AddCustomerDefaultTeamAsync(
            input.TeamId,
            null,
            false,
            cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> RemoveCustomerDefaultTeamAsync(
        RemoveCustomerDefaultTeamInput input,
        [Service] ICustomerTeamSettingsService customerTeamSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerTeamSettingsService.RemoveCustomerDefaultTeamAsync(
            input.TeamId,
            null,
            cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> SetCustomerDefaultOrganizationAsync(
        SetCustomerDefaultOrganizationInput input,
        [Service] ICustomerOrganizationSettingsService customerOrganizationSettingsService,
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

    [UseResolverScope]
    public async Task<CustomerPayload?> ClearCustomerDefaultOrganizationAsync(
        ClearCustomerDefaultOrganizationInput input,
        [Service] ICustomerOrganizationSettingsService customerOrganizationSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerOrganizationSettingsService.ClearCustomerDefaultOrganizationAsync(
            null,
            cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> AddCustomerDefaultDeskAsync(
        AddCustomerDefaultDeskInput input,
        [Service] ICustomerDeskSettingsService customerDeskSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerDeskSettingsService.AddCustomerDefaultDeskAsync(
            input.DeskId,
            null,
            cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> RemoveCustomerDefaultDeskAsync(
        RemoveCustomerDefaultDeskInput input,
        [Service] ICustomerDeskSettingsService customerDeskSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerDeskSettingsService.RemoveCustomerDefaultDeskAsync(
            input.DeskId,
            null,
            cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<SubmitCustomerFeedbackPayload?> SubmitCustomerFeedbackAsync(
        SubmitCustomerFeedbackInput input,
        [Service] ICustomerFeedbackService customerFeedbackService,
        CancellationToken cancellationToken)
    {
        var customerFeedback = await customerFeedbackService.SubmitFeedbackAsync(
            mapper.MapTo(input),
            cancellationToken);
        return mapper.MapTo(customerFeedback, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> UpdateMyCustomerDetailsAsync(
        UpdateMyCustomerDetailsInput input,
        [Service] ICustomerDetailsService customerDetailsService,
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
            input.PhoneNumber,
            cancellationToken);
        return mapper.MapTo(customerFeedback, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> AddCustomerDefaultOrganizationTagAsync(
        AddCustomerDefaultOrganizationTagInput input,
        [Service] ICustomerOrganizationTagSettingsService customerOrganizationTagSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerOrganizationTagSettingsService.AddCustomerDefaultOrganizationTagAsync(
            input.OrganizationTagId,
            null,
            cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> RemoveCustomerDefaultOrganizationTagAsync(
        RemoveCustomerDefaultOrganizationTagInput input,
        [Service] ICustomerOrganizationTagSettingsService customerOrganizationTagSettingsService,
        CancellationToken cancellationToken)
    {
        var customer =
            await customerOrganizationTagSettingsService.RemoveCustomerDefaultOrganizationTagAsync(
                input.OrganizationTagId,
                null,
                cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }
}
