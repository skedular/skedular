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

    Task<LocationMember> ChangeRoleAsync(string id, LocationMemberRole memberRole, CancellationToken cancellationToken);
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
            await repositoryFactory.LocationMemberRepository.GetPaginatedLocationMembersAsync(
                paginationInputParam,
                searchCriteria,
                orderByFields,
                cancellationToken);

        return (paginatedInfo, mapper.MapTo(edges, mapper.MapTo(location)).ToList(), totalCount);
    }

    public async Task<LocationMember> ChangeRoleAsync(string id, LocationMemberRole memberRole, CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var locationMember = await repositoryFactory.LocationMemberRepository.GetByIdAsync(id, cancellationToken);
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

        var myMemberDetails = location.LocationMembers.Single(item => item.Customer.Id == customer.Id);
        if (myMemberDetails.Role == LocationMemberRoleConstants.Administrator &&
            memberRole == LocationMemberRole.Owner)
        {
            throw new Unauthorized();
        }

        if (myMemberDetails.Role == LocationMemberRoleConstants.Member &&
            memberRole == LocationMemberRole.Administrator)
        {
            throw new Unauthorized();
        }

        var mappedRole = memberRole.ToLocationMemberRole();
        if (locationMember.Role == mappedRole)
        {
            return mapper.MapTo(locationMember, mapper.MapTo(location));
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        locationMember.Role = mappedRole;
        repositoryFactory.LocationMemberRepository.Update(locationMember);

        await locationOutboxPublisher.PublishLocationAsync(
            [mapper.MapTo(location)],
            repositoryFactory.UnitOfWork,
            cancellationToken);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return mapper.MapTo(locationMember, mapper.MapTo(location));
    }
}
