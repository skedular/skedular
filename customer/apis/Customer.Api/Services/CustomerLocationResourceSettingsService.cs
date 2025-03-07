using Customer.Api.Mappers;
using Customer.Api.Services.Authorization;
using Customer.Shared.Repositories;
using Enterprise.Shared.Exceptions;

namespace Customer.Api.Services;

public interface ICustomerLocationResourceSettingsService
{
    Task<Shared.Models.Customer> AddCustomerPreferredLocationResourceAsync(
        string locationResourceId,
        string? customerId,
        CancellationToken cancellationToken);

    Task<Shared.Models.Customer> RemoveCustomerPreferredLocationResourceAsync(
        string locationResourceId,
        string? customerId,
        CancellationToken cancellationToken);
}

public class CustomerLocationResourceSettingsService(
    ICustomerHelperService customerHelperService,
    ILocationAuthorizationService locationAuthorizationService,
    IRepositoryFactory repositoryFactory,
    IMapper mapper)
    : ICustomerLocationResourceSettingsService
{
    public async Task<Shared.Models.Customer> AddCustomerPreferredLocationResourceAsync(
        string locationResourceId,
        string? customerId,
        CancellationToken cancellationToken)
    {
        var customer = string.IsNullOrWhiteSpace(customerId)
            ? await customerHelperService.GetCustomerAsync(cancellationToken)
            : await customerHelperService.GetCustomerAsync(customerId, cancellationToken);
        var locationResource = await repositoryFactory.LocationResourceRepository.GetByIdAsync(locationResourceId, false, cancellationToken);
        if (locationResource is null)
        {
            throw new LocationResourceNotFound();
        }

        var location = await repositoryFactory.LocationRepository.GetByIdAsync(locationResource.Location.Id, false, cancellationToken);
        if (location is null)
        {
            throw new LocationNotFound();
        }

        if (!await locationAuthorizationService.CanAddLocationAsPreferredAsync(location, customer, cancellationToken))
        {
            throw new Unauthorized();
        }

        if (customer.PreferredLocationResources.Any(item => item.Id == locationResourceId))
        {
            return mapper.MapTo(customer);
        }

        customer.PreferredLocationResources = customer.PreferredLocationResources.Concat([locationResource]).ToList();
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }

    public async Task<Shared.Models.Customer> RemoveCustomerPreferredLocationResourceAsync(
        string locationResourceId,
        string? customerId,
        CancellationToken cancellationToken)
    {
        var customer = string.IsNullOrWhiteSpace(customerId)
            ? await customerHelperService.GetCustomerAsync(cancellationToken)
            : await customerHelperService.GetCustomerAsync(customerId, cancellationToken);
        customer.PreferredLocationResources = customer.PreferredLocationResources.Where(item => item.Id != locationResourceId).ToList();
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }
}
