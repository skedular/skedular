using Api.Shared.Services.GraphQL.UnityHub.V1.Customer;
using Customer.Api.Mappers;
using Customer.Api.Services;
using Enterprise.Shared.Context;

namespace Customer.Api.GraphQL;

public class CustomerMutation(IMapper mapper) : Mutation
{
    public override async Task<CustomerPayload?> CompleteOrganizationOnboardingAsync(
        CompleteOrganizationOnboardingInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerSettingsService>();
        var customer = await service.CompleteOrganizationOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public override async Task<CustomerPayload?> CompleteLocationOnboardingAsync(
        CompleteLocationOnboardingInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerSettingsService>();
        var customer = await service.CompleteLocationOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public override async Task<CustomerPayload?> CompleteDefaultOrganizationOnboardingAsync(
        CompleteDefaultOrganizationOnboardingInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerSettingsService>();
        var customer = await service.CompleteDefaultOrganizationOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public override async Task<CustomerPayload?> CompleteDefaultLocationOnboardingAsync(
        CompleteDefaultLocationOnboardingInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerSettingsService>();
        var customer = await service.CompleteDefaultLocationOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public override async Task<CustomerPayload?> CompletePreferredZoneOnboardingAsync(
        CompletePreferredZoneOnboardingInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerSettingsService>();
        var customer = await service.CompletePreferredZoneOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public override async Task<CustomerPayload?> CompletePreferredDeskOnboardingAsync(
        CompletePreferredDeskOnboardingInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerSettingsService>();
        var customer = await service.CompletePreferredDeskOnboardingAsync(cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public override async Task<CustomerPayload?> AddCustomerDefaultLocationAsync(
        AddCustomerDefaultLocationInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerLocationSettingsService>();
        var customer = await service.AddCustomerDefaultLocationAsync(input.LocationId, null, false, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public override async Task<CustomerPayload?> RemoveCustomerDefaultLocationAsync(
        RemoveCustomerDefaultLocationInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerLocationSettingsService>();
        var customer = await service.RemoveCustomerDefaultLocationAsync(input.LocationId, null, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public override async Task<CustomerPayload?> AddCustomerDefaultTeamAsync(
        AddCustomerDefaultTeamInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerTeamSettingsService>();
        var customer = await service.AddCustomerDefaultTeamAsync(input.TeamId, null, false, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public override async Task<CustomerPayload?> RemoveCustomerDefaultTeamAsync(
        RemoveCustomerDefaultTeamInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerTeamSettingsService>();
        var customer = await service.RemoveCustomerDefaultTeamAsync(input.TeamId, null, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public override async Task<CustomerPayload?> SetCustomerDefaultOrganizationAsync(
        SetCustomerDefaultOrganizationInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerOrganizationSettingsService>();
        var customer =
            await service.SetCustomerDefaultOrganizationAsync(input.OrganizationId, null, false, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public override async Task<CustomerPayload?> ClearCustomerDefaultOrganizationAsync(
        ClearCustomerDefaultOrganizationInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerOrganizationSettingsService>();
        var customer = await service.ClearCustomerDefaultOrganizationAsync(null, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public override async Task<CustomerPayload?> AddCustomerDefaultLocationTagAsync(
        AddCustomerDefaultLocationTagInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerLocationTagSettingsService>();
        var customer = await service.AddCustomerDefaultLocationTagAsync(input.LocationTagId, null, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public override async Task<CustomerPayload?> RemoveCustomerDefaultLocationTagAsync(
        RemoveCustomerDefaultLocationTagInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerLocationTagSettingsService>();
        var customer =
            await service.RemoveCustomerDefaultLocationTagAsync(input.LocationTagId, null, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public override async Task<CustomerPayload?> AddCustomerDefaultDeskAsync(
        AddCustomerDefaultDeskInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerDeskSettingsService>();
        var customer = await service.AddCustomerDefaultDeskAsync(input.DeskId, null, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public override async Task<CustomerPayload?> RemoveCustomerDefaultDeskAsync(
        RemoveCustomerDefaultDeskInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerDeskSettingsService>();
        var customer = await service.RemoveCustomerDefaultDeskAsync(input.DeskId, null, cancellationToken);
        return mapper.MapTo(customer, input.ClientMutationId);
    }

    public override async Task<SubmitCustomerFeedbackPayload?> SubmitCustomerFeedbackAsync(
        SubmitCustomerFeedbackInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerFeedbackService>();
        var customerFeedback = await service.SubmitFeedbackAsync(mapper.MapTo(input), cancellationToken);
        return mapper.MapTo(customerFeedback, input.ClientMutationId);
    }

    public override async Task<CustomerPayload?> UpdateMyCustomerDetailsAsync(
        UpdateMyCustomerDetailsInput input,
        IServiceProvider serviceProvider,
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
