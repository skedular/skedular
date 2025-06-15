using Api.Shared.Services;
using Customer.Api.Mappers;
using Customer.Api.Services.Authorization;
using Customer.Shared.Repositories;

namespace Customer.Api.Services;

public interface ICustomerResourceSettingsService
{
    Task<Shared.Models.Customer> AddCustomerPreferredResourceAsync(string resourceId, string? customerId, CancellationToken cancellationToken);
    Task<Shared.Models.Customer> RemoveCustomerPreferredResourceAsync(string resourceId, string? customerId, CancellationToken cancellationToken);
}

public class CustomerResourceSettingsService(
    ICustomerHelperService customerHelperService,
    ILocationAuthorizationService locationAuthorizationService,
    IRepositoryFactory repositoryFactory,
    IMapper mapper)
    : ICustomerResourceSettingsService
{
    public async Task<Shared.Models.Customer> AddCustomerPreferredResourceAsync(
        string resourceId,
        string? customerId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceId);

        var customer = string.IsNullOrWhiteSpace(customerId)
            ? await customerHelperService.GetCustomerAsync(cancellationToken)
            : await customerHelperService.GetCustomerAsync(customerId, cancellationToken);
        var resource = await repositoryFactory.ResourceRepository.GetByIdAsync(resourceId, false, cancellationToken) ?? throw new ResourceNotFound();
        if (resource.Location is not null)
        {
            var location = await repositoryFactory.LocationRepository.GetByIdAsync(resource.Location.Id, false, cancellationToken) ??
                           throw new LocationNotFound();
            if (!await locationAuthorizationService.CanAddLocationAsPreferredAsync(location, customer, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        if (customer.PreferredResources.Any(item => item.Id == resourceId))
        {
            return mapper.MapTo(customer);
        }

        customer.PreferredResources = customer.PreferredResources.Concat([resource]).ToList();
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }

    public async Task<Shared.Models.Customer> RemoveCustomerPreferredResourceAsync(
        string resourceId,
        string? customerId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceId);

        var customer = string.IsNullOrWhiteSpace(customerId)
            ? await customerHelperService.GetCustomerAsync(cancellationToken)
            : await customerHelperService.GetCustomerAsync(customerId, cancellationToken);
        customer.PreferredResources = customer.PreferredResources.Where(item => item.Id != resourceId).ToList();
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }
}
