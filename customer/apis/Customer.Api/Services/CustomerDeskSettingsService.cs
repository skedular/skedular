using Customer.Api.Mappers;
using Customer.Api.Services.Authorization;
using Customer.Shared.Repositories;
using Enterprise.Shared.Exceptions;

namespace Customer.Api.Services;

public interface ICustomerDeskSettingsService
{
    Task<Shared.Models.Customer> AddCustomerDefaultDeskAsync(
        string deskId,
        string? customerId,
        CancellationToken cancellationToken);

    Task<Shared.Models.Customer> RemoveCustomerDefaultDeskAsync(
        string deskId,
        string? customerId,
        CancellationToken cancellationToken);
}

public class CustomerDeskSettingsService(
    ICustomerHelperService customerHelperService,
    ILocationAuthorizationService locationAuthorizationService,
    IRepositoryFactory repositoryFactory,
    IMapper mapper)
    : ICustomerDeskSettingsService
{
    public async Task<Shared.Models.Customer> AddCustomerDefaultDeskAsync(
        string deskId,
        string? customerId,
        CancellationToken cancellationToken)
    {
        var customer = string.IsNullOrWhiteSpace(customerId)
            ? await customerHelperService.GetCustomerAsync(cancellationToken)
            : await customerHelperService.GetCustomerAsync(customerId, cancellationToken);
        var desk = await repositoryFactory.DeskRepository.GetByIdAsync(deskId, cancellationToken);
        if (desk is null)
        {
            throw new DeskNotFound();
        }

        var location =
            await repositoryFactory.LocationRepository.GetByIdAsync(desk.Location.Id, false, cancellationToken);
        if (location is null)
        {
            throw new LocationNotFound();
        }

        if (!await locationAuthorizationService.CanAddLocationAsDefaultAsync(location, customer, cancellationToken))
        {
            throw new Unauthorized();
        }

        if (customer.PreferredDesks.Any(item => item.Id == deskId))
        {
            return mapper.MapTo(customer);
        }

        customer.PreferredDesks = customer.PreferredDesks.Concat([desk]).ToList();
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }

    public async Task<Shared.Models.Customer> RemoveCustomerDefaultDeskAsync(
        string deskId,
        string? customerId,
        CancellationToken cancellationToken)
    {
        var customer = string.IsNullOrWhiteSpace(customerId)
            ? await customerHelperService.GetCustomerAsync(cancellationToken)
            : await customerHelperService.GetCustomerAsync(customerId, cancellationToken);
        customer.PreferredDesks = customer.PreferredDesks.Where(item => item.Id != deskId).ToList();
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }
}
