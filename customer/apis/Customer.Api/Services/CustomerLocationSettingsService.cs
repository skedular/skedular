using Customer.Api.Mappers;
using Customer.Api.Services.Authorization;
using Customer.Shared.Repositories;
using Enterprise.Shared.Exceptions;

namespace Customer.Api.Services;

public interface ICustomerLocationSettingsService
{
    Task<Shared.Models.Customer> AddCustomerDefaultLocationAsync(
        string locationId,
        string? customerId,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);

    Task<Shared.Models.Customer> RemoveCustomerDefaultLocationAsync(
        string locationId,
        string? customerId,
        CancellationToken cancellationToken);
}

public class CustomerLocationSettingsService(
    ICustomerHelperService customerHelperService,
    ILocationAuthorizationService locationAuthorizationService,
    IRepositoryFactory repositoryFactory,
    IMapper mapper)
    : ICustomerLocationSettingsService
{
    public async Task<Shared.Models.Customer> AddCustomerDefaultLocationAsync(
        string locationId,
        string? customerId,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        var customer = string.IsNullOrWhiteSpace(customerId)
            ? await customerHelperService.GetCustomerAsync(cancellationToken)
            : await customerHelperService.GetCustomerAsync(customerId, cancellationToken);
        var location = await repositoryFactory.LocationRepository.UpsertNakedAsync(locationId, null, cancellationToken);
        if (location is null)
        {
            throw new LocationNotFound();
        }

        if (!ignoreAuthorizationCheck &&
            !await locationAuthorizationService.CanAddLocationAsDefaultAsync(location, customer, cancellationToken))
        {
            throw new Unauthorized();
        }

        if (customer.DefaultLocations.Any(item => item.Id == locationId))
        {
            return mapper.MapTo(customer);
        }

        customer.DefaultLocations = customer.DefaultLocations.Concat([location]).ToList();
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }

    public async Task<Shared.Models.Customer> RemoveCustomerDefaultLocationAsync(
        string locationId,
        string? customerId,
        CancellationToken cancellationToken)
    {
        var customer = string.IsNullOrWhiteSpace(customerId)
            ? await customerHelperService.GetCustomerAsync(cancellationToken)
            : await customerHelperService.GetCustomerAsync(customerId, cancellationToken);
        customer.DefaultLocations = customer.DefaultLocations.Where(item => item.Id != locationId).ToList();
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }
}
