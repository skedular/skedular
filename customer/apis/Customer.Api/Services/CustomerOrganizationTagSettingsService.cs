using Api.Shared.Services;
using Customer.Api.Mappers;
using Customer.Api.Services.Authorization;
using Customer.Shared.Repositories;

namespace Customer.Api.Services;

public interface ICustomerOrganizationTagSettingsService
{
    Task<Shared.Models.Customer> AddCustomerPreferredOrganizationTagAsync(
        string organizationTagId,
        string? customerId,
        CancellationToken cancellationToken);

    Task<Shared.Models.Customer> RemoveCustomerPreferredOrganizationTagAsync(
        string organizationTagId,
        string? customerId,
        CancellationToken cancellationToken);
}

public class CustomerOrganizationTagSettingsService(
    ICustomerHelperService customerHelperService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IRepositoryFactory repositoryFactory,
    IMapper mapper)
    : ICustomerOrganizationTagSettingsService
{
    public async Task<Shared.Models.Customer> AddCustomerPreferredOrganizationTagAsync(
        string organizationTagId,
        string? customerId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationTagId);

        var customer = string.IsNullOrWhiteSpace(customerId)
            ? await customerHelperService.GetCustomerAsync(cancellationToken)
            : await customerHelperService.GetCustomerAsync(customerId, cancellationToken);
        var organizationTag = await repositoryFactory.OrganizationTagRepository.GetByIdAsync(organizationTagId, cancellationToken) ??
                              throw new OrganizationTagNotFound();
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                               organizationTag.Organization.Id,
                               null,
                               false,
                               false,
                               cancellationToken) ??
                           throw new OrganizationNotFound();

        if (!await organizationAuthorizationService.CanAddOrganizationTagAsDefaultAsync(organization.Id, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        if (customer.PreferredOrganizationTags.Any(item => item.Id == organizationTagId))
        {
            return mapper.MapTo(customer);
        }

        customer.PreferredOrganizationTags = customer.PreferredOrganizationTags.Concat([organizationTag]).ToList();
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }

    public async Task<Shared.Models.Customer> RemoveCustomerPreferredOrganizationTagAsync(
        string organizationTagId,
        string? customerId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationTagId);

        var customer = string.IsNullOrWhiteSpace(customerId)
            ? await customerHelperService.GetCustomerAsync(cancellationToken)
            : await customerHelperService.GetCustomerAsync(customerId, cancellationToken);
        customer.PreferredOrganizationTags = customer.PreferredOrganizationTags.Where(item => item.Id != organizationTagId).ToList();
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }
}
