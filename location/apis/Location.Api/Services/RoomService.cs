using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using Location.Api.Mappers;
using Location.Api.Services.Authorization;
using Location.Shared.Models;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using OrganizationTag = Location.Shared.Database.Entities.OrganizationTag;

namespace Location.Api.Services;

public interface IRoomService
{
    Task<Room> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Room> AddAsync(Room room, bool ignoreAuthorizationCheck, CancellationToken cancellationToken);

    Task<ICollection<Room>> BulkAddAsync(
        string locationId,
        string? namePrefix,
        int count,
        ICollection<string> customTagIds,
        ICollection<string> zoneIds,
        bool deactivated,
        bool requireBookingApproval,
        string? color,
        CancellationToken cancellationToken);

    Task<Room> UpdateAsync(Room room, CancellationToken cancellationToken);
    Task<Room> DeleteAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<Room>> DeleteAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Task<ICollection<Room>> ActivateAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Task<ICollection<Room>> DeactivateAsync(ICollection<string> ids, CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<Room>>, int )> GetPaginatedRoomsAsync(
        PaginationInputParam paginationInputParam,
        RoomSearchCriteria searchCriteria,
        ICollection<RoomOrder> orderByFields,
        CancellationToken cancellationToken);
}

public class RoomService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ICachedCustomerService cachedCustomerService,
    ICustomerService customerService,
    ILocationAuthorizationService locationAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    IMapper mapper,
    ILocationOutboxPublisher locationOutboxPublisher) : IRoomService
{
    public async Task<Room> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var room = await repositoryFactory.RoomRepository.GetByIdAsync(id, false, cancellationToken);
        if (room is null)
        {
            throw new RoomNotFound();
        }

        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(room.Location.Id, cancellationToken);
        if (existingLocation is null)
        {
            throw new LocationNotFound();
        }

        if (existingLocation.Organization is not null &&
            !organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization, customer))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (!locationAuthorizationService.CanModify(existingLocation, customer))
        {
            throw new Unauthorized();
        }

        return mapper.MapTo(room);
    }

    public async Task<Room> AddAsync(Room room, bool ignoreAuthorizationCheck, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(room.Location.Id);

        Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(room.Id))
        {
            var existingRoom = await repositoryFactory.RoomRepository.GetByIdAsync(room.Id, false, cancellationToken);
            if (existingRoom is not null)
            {
                return await UpdateInternalAsync(room, existingRoom, customer, cancellationToken);
            }
        }
        else
        {
            room.Id = randomHelper.Generate();
        }

        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(room.Location.Id, cancellationToken);
        if (existingLocation is null)
        {
            throw new LocationNotFound();
        }

        if (customer is not null &&
            existingLocation.Organization is not null &&
            !organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization, customer))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (customer is not null && !locationAuthorizationService.CanModify(existingLocation, customer))
        {
            throw new Unauthorized();
        }

        var matchingRoomFound = await repositoryFactory.RoomRepository.Query(
                new Specification<Shared.Database.Entities.Room>
                {
                    Criteria = query =>
                        !query.DeletedAt.HasValue && query.Location.Id == room.Location.Id && EF.Functions.ILike(query.Name, room.Name)
                })
            .AnyAsync(cancellationToken);
        if (matchingRoomFound)
        {
            throw new RoomWithSameNameExist();
        }

        var organizationTags = existingLocation.Organization is null
            ? []
            : await repositoryFactory.OrganizationTagRepository.Query(
                new Specification<OrganizationTag>
                {
                    Criteria = query => !query.DeletedAt.HasValue &&
                                        room.CustomTags.Concat(room.Zones).Select(item => item.Id).Contains(query.Id) &&
                                        query.Organization.Id == existingLocation.Organization.Id &&
                                        !query.Organization.DeletedAt.HasValue
                }).ToListAsync(cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var mappedRoom = mapper.MapTo(repositoryFactory.RoomRepository.Add(mapper.MapTo(room, existingLocation, organizationTags)));
        await locationOutboxPublisher.PublishLocationAsync([mapper.MapTo(existingLocation)], repositoryFactory.UnitOfWork, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return mappedRoom;
    }

    public async Task<ICollection<Room>> BulkAddAsync(
        string locationId,
        string? namePrefix,
        int count,
        ICollection<string> customTagIds,
        ICollection<string> zoneIds,
        bool deactivated,
        bool requireBookingApproval,
        string? color,
        CancellationToken cancellationToken)
    {
        if (count <= 0)
        {
            return [];
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken);
        if (existingLocation is null)
        {
            throw new LocationNotFound();
        }

        if (existingLocation.Organization is not null &&
            !organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization, customer))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (!locationAuthorizationService.CanModify(existingLocation, customer))
        {
            throw new Unauthorized();
        }

        var organizationTags = existingLocation.Organization is null
            ? []
            : await repositoryFactory.OrganizationTagRepository.Query(
                new Specification<OrganizationTag>
                {
                    Criteria = query => !query.DeletedAt.HasValue &&
                                        customTagIds.Concat(zoneIds).Contains(query.Id) &&
                                        query.Organization.Id == existingLocation.Organization.Id &&
                                        !query.Organization.DeletedAt.HasValue
                }).ToListAsync(cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var rooms = new List<Room>();
        for (var idx = 1; idx <= count; idx++)
        {
            var roomName = string.IsNullOrWhiteSpace(namePrefix) ? idx.ToString() : $"{namePrefix}{idx}";
            string finalRoomName;
            var suffixIdx = 0;
            do
            {
                finalRoomName = suffixIdx == 0 ? roomName : $"{roomName}_{suffixIdx}";
                var name = finalRoomName;

                if (!existingLocation.Rooms.Any(item => item.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    break;
                }

                ++suffixIdx;
            } while (true);

            var roomEntity = mapper.MapTo(
                new Room { Id = randomHelper.Generate(), Name = finalRoomName },
                existingLocation,
                organizationTags);

            roomEntity.Deactivated = deactivated;
            roomEntity.RequireBookingApproval = requireBookingApproval;
            roomEntity.Color = color;
            rooms.Add(mapper.MapTo(repositoryFactory.RoomRepository.Add(roomEntity), mapper.MapTo(existingLocation)));
        }

        await locationOutboxPublisher.PublishLocationAsync([mapper.MapTo(existingLocation)], repositoryFactory.UnitOfWork, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return rooms;
    }

    public async Task<Room> UpdateAsync(Room room, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(room.Id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingRoom = await repositoryFactory.RoomRepository.GetByIdAsync(room.Id, false, cancellationToken);
        if (existingRoom is null)
        {
            throw new RoomNotFound();
        }

        return await UpdateInternalAsync(room, existingRoom, customer, cancellationToken);
    }

    public async Task<Room> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var room = await repositoryFactory.RoomRepository.GetByIdAsync(id, false, cancellationToken);
        if (room is null)
        {
            throw new RoomNotFound();
        }

        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(room.Location.Id, cancellationToken);
        if (existingLocation is null)
        {
            throw new LocationNotFound();
        }

        if (existingLocation.Organization is not null &&
            !organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization, customer))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (!locationAuthorizationService.CanModify(existingLocation, customer))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var deletedRoom = mapper.MapTo(repositoryFactory.RoomRepository.Remove(room), mapper.MapTo(existingLocation));

        var mappedLocation = mapper.MapTo(existingLocation);
        mappedLocation.Rooms = mappedLocation.Rooms.Where(item => item.Id != id).ToList();

        await locationOutboxPublisher.PublishLocationAsync([mappedLocation], repositoryFactory.UnitOfWork, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return deletedRoom;
    }

    public async Task<ICollection<Room>> DeleteAsync(ICollection<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var rooms = await repositoryFactory.RoomRepository.GetByIdsAsync(ids, false, cancellationToken);
        var locationIds = rooms.Select(item => item.Location.Id).ToList();
        var existingLocations = await repositoryFactory.LocationRepository.GetByIdsAsync(locationIds, cancellationToken);

        if (existingLocations
            .Where(item => item.Organization is not null)
            .Any(existingLocation => !organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization!, customer)))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (existingLocations.Any(existingOrganization => !locationAuthorizationService.CanModify(existingOrganization, customer)))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.RoomRepository.RemoveRange(rooms);

        var deletedRooms = rooms
            .Select(room => mapper.MapTo(room, mapper.MapTo(existingLocations.Single(item => item.Id == room.Location.Id))))
            .ToList();

        var mappedLocations = existingLocations.Select(mapper.MapTo).ToList();
        foreach (var mappedLocation in mappedLocations)
        {
            mappedLocation.Rooms = mappedLocation.Rooms.Where(item => !ids.Contains(item.Id)).ToList();
        }

        await locationOutboxPublisher.PublishLocationAsync(mappedLocations, repositoryFactory.UnitOfWork, cancellationToken);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return deletedRooms;
    }

    public async Task<ICollection<Room>> ActivateAsync(ICollection<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var rooms = await repositoryFactory.RoomRepository.GetByIdsAsync(ids, false, cancellationToken);
        var locationIds = rooms.Select(item => item.Location.Id).ToList();
        var existingLocations = await repositoryFactory.LocationRepository.GetByIdsAsync(locationIds, cancellationToken);

        if (existingLocations
            .Where(item => item.Organization is not null)
            .Any(existingLocation => !organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization!, customer)))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (existingLocations.Any(existingOrganization => !locationAuthorizationService.CanModify(existingOrganization, customer)))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        foreach (var room in rooms)
        {
            room.Deactivated = false;
            repositoryFactory.RoomRepository.Update(room);
        }

        var updatedRooms = rooms
            .Select(room => mapper.MapTo(room, mapper.MapTo(existingLocations.Single(item => item.Id == room.Location.Id))))
            .ToList();

        var mappedLocations = existingLocations.Select(mapper.MapTo).ToList();
        foreach (var room in mappedLocations.SelectMany(mappedLocation => mappedLocation.Rooms.Where(item => !ids.Contains(item.Id))))
        {
            room.Deactivated = false;
        }

        await locationOutboxPublisher.PublishLocationAsync(mappedLocations, repositoryFactory.UnitOfWork, cancellationToken);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return updatedRooms;
    }

    public async Task<ICollection<Room>> DeactivateAsync(ICollection<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var rooms = await repositoryFactory.RoomRepository.GetByIdsAsync(ids, false, cancellationToken);
        var locationIds = rooms.Select(item => item.Location.Id).ToList();
        var existingLocations = await repositoryFactory.LocationRepository.GetByIdsAsync(locationIds, cancellationToken);

        if (existingLocations
            .Where(item => item.Organization is not null)
            .Any(existingLocation => !organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization!, customer)))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (existingLocations.Any(existingOrganization => !locationAuthorizationService.CanModify(existingOrganization, customer)))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        foreach (var room in rooms)
        {
            room.Deactivated = true;
            repositoryFactory.RoomRepository.Update(room);
        }

        var updatedRooms = rooms
            .Select(room => mapper.MapTo(room, mapper.MapTo(existingLocations.Single(item => item.Id == room.Location.Id))))
            .ToList();

        var mappedLocations = existingLocations.Select(mapper.MapTo).ToList();
        foreach (var room in mappedLocations.SelectMany(mappedLocation => mappedLocation.Rooms.Where(item => !ids.Contains(item.Id))))
        {
            room.Deactivated = true;
        }

        await locationOutboxPublisher.PublishLocationAsync(mappedLocations, repositoryFactory.UnitOfWork, cancellationToken);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return updatedRooms;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Room>>, int)> GetPaginatedRoomsAsync(
        PaginationInputParam paginationInputParam,
        RoomSearchCriteria searchCriteria,
        ICollection<RoomOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(searchCriteria.LocationId, cancellationToken);
        if (location is null)
        {
            throw new LocationNotFound();
        }

        if (!locationAuthorizationService.CanView(location, customer))
        {
            throw new Unauthorized();
        }

        var (paginatedInfo, edges, totalCount) =
            await repositoryFactory.RoomRepository.GetPaginatedRoomsAsync(
                paginationInputParam,
                searchCriteria,
                orderByFields,
                cancellationToken);

        return (paginatedInfo, mapper.MapTo(edges, mapper.MapTo(location)).ToList(), totalCount);
    }

    private async Task<Room> UpdateInternalAsync(
        Room room,
        Shared.Database.Entities.Room existingRoom,
        Customer? customer,
        CancellationToken cancellationToken)
    {
        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(existingRoom.Location.Id, cancellationToken);
        if (existingLocation is null)
        {
            throw new LocationNotFound();
        }

        if (customer is not null &&
            existingLocation.Organization is not null &&
            !organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization, customer))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (customer is not null && !locationAuthorizationService.CanModify(existingLocation, customer))
        {
            throw new Unauthorized();
        }

        var roomId = room.Id;
        var roomName = room.Name;
        var customTags = room.CustomTags;
        var zones = room.Zones;
        var locationId = existingRoom.Location.Id;
        var matchingRoomFound = await repositoryFactory.RoomRepository.Query(
            new Specification<Shared.Database.Entities.Room>
            {
                Criteria = query => !query.DeletedAt.HasValue &&
                                    query.Location.Id == locationId &&
                                    EF.Functions.ILike(query.Name, roomName) &&
                                    query.Id != roomId
            }).AnyAsync(cancellationToken);
        if (matchingRoomFound)
        {
            throw new RoomWithSameNameExist();
        }

        var organizationTags = existingLocation.Organization is null
            ? []
            : await repositoryFactory.OrganizationTagRepository.Query(
                new Specification<OrganizationTag>
                {
                    Criteria = query => !query.DeletedAt.HasValue &&
                                        customTags.Concat(zones).Select(item => item.Id).Contains(query.Id) &&
                                        query.Organization.Id == existingLocation.Organization.Id &&
                                        !query.Organization.DeletedAt.HasValue
                }).ToListAsync(cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        room = mapper.MapTo(
            repositoryFactory.RoomRepository.Update(mapper.MergeTo(room, existingRoom, existingLocation, organizationTags)),
            mapper.MapTo(existingLocation));

        await locationOutboxPublisher.PublishLocationAsync([mapper.MapTo(existingLocation)], repositoryFactory.UnitOfWork, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return room;
    }
}
