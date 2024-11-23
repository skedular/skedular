using Customer.Api.Mappers;
using Customer.Api.Services.Authorization;
using Customer.Shared.Repositories;
using Enterprise.Shared.Exceptions;

namespace Customer.Api.Services;

public interface ICustomerLocationTagSettingsService
{
    Task<Shared.Models.Customer> AddCustomerDefaultLocationTagAsync(
        string locationTagId,
        string? customerId,
        CancellationToken cancellationToken);

    Task<Shared.Models.Customer> RemoveCustomerDefaultLocationTagAsync(
        string locationTagId,
        string? customerId,
        CancellationToken cancellationToken);
}

public class CustomerLocationTagSettingsService(
    ICustomerHelperService customerHelperService,
    ILocationAuthorizationService locationAuthorizationService,
    IRepositoryFactory repositoryFactory,
    IMapper mapper)
    : ICustomerLocationTagSettingsService
{
    public async Task<Shared.Models.Customer> AddCustomerDefaultLocationTagAsync(
        string locationTagId,
        string? customerId,
        CancellationToken cancellationToken)
    {
        var customer = string.IsNullOrWhiteSpace(customerId)
            ? await customerHelperService.GetCustomerAsync(cancellationToken)
            : await customerHelperService.GetCustomerAsync(customerId, cancellationToken);
        var locationTag = await repositoryFactory.LocationTagRepository.GetByIdAsync(locationTagId, cancellationToken);
        if (locationTag is null)
        {
            throw new LocationTagNotFound();
        }

        var location =
            await repositoryFactory.LocationRepository.GetByIdAsync(locationTag.Location.Id, cancellationToken);
        if (location is null)
        {
            throw new LocationNotFound();
        }

        if (!await locationAuthorizationService.CanAddLocationTagAsDefaultAsync(location, customer, cancellationToken))
        {
            throw new Unauthorized();
        }

        if (customer.PreferredLocationTags.Any(item => item.Id == locationTagId))
        {
            return mapper.MapTo(customer);
        }

        customer.PreferredLocationTags = customer.PreferredLocationTags.Concat([locationTag]).ToList();
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }

    public async Task<Shared.Models.Customer> RemoveCustomerDefaultLocationTagAsync(
        string locationTagId,
        string? customerId,
        CancellationToken cancellationToken)
    {
        var customer = string.IsNullOrWhiteSpace(customerId)
            ? await customerHelperService.GetCustomerAsync(cancellationToken)
            : await customerHelperService.GetCustomerAsync(customerId, cancellationToken);
        customer.PreferredLocationTags =
            customer.PreferredLocationTags.Where(item => item.Id != locationTagId).ToList();
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }
}
