using Api.Shared.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Location.Api.Mappers;
using Location.Api.Services.Authorization;
using Location.Shared.Models;
using Location.Shared.Publishers;
using Location.Shared.Repositories;

namespace Location.Api.Services;

public interface ILocationMemberService
{
    Task<(PaginatedInfo, ICollection<Edge<LocationMember>>, int )> GetPaginatedLocationMembersAsync(
        PaginationInputParam paginationInputParam,
        LocationMemberSearchCriteria searchCriteria,
        ICollection<LocationMemberOrder> orderByFields,
        CancellationToken cancellationToken);

    Task<LocationMember> ChangeMembershipTypeAsync(
        string locationMemberId,
        string membershipType,
        CancellationToken cancellationToken);

    Task<Shared.Models.Location> UpdateMembersAsync(
        string locationId,
        ICollection<LocationMember> members,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);
}

public class LocationMemberService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    ICustomerService customerService,
    ILocationAuthorizationService locationAuthorizationService,
    ILocationOutboxPublisher locationOutboxPublisher,
    IMapper mapper) : ILocationMemberService
{
    public async Task<(PaginatedInfo, ICollection<Edge<LocationMember>>, int)>
        GetPaginatedLocationMembersAsync(
            PaginationInputParam paginationInputParam,
            LocationMemberSearchCriteria searchCriteria,
            ICollection<LocationMemberOrder> orderByFields,
            CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var location =
            await repositoryFactory.LocationRepository.GetByIdAsync(searchCriteria.LocationId,
                cancellationToken);
        if (location is null)
        {
            throw new LocationNotFound();
        }

        if (!locationAuthorizationService.CanView(location, customer))
        {
            throw new Unauthorized();
        }

        var (paginatedInfo, edges, totalCount) =
            await repositoryFactory.LocationMemberRepository.GetPaginatedLocationMembersAsync(
                paginationInputParam,
                searchCriteria,
                orderByFields,
                cancellationToken);

        return (paginatedInfo, mapper.MapTo(edges, mapper.MapTo(location)).ToList(), totalCount);
    }

    public async Task<LocationMember> ChangeMembershipTypeAsync(
        string locationMemberId,
        string membershipType,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var locationMember =
            await repositoryFactory.LocationMemberRepository.GetByIdAsync(locationMemberId, cancellationToken);
        if (locationMember is null)
        {
            throw new LocationMemberNotFound();
        }

        var location = await repositoryFactory.LocationRepository.GetByIdAsync(
            locationMember.Location.Id,
            cancellationToken);
        if (location is null)
        {
            throw new LocationNotFound();
        }

        if (!locationAuthorizationService.CanModify(location, customer))
        {
            throw new Unauthorized();
        }

        var myMembershipDetails =
            location.LocationMembers.Single(item => item.Customer.Id == customer.Id);

        if (myMembershipDetails.NewMembershipType == LocationMembershipType.Administrator &&
            membershipType == LocationMembershipType.Owner)
        {
            throw new Unauthorized();
        }

        if (myMembershipDetails.NewMembershipType == LocationMembershipType.Member &&
            membershipType == LocationMembershipType.Administrator)
        {
            throw new Unauthorized();
        }

        if (locationMember.NewMembershipType == membershipType)
        {
            return mapper.MapTo(locationMember, mapper.MapTo(location));
        }

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.LocationMemberRepository.UnitOfWork,
                cancellationToken);

        locationMember.NewMembershipType = membershipType;
        repositoryFactory.LocationMemberRepository.Update(locationMember);

        await locationOutboxPublisher.PublishLocationAsync(
            [mapper.MapTo(location)],
            repositoryFactory.LocationRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.LocationMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return mapper.MapTo(locationMember, mapper.MapTo(location));
    }

    public async Task<Shared.Models.Location> UpdateMembersAsync(
        string locationId,
        ICollection<LocationMember> members,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        }

        var location = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken);
        if (location is null)
        {
            throw new LocationNotFound();
        }

        if (customer is not null && !locationAuthorizationService.CanModify(location, customer))
        {
            throw new Unauthorized();
        }

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.LocationMemberRepository.UnitOfWork,
                cancellationToken);

        var itemsToRemove = location.LocationMembers
            .Where(teamMember => members.All(item => item.Id != teamMember.Id))
            .ToList();

        var updatedItems = new List<Shared.Database.Entities.LocationMember>();
        foreach (var teamMember in location.LocationMembers
                     .Where(teamMember =>
                         members.Any(item => item.Id == teamMember.Id)))
        {
            var customerToAdd =
                await repositoryFactory.CustomerRepository.UpsertNakedAsync(teamMember.Customer.Id,
                    cancellationToken);

            updatedItems.Add(repositoryFactory.LocationMemberRepository.Update(
                mapper.MergeToEntity(
                    members.Single(item => item.Id == teamMember.Id),
                    teamMember,
                    location,
                    customerToAdd)));
        }

        var addedItems = new List<Shared.Database.Entities.LocationMember>();
        foreach (var teamMember in members.Where(teamMember =>
                     location.LocationMembers.All(item => item.Id != teamMember.Id)))
        {
            var customerToAdd =
                await repositoryFactory.CustomerRepository.UpsertNakedAsync(teamMember.Customer.Id,
                    cancellationToken);

            addedItems.Add(repositoryFactory.LocationMemberRepository.Add(
                mapper.MapToEntity(teamMember, location, customerToAdd)));
        }

        repositoryFactory.LocationMemberRepository.RemoveRange(itemsToRemove);
        location.LocationMembers = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        await locationOutboxPublisher.PublishLocationAsync(
            [mapper.MapTo(location)],
            repositoryFactory.LocationRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.LocationMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return mapper.MapTo(location);
    }
}
