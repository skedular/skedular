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
    public async Task<CustomerPayload?> CompletePreferredLocationOnboardingAsync(
        CompletePreferredLocationOnboardingInput input,
        [Service] ICustomerSettingsService customerSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerSettingsService.CompletePreferredLocationOnboardingAsync(cancellationToken);
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
    public async Task<CustomerPayload?> AddCustomerPreferredLocationAsync(
        AddCustomerPreferredLocationInput input,
        [Service] ICustomerLocationSettingsService customerLocationSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerLocationSettingsService.AddCustomerPreferredLocationAsync(
            input.LocationId,
            null,
            false,
            cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> RemoveCustomerPreferredLocationAsync(
        RemoveCustomerPreferredLocationInput input,
        [Service] ICustomerLocationSettingsService customerLocationSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerLocationSettingsService.RemoveCustomerPreferredLocationAsync(
            input.LocationId,
            null,
            cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> AddCustomerPreferredTeamAsync(
        AddCustomerPreferredTeamInput input,
        [Service] ICustomerTeamSettingsService customerTeamSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerTeamSettingsService.AddCustomerPreferredTeamAsync(
            input.TeamId,
            null,
            false,
            cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> RemoveCustomerPreferredTeamAsync(
        RemoveCustomerPreferredTeamInput input,
        [Service] ICustomerTeamSettingsService customerTeamSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerTeamSettingsService.RemoveCustomerPreferredTeamAsync(
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
        var customer = await customerOrganizationSettingsService.SetCustomerDefaultOrganizationAsync(
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
        var customer = await customerOrganizationSettingsService.ClearCustomerDefaultOrganizationAsync(null, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }


    [UseResolverScope]
    public async Task<SubmitCustomerFeedbackPayload?> SubmitCustomerFeedbackAsync(
        SubmitCustomerFeedbackInput input,
        [Service] ICustomerFeedbackService customerFeedbackService,
        CancellationToken cancellationToken)
    {
        var customerFeedback = await customerFeedbackService.SubmitFeedbackAsync(mapper.MapTo(input), cancellationToken);
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
    public async Task<CustomerPayload?> UpdateCustomerDetailsAsync(
        UpdateCustomerDetailsInput input,
        [Service] ICustomerDetailsService customerDetailsService,
        CancellationToken cancellationToken)
    {
        var customerFeedback = await customerDetailsService.UpdateCustomerDetailsAsync(
            input.Id,
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
    public async Task<CustomerPayload?> AddCustomerPreferredOrganizationTagAsync(
        AddCustomerPreferredOrganizationTagInput input,
        [Service] ICustomerOrganizationTagSettingsService customerOrganizationTagSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerOrganizationTagSettingsService.AddCustomerPreferredOrganizationTagAsync(
            input.OrganizationTagId,
            null,
            cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> RemoveCustomerPreferredOrganizationTagAsync(
        RemoveCustomerPreferredOrganizationTagInput input,
        [Service] ICustomerOrganizationTagSettingsService customerOrganizationTagSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerOrganizationTagSettingsService.RemoveCustomerPreferredOrganizationTagAsync(
            input.OrganizationTagId,
            null,
            cancellationToken);
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
    public async Task<CustomerPayload?> AddCustomerPreferredDeskAsync(
        AddCustomerPreferredDeskInput input,
        [Service] ICustomerDeskSettingsService customerDeskSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerDeskSettingsService.AddCustomerPreferredDeskAsync(input.DeskId, null, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> RemoveCustomerPreferredDeskAsync(
        RemoveCustomerPreferredDeskInput input,
        [Service] ICustomerDeskSettingsService customerDeskSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerDeskSettingsService.RemoveCustomerPreferredDeskAsync(input.DeskId, null, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> CompletePreferredRoomOnboardingAsync(
        CompletePreferredRoomOnboardingInput input,
        [Service] ICustomerSettingsService customerSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerSettingsService.CompletePreferredRoomOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> AddCustomerPreferredRoomAsync(
        AddCustomerPreferredRoomInput input,
        [Service] ICustomerRoomSettingsService customerRoomSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerRoomSettingsService.AddCustomerPreferredRoomAsync(input.RoomId, null, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    [UseResolverScope]
    public async Task<CustomerPayload?> RemoveCustomerPreferredRoomAsync(
        RemoveCustomerPreferredRoomInput input,
        [Service] ICustomerRoomSettingsService customerRoomSettingsService,
        CancellationToken cancellationToken)
    {
        var customer = await customerRoomSettingsService.RemoveCustomerPreferredRoomAsync(input.RoomId, null, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }
}
