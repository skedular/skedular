using Api.Shared.Services;
using Api.Shared.Services.Models;
using Customer.Api.Mappers;
using Customer.Api.Services.Authorization;
using Customer.Shared.Repositories;

namespace Customer.Api.Services;

public interface ICustomerLocationSettingsService
{
    Task<Shared.Models.Customer> AddCustomerPreferredLocationAsync(
        string locationId,
        string? customerId,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);

    Task<Shared.Models.Customer> RemoveCustomerPreferredLocationAsync(string locationId, string? customerId, CancellationToken cancellationToken);
    Task<Shared.Models.Customer> AddCustomerFavouriteLocationAsync(string locationId, CancellationToken cancellationToken);
    Task<Shared.Models.Customer> RemoveCustomerFavouriteLocationAsync(string locationId, CancellationToken cancellationToken);
}

public class CustomerLocationSettingsService(
    ICustomerHelperService customerHelperService,
    ILocationAuthorizationService locationAuthorizationService,
    IRepositoryFactory repositoryFactory,
    IMapper mapper)
    : ICustomerLocationSettingsService
{
    public async Task<Shared.Models.Customer> AddCustomerPreferredLocationAsync(
        string locationId,
        string? customerId,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        var customer = string.IsNullOrWhiteSpace(customerId)
            ? await customerHelperService.GetCustomerAsync(cancellationToken)
            : await customerHelperService.GetCustomerAsync(customerId, cancellationToken);
        var location = await repositoryFactory.LocationRepository.UpsertNakedAsync(locationId, null, cancellationToken) ??
                       throw new LocationNotFound();
        if (!ignoreAuthorizationCheck && !await locationAuthorizationService.CanAddLocationAsPreferredAsync(location, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        if (customer.PreferredLocations.Any(item => item.Id == locationId))
        {
            return mapper.MapTo(customer);
        }

        customer.PreferredLocations = customer.PreferredLocations.Append(location).ToList();
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }

    public async Task<Shared.Models.Customer> RemoveCustomerPreferredLocationAsync(
        string locationId,
        string? customerId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        var customer = string.IsNullOrWhiteSpace(customerId)
            ? await customerHelperService.GetCustomerAsync(cancellationToken)
            : await customerHelperService.GetCustomerAsync(customerId, cancellationToken);
        customer.PreferredLocations = customer.PreferredLocations.Where(item => item.Id != locationId).ToList();
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }

    public async Task<Shared.Models.Customer> AddCustomerFavouriteLocationAsync(string locationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        var customer = await customerHelperService.GetCustomerAsync(cancellationToken);
        var location = await repositoryFactory.LocationRepository.UpsertNakedAsync(locationId, null, cancellationToken) ??
                       throw new LocationNotFound();

        if (location.Type is null || location.Type != LocationTypeConstants.Marketplace ||
            customer.FavouriteLocations.Any(item => item.Id == locationId))
        {
            return mapper.MapTo(customer);
        }

        customer.FavouriteLocations = customer.FavouriteLocations.Append(location).ToList();
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }

    public async Task<Shared.Models.Customer> RemoveCustomerFavouriteLocationAsync(string locationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        var customer = await customerHelperService.GetCustomerAsync(cancellationToken);
        customer.FavouriteLocations = customer.FavouriteLocations.Where(item => item.Id != locationId).ToList();
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }
}
