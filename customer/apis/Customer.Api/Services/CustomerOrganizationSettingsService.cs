using Api.Shared.Services;
using Customer.Api.Services.Authorization;
using Customer.Shared.Database.Entities;
using Customer.Shared.Repositories;

namespace Customer.Api.Services;

public interface ICustomerOrganizationSettingsService
{
    Task<Shared.Models.Customer> SetCustomerDefaultOrganizationAsync(
        string? organizationId,
        string? organizationCustomDomain,
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
        string? organizationId,
        string? organizationCustomDomain,
        string? customerId,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        var customer = string.IsNullOrWhiteSpace(customerId)
            ? await customerHelperService.GetCustomerAsync(cancellationToken)
            : await customerHelperService.GetCustomerAsync(customerId, cancellationToken);

        Organization organization;
        if (!string.IsNullOrWhiteSpace(organizationId))
        {
            organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organizationId, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(organizationCustomDomain))
        {
            organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                organizationId,
                organizationCustomDomain,
                false,
                false,
                cancellationToken) ?? throw new OrganizationNotFound();
        }
        else
        {
            throw new InvalidOperationException("Either id or customDomain must be provided.");
        }

        if (!ignoreAuthorizationCheck &&
            !await organizationAuthorizationService.CanAddOrganizationAsDefaultAsync(organization.Id, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
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
