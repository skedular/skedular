using Customer.Api.Mappers;
using Customer.Api.Services.Authorization;
using Customer.Shared.Repositories;
using Enterprise.Shared.Exceptions;

namespace Customer.Api.Services;

public interface ICustomerRoomSettingsService
{
    Task<Shared.Models.Customer> AddCustomerDefaultRoomAsync(string roomId, string? customerId, CancellationToken cancellationToken);
    Task<Shared.Models.Customer> RemoveCustomerDefaultRoomAsync(string roomId, string? customerId, CancellationToken cancellationToken);
}

public class CustomerRoomSettingsService(
    ICustomerHelperService customerHelperService,
    ILocationAuthorizationService locationAuthorizationService,
    IRepositoryFactory repositoryFactory,
    IMapper mapper)
    : ICustomerRoomSettingsService
{
    public async Task<Shared.Models.Customer> AddCustomerDefaultRoomAsync(string roomId, string? customerId, CancellationToken cancellationToken)
    {
        var customer = string.IsNullOrWhiteSpace(customerId)
            ? await customerHelperService.GetCustomerAsync(cancellationToken)
            : await customerHelperService.GetCustomerAsync(customerId, cancellationToken);
        var room = await repositoryFactory.RoomRepository.GetByIdAsync(roomId, false, cancellationToken);
        if (room is null)
        {
            throw new RoomNotFound();
        }

        var location = await repositoryFactory.LocationRepository.GetByIdAsync(room.Location.Id, false, cancellationToken);
        if (location is null)
        {
            throw new LocationNotFound();
        }

        if (!await locationAuthorizationService.CanAddLocationAsDefaultAsync(location, customer, cancellationToken))
        {
            throw new Unauthorized();
        }

        if (customer.PreferredRooms.Any(item => item.Id == roomId))
        {
            return mapper.MapTo(customer);
        }

        customer.PreferredRooms = customer.PreferredRooms.Concat([room]).ToList();
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }

    public async Task<Shared.Models.Customer> RemoveCustomerDefaultRoomAsync(string roomId, string? customerId, CancellationToken cancellationToken)
    {
        var customer = string.IsNullOrWhiteSpace(customerId)
            ? await customerHelperService.GetCustomerAsync(cancellationToken)
            : await customerHelperService.GetCustomerAsync(customerId, cancellationToken);
        customer.PreferredRooms = customer.PreferredRooms.Where(item => item.Id != roomId).ToList();
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }
}
