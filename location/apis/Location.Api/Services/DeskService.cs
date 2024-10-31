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
using Tag = Location.Shared.Database.Entities.Tag;

namespace Location.Api.Services;

public interface IDeskService
{
    Task<Desk> GetAsync(string deskId, CancellationToken cancellationToken);

    Task<Desk> AddAsync(
        Desk desk,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);

    Task<ICollection<Desk>> BulkAddAsync(
        string locationId,
        string? namePrefix,
        int count,
        ICollection<string> tagIds,
        bool deactivated,
        bool requireBookingApproval,
        CancellationToken cancellationToken);

    Task<Desk> UpdateAsync(Desk desk, CancellationToken cancellationToken);
    Task<Desk> DeleteAsync(string deskId, CancellationToken cancellationToken);

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
    public async Task<Desk> GetAsync(string deskId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(deskId);

        var (customer, _) = await cachedCustomerService.GetCustomerAsync(cancellationToken);
        var desk = await repositoryFactory.DeskRepository.GetByIdAsync(deskId, cancellationToken);
        if (desk is null)
        {
            throw new DeskNotFound();
        }

        var existingLocation =
            await repositoryFactory.LocationRepository.GetByIdAsync(desk.Location.Id, cancellationToken);
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

    public async Task<Desk> AddAsync(
        Desk desk,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(desk.Location.Id);

        Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(desk.Id))
        {
            var existingDesk =
                await repositoryFactory.DeskRepository.GetByIdAsync(desk.Id, cancellationToken);
            if (existingDesk is not null)
            {
                return await UpdateInternalAsync(
                    desk,
                    existingDesk,
                    customer,
                    cancellationToken);
            }
        }
        else
        {
            desk.Id = randomHelper.Generate();
        }

        var existingLocation =
            await repositoryFactory.LocationRepository.GetByIdAsync(desk.Location.Id, cancellationToken);
        if (existingLocation is null)
        {
            throw new LocationNotFound();
        }

        if (customer is not null && existingLocation.Organization is not null &&
            !organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization, customer))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (customer is not null && !locationAuthorizationService.CanModify(existingLocation, customer))
        {
            throw new Unauthorized();
        }

        var matchingDeskFound = await repositoryFactory.DeskRepository
            .Query(new Specification<Shared.Database.Entities.Desk>
            {
                Criteria = query => !query.DeletedAt.HasValue &&
                                    query.Location.Id == desk.Location.Id &&
                                    EF.Functions.ILike(query.Name, desk.Name)
            }).AnyAsync(cancellationToken);
        if (matchingDeskFound)
        {
            throw new DeskWithSameNameExist();
        }

