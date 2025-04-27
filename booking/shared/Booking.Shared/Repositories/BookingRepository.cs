using Booking.Shared.Database;
using Booking.Shared.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Time;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Team = Booking.Shared.Database.Entities.Team;

namespace Booking.Shared.Repositories;

public interface IBookingRepository : IRepository<Database.Entities.Booking>
{
    Task<Database.Entities.Booking?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<Database.Entities.Booking>> GetAllAsync(CancellationToken cancellationToken);
    Database.Entities.Booking Add(Database.Entities.Booking booking);
    Database.Entities.Booking Update(Database.Entities.Booking booking);
    Database.Entities.Booking Remove(Database.Entities.Booking booking);

    Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Booking>>, int)> GetPaginatedBookingsAsync(
        PaginationInputParam paginationInputParam,
        BookingSearchCriteria searchCriteria,
        ICollection<BookingOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class BookingExtensions
{
    internal static IIncludableQueryable<Database.Entities.Booking, ICollection<Team>> AddDependentObjects(
        this IQueryable<Database.Entities.Booking> originalQuery) =>
        originalQuery
            .Include(query => query.ResourceBookingSlots.Where(resourceBookingSlot => !resourceBookingSlot.Resource.DeletedAt.HasValue))
            .ThenInclude(query => query.Customers)
            .Include(query => query.ResourceBookingSlots.Where(resourceBookingSlot => !resourceBookingSlot.Resource.DeletedAt.HasValue))
            .ThenInclude(query => query.Resource)
            .ThenInclude(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
            .Include(query => query.ResourceBookingSlots.Where(resourceBookingSlot => !resourceBookingSlot.Resource.DeletedAt.HasValue))
            .ThenInclude(query => query.Resource)
            .ThenInclude(query => query.Location)
            .ThenInclude(query => query.Organization)
            .Include(query => query.InvolvedCustomers)
            .ThenInclude(query => query.Identities)
            .Include(query => query.InvolvedCustomers)
            .Include(query => query.InvolvedOrganizations)
            .ThenInclude(query => query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .Include(query => query.InvolvedLocations)
            .ThenInclude(query => query.Organization)
            .Include(query => query.InvolvedTeams);

    internal static IQueryable<Database.Entities.Booking> AddSearchCriteria(
        this IQueryable<Database.Entities.Booking> query,
        BookingSearchCriteria searchCriteria,
        TimeProvider timeProvider)
    {
        query = query.Where(item => !item.DeletedAt.HasValue);

        if (searchCriteria.IncludeFutureBookingsOnly is not null && searchCriteria.IncludeFutureBookingsOnly.Value)
        {
            query = query.Where(item => item.From >= timeProvider.GetUtcNow().StartOfDay());
        }

        if (searchCriteria.FromGT is not null)
        {
            query = query.Where(item => item.From > searchCriteria.FromGT);
        }

        if (searchCriteria.FromGTE is not null)
        {
            query = query.Where(item => item.From >= searchCriteria.FromGTE);
        }

        if (searchCriteria.FromLT is not null)
        {
            query = query.Where(item => item.From < searchCriteria.FromLT);
        }

        if (searchCriteria.FromLTE is not null)
        {
            query = query.Where(item => item.From <= searchCriteria.FromLTE);
        }

        if (searchCriteria.ToGT is not null)
        {
            query = query.Where(item => item.Until > searchCriteria.ToGT);
        }

        if (searchCriteria.ToGTE is not null)
        {
            query = query.Where(item => item.Until >= searchCriteria.ToGTE);
        }

        if (searchCriteria.ToLT is not null)
        {
            query = query.Where(item => item.Until < searchCriteria.ToLT);
        }

        if (searchCriteria.ToLTE is not null)
        {
            query = query.Where(item => item.Until <= searchCriteria.ToLTE);
        }

        if (searchCriteria.CustomerIds.Count != 0)
        {
            query = query.Where(item => item.InvolvedCustomers.Any(customer => searchCriteria.CustomerIds.Contains(customer.Id)));
        }

        if (!string.IsNullOrWhiteSpace(searchCriteria.BookingType))
        {
            query = query.Where(item => item.Type == searchCriteria.BookingType);
        }

        if (searchCriteria.CombineOrganizationsLocationsTeams is not null && searchCriteria.CombineOrganizationsLocationsTeams.Value)
        {
            if (searchCriteria.OrganizationIds.Count != 0)
            {
                query = query.Where(item => item.InvolvedOrganizations.Any(organization =>
                    !organization.DeletedAt.HasValue && searchCriteria.OrganizationIds.Contains(organization.Id)));
            }

            if (searchCriteria.LocationIds.Count != 0)
            {
                query = query.Where(item =>
                    item.InvolvedLocations.Any(location => !location.DeletedAt.HasValue && searchCriteria.LocationIds.Contains(location.Id)));
            }

            if (searchCriteria.TeamIds.Count != 0)
            {
                query = query.Where(item => item.InvolvedTeams.Any(team => !team.DeletedAt.HasValue && searchCriteria.TeamIds.Contains(team.Id)));
            }
        }
        else
        {
            if (searchCriteria.OrganizationIds.Count != 0 || searchCriteria.LocationIds.Count != 0 || searchCriteria.TeamIds.Count != 0)
            {
                query = query.Where(item =>
                    !item.InvolvedOrganizations.Any(organization =>
                        !organization.DeletedAt.HasValue && searchCriteria.OrganizationIds.Contains(organization.Id)) ||
                    item.InvolvedLocations.Any(location => !location.DeletedAt.HasValue && searchCriteria.LocationIds.Contains(location.Id)) ||
                    item.InvolvedTeams.Any(team => !team.DeletedAt.HasValue && searchCriteria.TeamIds.Contains(team.Id)));
            }
        }

        if (!string.IsNullOrWhiteSpace(searchCriteria.NotesContains))
        {
            query = query.Where(item => item.Notes != null && EF.Functions.ILike(item.Notes, $"%{searchCriteria.NotesContains}%"));
        }

        if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
        {
            query = query.Where(item =>
                item.InvolvedCustomers.Any(customer => (customer.Name != null &&
                                                        EF.Functions.ILike(customer.Name, $"%{searchCriteria.NameContains}%")) ||
                                                       (customer.GivenName != null &&
                                                        EF.Functions.ILike(customer.GivenName, $"%{searchCriteria.NameContains}%")) ||
                                                       (customer.MiddleName != null &&
                                                        EF.Functions.ILike(customer.MiddleName, $"%{searchCriteria.NameContains}%")) ||
                                                       (customer.FamilyName != null &&
                                                        EF.Functions.ILike(customer.FamilyName, $"%{searchCriteria.NameContains}%"))));
        }

        return query;
    }

    internal static IQueryable<Database.Entities.Booking> AddSortingOrders(
        this IQueryable<Database.Entities.Booking> originalQuery,
        ICollection<BookingOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return originalQuery.OrderBy(query => query.From).ThenBy(query => query.Id);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            BookingOrderField.From => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.From)
                : originalQuery.OrderByDescending(x => x.From),
            BookingOrderField.To => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Until)
                : originalQuery.OrderByDescending(x => x.Until),
            BookingOrderField.Notes => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Notes)
                : originalQuery.OrderByDescending(x => x.Notes),
            BookingOrderField.BookingType => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Type)
                : originalQuery.OrderByDescending(x => x.Type),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                BookingOrderField.From => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.From)
                    : query.ThenByDescending(x => x.From),
                BookingOrderField.To => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Until)
                    : query.ThenByDescending(x => x.Until),
                BookingOrderField.Notes => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Notes)
                    : query.ThenByDescending(x => x.Notes),
                BookingOrderField.BookingType => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Type)
                    : query.ThenByDescending(x => x.Type),
                _ => throw new ArgumentOutOfRangeException()
            }).ThenBy(query => query.Id);
    }
}

