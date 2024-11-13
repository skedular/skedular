using Booking.Api.Mappers;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Time;
using Microsoft.EntityFrameworkCore;

namespace Booking.Api.Services;

public interface IDeskService
{
    Task<ICollection<Desk>> GetAvailableDesksByOrganizationAsync(
        string organizationId,
        DateTimeOffset date,
        ICollection<string> deskIdsToInclude,
        CancellationToken cancellationToken);

    Task<ICollection<Desk>> GetAvailableDesksAsync(
        string locationId,
        DateTimeOffset date,
        ICollection<string> deskIdsToInclude,
        CancellationToken cancellationToken);
}

public class DeskService(
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ILocationAuthorizationService locationAuthorizationService,
    IMapper mapper) : IDeskService
{
    public async Task<ICollection<Desk>> GetAvailableDesksByOrganizationAsync(
        string organizationId,
        DateTimeOffset date,
        ICollection<string> deskIdsToInclude,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(organizationId))
        {
            return [];
        }

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanViewOrganizationDetails(organization, customer))
        {
            throw new Unauthorized();
        }

        var desks = deskIdsToInclude.Count == 0
            ? await repositoryFactory.DeskRepository.Query(new Specification<Shared.Database.Entities.Desk>
                    {
                        Criteria = query =>
                            !query.DeletedAt.HasValue &&
                            !query.Deactivated &&
                            query.Location != null &&
                            query.Location.Organization != null &&
                            query.Location.Organization.Id == organizationId &&
                            !query.Bookings.Any(booking =>
                                !booking.DeletedAt.HasValue && booking.From >= date && booking.To < date.Tomorrow() &&
                                booking.Location != null && booking.Location.Organization != null &&
                                booking.Location.Organization.Id == organizationId)
                    }
                    .AddInclude(query => query.Location)
                    .AddInclude(query => query.Tags.Where(tag => !tag.DeletedAt.HasValue)))
                .ToListAsync(cancellationToken)
            : await repositoryFactory.DeskRepository.Query(new Specification<Shared.Database.Entities.Desk>
                    {
                        Criteria = query =>
                            (!query.DeletedAt.HasValue && !query.Deactivated &&
                             query.Location != null &&
                             query.Location.Organization != null &&
                             query.Location.Organization.Id == organizationId &&
                             !query.Bookings.Any(booking =>
                                 !booking.DeletedAt.HasValue &&
                                 booking.From >= date &&
                                 booking.To < date.Tomorrow() &&
                                 booking.Location != null &&
                                 booking.Location.Organization != null &&
                                 booking.Location.Organization.Id == organizationId)) ||
                            deskIdsToInclude.Contains(query.Id)
                    }
                    .AddInclude(query => query.Location)
                    .AddInclude(query => query.Tags.Where(tag => !tag.DeletedAt.HasValue))
                    .ApplyOrderBy(query => query.Location.Name))
                .ToListAsync(cancellationToken);

        return mapper.MapTo(desks).Select(item =>
        {
            item.Location = mapper.MapTo(desks.Single(desk => desk.Id == item.Id).Location);

            return item;
        }).ToList();
    }

    public async Task<ICollection<Desk>> GetAvailableDesksAsync(
        string locationId,
        DateTimeOffset date,
        ICollection<string> deskIdsToInclude,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(locationId))
        {
            return [];
        }

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var location =
            await repositoryFactory.LocationRepository.GetByIdAndExcludeDeactivatedDesksAsync(
                locationId,
                cancellationToken);
        if (location is null)
        {
            throw new LocationNotFound();
        }

        if (!locationAuthorizationService.CanViewLocationDetails(location, customer))
        {
            throw new Unauthorized();
        }

        var desks = deskIdsToInclude.Count == 0
            ? await repositoryFactory.DeskRepository.Query(new Specification<Shared.Database.Entities.Desk>
                    {
                        Criteria = query =>
                            !query.DeletedAt.HasValue && !query.Deactivated && query.Location.Id == locationId &&
                            !query.Bookings.Any(booking =>
                                !booking.DeletedAt.HasValue && booking.From >= date && booking.To < date.Tomorrow() &&
                                booking.Location.Id == locationId)
                    }
                    .AddInclude(query => query.Tags.Where(tag => !tag.DeletedAt.HasValue)))
                .ToListAsync(cancellationToken)
            : await repositoryFactory.DeskRepository.Query(new Specification<Shared.Database.Entities.Desk>
                    {
                        Criteria = query =>
                            (!query.DeletedAt.HasValue && !query.Deactivated && query.Location.Id == locationId &&
                             !query.Bookings.Any(booking =>
                                 !booking.DeletedAt.HasValue && booking.From >= date && booking.To < date.Tomorrow() &&
                                 booking.Location.Id == locationId)) ||
                            deskIdsToInclude.Contains(query.Id)
                    }
                    .AddInclude(query => query.Tags.Where(tag => !tag.DeletedAt.HasValue)))
                .ToListAsync(cancellationToken);

        return mapper.MapTo(desks, mapper.MapTo(location)!).ToList();
    }
}
