namespace Customer.Api.Services;

public interface ICustomerSettingsService
{
    Task<Shared.Models.Customer> CompleteOrganizationOnboardingAsync(CancellationToken cancellationToken);
    Task<Shared.Models.Customer> CompleteLocationOnboardingAsync(CancellationToken cancellationToken);
    Task<Shared.Models.Customer> CompleteTeamOnboardingAsync(CancellationToken cancellationToken);
    Task<Shared.Models.Customer> CompleteDefaultOrganizationOnboardingAsync(CancellationToken cancellationToken);
    Task<Shared.Models.Customer> CompletePreferredLocationOnboardingAsync(CancellationToken cancellationToken);
    Task<Shared.Models.Customer> CompletePreferredZoneOnboardingAsync(CancellationToken cancellationToken);
    Task<Shared.Models.Customer> CompletePreferredDeskOnboardingAsync(CancellationToken cancellationToken);
    Task<Shared.Models.Customer> CompletePreferredRoomOnboardingAsync(CancellationToken cancellationToken);
}

public class CustomerSettingsService(ICustomerHelperService customerHelperService) : ICustomerSettingsService
{
    public async Task<Shared.Models.Customer> CompleteOrganizationOnboardingAsync(CancellationToken cancellationToken)
    {
        var customer = await customerHelperService.GetCustomerAsync(cancellationToken);
        customer.IsOrganizationOnboardingDone = true;
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }

    public async Task<Shared.Models.Customer> CompleteLocationOnboardingAsync(CancellationToken cancellationToken)
    {
        var customer = await customerHelperService.GetCustomerAsync(cancellationToken);
        customer.IsLocationOnboardingDone = true;
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }

    public async Task<Shared.Models.Customer> CompleteTeamOnboardingAsync(CancellationToken cancellationToken)
    {
        var customer = await customerHelperService.GetCustomerAsync(cancellationToken);
        customer.IsTeamOnboardingDone = true;
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }

    public async Task<Shared.Models.Customer> CompleteDefaultOrganizationOnboardingAsync(CancellationToken cancellationToken)
    {
        var customer = await customerHelperService.GetCustomerAsync(cancellationToken);
        customer.IsDefaultOrganizationOnboardingDone = true;
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }

    public async Task<Shared.Models.Customer> CompletePreferredLocationOnboardingAsync(CancellationToken cancellationToken)
    {
        var customer = await customerHelperService.GetCustomerAsync(cancellationToken);
        customer.IsPreferredLocationOnboardingDone = true;
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }

    public async Task<Shared.Models.Customer> CompletePreferredZoneOnboardingAsync(CancellationToken cancellationToken)
    {
        var customer = await customerHelperService.GetCustomerAsync(cancellationToken);
        customer.IsPreferredZoneOnboardingDone = true;
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }

    public async Task<Shared.Models.Customer> CompletePreferredDeskOnboardingAsync(CancellationToken cancellationToken)
    {
        var customer = await customerHelperService.GetCustomerAsync(cancellationToken);
        customer.IsPreferredDeskOnboardingDone = true;
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }

    public async Task<Shared.Models.Customer> CompletePreferredRoomOnboardingAsync(CancellationToken cancellationToken)
    {
        var customer = await customerHelperService.GetCustomerAsync(cancellationToken);
        customer.IsPreferredRoomOnboardingDone = true;
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }
}