        var tagIds = desk.Tags.Select(item => item.Id).ToList();
        var tags = tagIds.Count == 0
            ? []
            : await repositoryFactory.TagRepository
                .Query(new Specification<Tag>
                {
                    Criteria = query =>
                        !query.DeletedAt.HasValue && query.Location.Id == desk.Location.Id &&
                        tagIds.Contains(query.Id)
                })
                .ToListAsync(cancellationToken);

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.LocationRepository.UnitOfWork,
                cancellationToken);

        var deskEntity = mapper.MapTo(desk, existingLocation, tags);
        _ = repositoryFactory.DeskRepository.Add(deskEntity);

        await locationOutboxPublisher.PublishLocationAsync(
            [mapper.MapTo(existingLocation)],
            repositoryFactory.DeskRepository.UnitOfWork,
            cancellationToken);
        await repositoryFactory.DeskRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return desk;
    }

    public async Task<ICollection<Desk>> BulkAddAsync(
        string locationId,
        string? namePrefix,
        int count,
        ICollection<string> tagIds,
        bool deactivated,
        bool requireBookingApproval,
        CancellationToken cancellationToken)
    {
        if (count <= 0)
        {
            return [];
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingLocation =
            await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken);
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

        var tags = tagIds.Count == 0
            ? []
            : await repositoryFactory.TagRepository
                .Query(new Specification<Tag>
                {
                    Criteria = query => !query.DeletedAt.HasValue &&
                                        query.Location.Id == locationId &&
                                        tagIds.Contains(query.Id)
                })
                .ToListAsync(cancellationToken);

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.LocationRepository.UnitOfWork,
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

                if (!existingLocation.Desks.Any(item =>
                        item.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    break;
                }

                ++suffixIdx;
            } while (true);

            var deskEntity = mapper.MapTo(
                new Desk { Id = randomHelper.Generate(), Name = finalDeskName },
                existingLocation,
                tags);

            deskEntity.Deactivated = deactivated;
            deskEntity.RequireBookingApproval = requireBookingApproval;
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
        var existingDesk =
            await repositoryFactory.DeskRepository.GetByIdAsync(desk.Id, cancellationToken);
        if (existingDesk is null)
        {
            throw new DeskNotFound();
        }

        return await UpdateInternalAsync(desk, existingDesk, customer, cancellationToken);
    }

    public async Task<Desk> DeleteAsync(string deskId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(deskId);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var desk =
            await repositoryFactory.DeskRepository.GetByIdAsync(deskId, cancellationToken);
        if (desk is null)
        {
            throw new DeskNotFound();
        }

        var existingLocation =
            await repositoryFactory.LocationRepository.GetByIdAsync(desk.Location.Id, cancellationToken);
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

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.DeskRepository.UnitOfWork,
                cancellationToken);

        var deletedDesk = mapper.MapTo(repositoryFactory.DeskRepository.Remove(desk), mapper.MapTo(existingLocation));

        var mappedLocation = mapper.MapTo(existingLocation);
        mappedLocation.Desks = mappedLocation.Desks.Where(item => item.Id != deskId).ToList();

        await locationOutboxPublisher.PublishLocationAsync(
            [mappedLocation],
            repositoryFactory.DeskRepository.UnitOfWork,
            cancellationToken);
        await repositoryFactory.DeskRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return deletedDesk;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Desk>>, int)>
        GetPaginatedDesksAsync(
            PaginationInputParam paginationInputParam,
            DeskSearchCriteria searchCriteria,
            ICollection<DeskOrder> orderByFields,
            CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetCustomerAsync(cancellationToken);
        var location =
            await repositoryFactory.LocationRepository.GetByIdAsync(searchCriteria.LocationId, cancellationToken);
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
        var existingLocation =
            await repositoryFactory.LocationRepository.GetByIdAsync(existingDesk.Location.Id, cancellationToken);
        if (existingLocation is null)
        {
            throw new LocationNotFound();
        }

        if (customer is not null && existingLocation.Organization is not null &&
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
        var locationId = existingDesk.Location.Id;
        var matchingDeskFound = await repositoryFactory.DeskRepository
            .Query(new Specification<Shared.Database.Entities.Desk>
            {
                Criteria = query =>
                    !query.DeletedAt.HasValue &&
                    query.Location.Id == locationId &&
                    EF.Functions.ILike(query.Name, deskName) &&
                    query.Id != deskId
            }).AnyAsync(cancellationToken);
        if (matchingDeskFound)
        {
            throw new DeskWithSameNameExist();
        }

        var tagIds = desk.Tags.Select(item => item.Id).ToList();
        var tags = tagIds.Count == 0
            ? []
            : await repositoryFactory.TagRepository
                .Query(new Specification<Tag>
                {
                    Criteria = query => !query.DeletedAt.HasValue &&
                                        query.Location.Id == locationId &&
                                        tagIds.Contains(query.Id)
                })
                .ToListAsync(cancellationToken);

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.DeskRepository.UnitOfWork,
                cancellationToken);

        desk =
            mapper.MapTo(
                repositoryFactory.DeskRepository.Update(mapper.MergeTo(desk, existingDesk, existingLocation, tags)),
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
