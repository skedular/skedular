using Customer.Api.Mappers;
using Customer.Api.Services;
using Enterprise.Shared.Context;

namespace Customer.Api.GraphQL;

public class CustomerMutation(IServiceProvider serviceProvider, IMapper mapper)
{
    public async Task<CustomerPayload?> CompleteOrganizationOnboardingAsync(
        CompleteOrganizationOnboardingInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerSettingsService>();
        var customer = await service.CompleteOrganizationOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> CompleteLocationOnboardingAsync(
        CompleteLocationOnboardingInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerSettingsService>();
        var customer = await service.CompleteLocationOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> CompleteTeamOnboardingAsync(
        CompleteTeamOnboardingInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerSettingsService>();
        var customer = await service.CompleteTeamOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> CompleteDefaultOrganizationOnboardingAsync(
        CompleteDefaultOrganizationOnboardingInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerSettingsService>();
        var customer = await service.CompleteDefaultOrganizationOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> CompleteDefaultLocationOnboardingAsync(
        CompleteDefaultLocationOnboardingInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerSettingsService>();
        var customer = await service.CompleteDefaultLocationOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> CompletePreferredZoneOnboardingAsync(
        CompletePreferredZoneOnboardingInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerSettingsService>();
        var customer = await service.CompletePreferredZoneOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> CompletePreferredDeskOnboardingAsync(
        CompletePreferredDeskOnboardingInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerSettingsService>();
        var customer = await service.CompletePreferredDeskOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> AddCustomerDefaultLocationAsync(
        AddCustomerDefaultLocationInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerLocationSettingsService>();
        var customer = await service.AddCustomerDefaultLocationAsync(input.LocationId, null, false, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> RemoveCustomerDefaultLocationAsync(
        RemoveCustomerDefaultLocationInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerLocationSettingsService>();
        var customer = await service.RemoveCustomerDefaultLocationAsync(input.LocationId, null, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> AddCustomerDefaultTeamAsync(
        AddCustomerDefaultTeamInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerTeamSettingsService>();
        var customer = await service.AddCustomerDefaultTeamAsync(input.TeamId, null, false, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> RemoveCustomerDefaultTeamAsync(
        RemoveCustomerDefaultTeamInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerTeamSettingsService>();
        var customer = await service.RemoveCustomerDefaultTeamAsync(input.TeamId, null, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> SetCustomerDefaultOrganizationAsync(
        SetCustomerDefaultOrganizationInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerOrganizationSettingsService>();
        var customer =
            await service.SetCustomerDefaultOrganizationAsync(input.OrganizationId, null, false, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> ClearCustomerDefaultOrganizationAsync(
        ClearCustomerDefaultOrganizationInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerOrganizationSettingsService>();
        var customer = await service.ClearCustomerDefaultOrganizationAsync(null, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> AddCustomerDefaultLocationTagAsync(
        AddCustomerDefaultLocationTagInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerLocationTagSettingsService>();
        var customer = await service.AddCustomerDefaultLocationTagAsync(input.LocationTagId, null, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> RemoveCustomerDefaultLocationTagAsync(
        RemoveCustomerDefaultLocationTagInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerLocationTagSettingsService>();
        var customer =
            await service.RemoveCustomerDefaultLocationTagAsync(input.LocationTagId, null, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> AddCustomerDefaultDeskAsync(
        AddCustomerDefaultDeskInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerDeskSettingsService>();
        var customer = await service.AddCustomerDefaultDeskAsync(input.DeskId, null, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> RemoveCustomerDefaultDeskAsync(
        RemoveCustomerDefaultDeskInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerDeskSettingsService>();
        var customer = await service.RemoveCustomerDefaultDeskAsync(input.DeskId, null, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public async Task<SubmitCustomerFeedbackPayload?> SubmitCustomerFeedbackAsync(
        SubmitCustomerFeedbackInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerFeedbackService>();
        var customerFeedback = await service.SubmitFeedbackAsync(mapper.MapTo(input), cancellationToken);
        return mapper.MapTo(customerFeedback, input.ClientMutationId);
    }

    public async Task<CustomerPayload?> UpdateMyCustomerDetailsAsync(
        UpdateMyCustomerDetailsInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerDetailsService>();
        var customerFeedback = await service.UpdateMyCustomerDetailsAsync(
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
