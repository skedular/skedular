using Customer.Api.Services.Authorization;
using Customer.Shared.Repositories;
using Enterprise.Shared.Exceptions;

namespace Customer.Api.Services;

public interface ICustomerOrganizationSettingsService
{
    Task<Shared.Models.Customer> SetCustomerDefaultOrganizationAsync(
        string organizationId,
        string? customerId,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);

    Task<Shared.Models.Customer> ClearCustomerDefaultOrganizationAsync(string? customerId, CancellationToken cancellationToken);
}

public class CustomerOrganizationSettingsService(
    ICustomerHelperService customerHelperService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IRepositoryFactory repositoryFactory)
    : ICustomerOrganizationSettingsService
{
    public async Task<Shared.Models.Customer> SetCustomerDefaultOrganizationAsync(
        string organizationId,
        string? customerId,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);

        var customer = string.IsNullOrWhiteSpace(customerId)
            ? await customerHelperService.GetCustomerAsync(cancellationToken)
            : await customerHelperService.GetCustomerAsync(customerId, cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organizationId, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!ignoreAuthorizationCheck && !organizationAuthorizationService.CanAddOrganizationAsDefault(organization, customer))
        {
            throw new Unauthorized();
        }

        customer.DefaultOrganization = organization;
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }

    public async Task<Shared.Models.Customer> ClearCustomerDefaultOrganizationAsync(string? customerId, CancellationToken cancellationToken)
    {
        var customer = string.IsNullOrWhiteSpace(customerId)
            ? await customerHelperService.GetCustomerAsync(cancellationToken)
            : await customerHelperService.GetCustomerAsync(customerId, cancellationToken);
        customer.DefaultOrganization = null;
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }
}
