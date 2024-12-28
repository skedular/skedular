using Api.Shared.Services.Models;
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
        LocationMembershipType membershipType,
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
        LocationMembershipType membershipType,
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

        if (myMembershipDetails.MembershipType == LocationMembershipTypeConstants.Administrator &&
            membershipType == LocationMembershipType.Owner)
        {
            throw new Unauthorized();
        }

        if (myMembershipDetails.MembershipType == LocationMembershipTypeConstants.Member &&
            membershipType == LocationMembershipType.Administrator)
        {
            throw new Unauthorized();
        }

        var mappedMembershipType = membershipType switch
        {
            LocationMembershipType.Owner => LocationMembershipTypeConstants.Owner,
            LocationMembershipType.Administrator => LocationMembershipTypeConstants.Administrator,
            LocationMembershipType.Member => LocationMembershipTypeConstants.Member,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (locationMember.MembershipType == mappedMembershipType)
        {
            return mapper.MapTo(locationMember, mapper.MapTo(location));
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.LocationMemberRepository.UnitOfWork,
            cancellationToken);

        locationMember.MembershipType = mappedMembershipType;
        repositoryFactory.LocationMemberRepository.Update(locationMember);

        await locationOutboxPublisher.PublishLocationAsync(
            [mapper.MapTo(location)],
            repositoryFactory.LocationRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.LocationMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return mapper.MapTo(locationMember, mapper.MapTo(location));
    }
}
