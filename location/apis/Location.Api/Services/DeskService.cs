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

public interface IDeskService
{
    Task<Desk> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Desk> AddAsync(Desk desk, bool ignoreAuthorizationCheck, CancellationToken cancellationToken);

    Task<ICollection<Desk>> BulkAddAsync(
        string locationId,
        string? namePrefix,
        int count,
        ICollection<string> customTagIds,
        ICollection<string> zoneIds,
        bool deactivated,
        bool requireBookingApproval,
        string? color,
        CancellationToken cancellationToken);

    Task<Desk> UpdateAsync(Desk desk, CancellationToken cancellationToken);
    Task<Desk> DeleteAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<Desk>> DeleteAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Task<ICollection<Desk>> ActivateAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Task<ICollection<Desk>> DeactivateAsync(ICollection<string> ids, CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<Desk>>, int )> GetPaginatedDesksAsync(
        PaginationInputParam paginationInputParam,
        DeskSearchCriteria searchCriteria,
        ICollection<DeskOrder> orderByFields,
        CancellationToken cancellationToken);
}

public class DeskService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ICachedCustomerService cachedCustomerService,
    ICustomerService customerService,
    ILocationAuthorizationService locationAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    IMapper mapper,
    ILocationOutboxPublisher locationOutboxPublisher) : IDeskService
{
    public async Task<Desk> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var desk = await repositoryFactory.DeskRepository.GetByIdAsync(id, false, cancellationToken);
        if (desk is null)
        {
            throw new DeskNotFound();
        }

        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(desk.Location.Id, cancellationToken);
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

        return mapper.MapTo(desk);
    }

    public async Task<Desk> AddAsync(Desk desk, bool ignoreAuthorizationCheck, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(desk.Location.Id);

        Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(desk.Id))
        {
            var existingDesk = await repositoryFactory.DeskRepository.GetByIdAsync(desk.Id, false, cancellationToken);
            if (existingDesk is not null)
            {
                return await UpdateInternalAsync(desk, existingDesk, customer, cancellationToken);
            }
        }
        else
        {
            desk.Id = randomHelper.Generate();
        }

        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(desk.Location.Id, cancellationToken);
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

        var matchingDeskFound = await repositoryFactory.DeskRepository.Query(
                new Specification<Shared.Database.Entities.Desk>
                {
                    Criteria = query =>
                        !query.DeletedAt.HasValue && query.Location.Id == desk.Location.Id && EF.Functions.ILike(query.Name, desk.Name)
                })
            .AnyAsync(cancellationToken);
        if (matchingDeskFound)
        {
            throw new DeskWithSameNameExist();
        }

        var organizationTags = existingLocation.Organization is null
            ? []
            : await repositoryFactory.OrganizationTagRepository.Query(
                new Specification<OrganizationTag>
                {
                    Criteria = query => !query.DeletedAt.HasValue &&
                                        desk.CustomTags.Concat(desk.Zones).Select(item => item.Id).Contains(query.Id) &&
                                        query.Organization.Id == existingLocation.Organization.Id &&
                                        !query.Organization.DeletedAt.HasValue
                }).ToListAsync(cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.LocationRepository.UnitOfWork,
            cancellationToken);

        var mappedDesk = mapper.MapTo(repositoryFactory.DeskRepository.Add(mapper.MapTo(desk, existingLocation, organizationTags)));
        await locationOutboxPublisher.PublishLocationAsync(
            [mapper.MapTo(existingLocation)],
            repositoryFactory.DeskRepository.UnitOfWork,
            cancellationToken);
        await repositoryFactory.DeskRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return mappedDesk;
    }

    public async Task<ICollection<Desk>> BulkAddAsync(
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

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.LocationRepository.UnitOfWork,
            cancellationToken);

        var desks = new List<Desk>();
        for (var idx = 1; idx <= count; idx++)
        {
            var deskName = string.IsNullOrWhiteSpace(namePrefix) ? idx.ToString() : $"{namePrefix}{idx}";
            string finalDeskName;
            var suffixIdx = 0;
            do
            {
                finalDeskName = suffixIdx == 0 ? deskName : $"{deskName}_{suffixIdx}";
                var name = finalDeskName;

                if (!existingLocation.Desks.Any(item => item.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    break;
                }

                ++suffixIdx;
            } while (true);

            var deskEntity = mapper.MapTo(
                new Desk { Id = randomHelper.Generate(), Name = finalDeskName },
                existingLocation,
                organizationTags);

            deskEntity.Deactivated = deactivated;
            deskEntity.RequireBookingApproval = requireBookingApproval;
            deskEntity.Color = color;
            desks.Add(mapper.MapTo(repositoryFactory.DeskRepository.Add(deskEntity), mapper.MapTo(existingLocation)));
        }

        await locationOutboxPublisher.PublishLocationAsync(
            [mapper.MapTo(existingLocation)],
            repositoryFactory.DeskRepository.UnitOfWork,
            cancellationToken);
        await repositoryFactory.DeskRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return desks;
    }

    public async Task<Desk> UpdateAsync(Desk desk, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(desk.Id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingDesk = await repositoryFactory.DeskRepository.GetByIdAsync(desk.Id, false, cancellationToken);
        if (existingDesk is null)
        {
            throw new DeskNotFound();
        }

        return await UpdateInternalAsync(desk, existingDesk, customer, cancellationToken);
    }

    public async Task<Desk> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var desk = await repositoryFactory.DeskRepository.GetByIdAsync(id, false, cancellationToken);
        if (desk is null)
        {
            throw new DeskNotFound();
        }

        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(desk.Location.Id, cancellationToken);
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

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.DeskRepository.UnitOfWork,
            cancellationToken);

        var deletedDesk = mapper.MapTo(repositoryFactory.DeskRepository.Remove(desk), mapper.MapTo(existingLocation));

        var mappedLocation = mapper.MapTo(existingLocation);
        mappedLocation.Desks = mappedLocation.Desks.Where(item => item.Id != id).ToList();

        await locationOutboxPublisher.PublishLocationAsync(
            [mappedLocation],
            repositoryFactory.DeskRepository.UnitOfWork,
            cancellationToken);
        await repositoryFactory.DeskRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return deletedDesk;
    }

    public async Task<ICollection<Desk>> DeleteAsync(ICollection<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var desks = await repositoryFactory.DeskRepository.GetByIdsAsync(ids, false, cancellationToken);
        var locationIds = desks.Select(item => item.Location.Id).ToList();
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

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.DeskRepository.UnitOfWork,
            cancellationToken);

        repositoryFactory.DeskRepository.RemoveRange(desks);

        var deletedDesks = desks
            .Select(desk => mapper.MapTo(desk, mapper.MapTo(existingLocations.Single(item => item.Id == desk.Location.Id))))
            .ToList();

        var mappedLocations = existingLocations.Select(mapper.MapTo).ToList();
        foreach (var mappedLocation in mappedLocations)
        {
            mappedLocation.Desks = mappedLocation.Desks.Where(item => !ids.Contains(item.Id)).ToList();
        }

        await locationOutboxPublisher.PublishLocationAsync(
            mappedLocations,
            repositoryFactory.DeskRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.DeskRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return deletedDesks;
    }

    public async Task<ICollection<Desk>> ActivateAsync(ICollection<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var desks = await repositoryFactory.DeskRepository.GetByIdsAsync(ids, false, cancellationToken);
        var locationIds = desks.Select(item => item.Location.Id).ToList();
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

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.DeskRepository.UnitOfWork,
            cancellationToken);

        foreach (var desk in desks)
        {
            desk.Deactivated = false;
            repositoryFactory.DeskRepository.Update(desk);
        }

        var updatedDesks = desks
            .Select(desk => mapper.MapTo(desk, mapper.MapTo(existingLocations.Single(item => item.Id == desk.Location.Id))))
            .ToList();

        var mappedLocations = existingLocations.Select(mapper.MapTo).ToList();
        foreach (var desk in mappedLocations.SelectMany(mappedLocation => mappedLocation.Desks.Where(item => !ids.Contains(item.Id))))
        {
            desk.Deactivated = false;
        }

        await locationOutboxPublisher.PublishLocationAsync(
            mappedLocations,
            repositoryFactory.DeskRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.DeskRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return updatedDesks;
    }

    public async Task<ICollection<Desk>> DeactivateAsync(ICollection<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var desks = await repositoryFactory.DeskRepository.GetByIdsAsync(ids, false, cancellationToken);
        var locationIds = desks.Select(item => item.Location.Id).ToList();
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

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.DeskRepository.UnitOfWork,
            cancellationToken);

        foreach (var desk in desks)
        {
            desk.Deactivated = true;
            repositoryFactory.DeskRepository.Update(desk);
        }

        var updatedDesks = desks
            .Select(desk => mapper.MapTo(desk, mapper.MapTo(existingLocations.Single(item => item.Id == desk.Location.Id))))
            .ToList();

        var mappedLocations = existingLocations.Select(mapper.MapTo).ToList();
        foreach (var desk in mappedLocations.SelectMany(mappedLocation => mappedLocation.Desks.Where(item => !ids.Contains(item.Id))))
        {
            desk.Deactivated = true;
        }

        await locationOutboxPublisher.PublishLocationAsync(
            mappedLocations,
            repositoryFactory.DeskRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.DeskRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return updatedDesks;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Desk>>, int)> GetPaginatedDesksAsync(
        PaginationInputParam paginationInputParam,
        DeskSearchCriteria searchCriteria,
        ICollection<DeskOrder> orderByFields,
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
            await repositoryFactory.DeskRepository.GetPaginatedDesksAsync(
                paginationInputParam,
                searchCriteria,
                orderByFields,
                cancellationToken);

        return (paginatedInfo, mapper.MapTo(edges, mapper.MapTo(location)).ToList(), totalCount);
    }

    private async Task<Desk> UpdateInternalAsync(
        Desk desk,
        Shared.Database.Entities.Desk existingDesk,
        Customer? customer,
        CancellationToken cancellationToken)
    {
        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(existingDesk.Location.Id, cancellationToken);
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

        var deskId = desk.Id;
        var deskName = desk.Name;
        var customTags = desk.CustomTags;
        var zones = desk.Zones;
        var locationId = existingDesk.Location.Id;
        var matchingDeskFound = await repositoryFactory.DeskRepository.Query(
            new Specification<Shared.Database.Entities.Desk>
            {
                Criteria = query => !query.DeletedAt.HasValue &&
                                    query.Location.Id == locationId &&
                                    EF.Functions.ILike(query.Name, deskName) &&
                                    query.Id != deskId
            }).AnyAsync(cancellationToken);
        if (matchingDeskFound)
        {
            throw new DeskWithSameNameExist();
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

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.DeskRepository.UnitOfWork,
            cancellationToken);

        desk = mapper.MapTo(
            repositoryFactory.DeskRepository.Update(mapper.MergeTo(desk, existingDesk, existingLocation, organizationTags)),
            mapper.MapTo(existingLocation));

        await locationOutboxPublisher.PublishLocationAsync(
            [mapper.MapTo(existingLocation)],
            repositoryFactory.DeskRepository.UnitOfWork,
            cancellationToken);
        await repositoryFactory.DeskRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return desk;
    }
}