public class BookingRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, Database.Entities.Booking>(dbContext, timeProvider), IBookingRepository
{
    public async Task<Database.Entities.Booking?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Booking
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<Database.Entities.Booking>> GetAllAsync(CancellationToken cancellationToken) =>
        await DbContext.Booking
            .Where(query => !query.DeletedAt.HasValue)
            .AddDependentObjects()
            .ToListAsync(cancellationToken);

    public Database.Entities.Booking Add(Database.Entities.Booking booking)
    {
        var now = TimeProvider.GetUtcNow();
        booking.CreatedAt = now;
        return DbContext.Booking.Add(booking).Entity;
    }

    public Database.Entities.Booking Update(Database.Entities.Booking booking)
    {
        var now = TimeProvider.GetUtcNow();
        booking.ModifiedAt = now;
        return DbContext.Booking.Update(booking).Entity;
    }

    public Database.Entities.Booking Remove(Database.Entities.Booking booking)
    {
        var now = TimeProvider.GetUtcNow();
        booking.DeletedAt = now;
        return DbContext.Booking.Update(booking).Entity;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Booking>>, int)> GetPaginatedBookingsAsync(
        PaginationInputParam paginationInputParam,
        BookingSearchCriteria searchCriteria,
        ICollection<BookingOrder> orderByFields,
        CancellationToken cancellationToken) =>
        (await DbContext.Booking
            .AddSearchCriteria(searchCriteria, TimeProvider)
            .AddSortingOrders(orderByFields)
            .AddDependentObjects()
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);
}
