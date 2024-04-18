using Booking.Shared.Database;
using Booking.Shared.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
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
    internal static IIncludableQueryable<Database.Entities.Booking, Team> AddDependentObjects(
        this IQueryable<Database.Entities.Booking> originalQuery) =>
        originalQuery
            .Include(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .Include(query => query.Organization)
            .Include(query => query.Location)
            .Include(query => query.Desks)
            .ThenInclude(query => query.Tags.Where(tag => !tag.DeletedAt.HasValue))
            .Include(query => query.Team);

    internal static IQueryable<Database.Entities.Booking> AddSearchCriteria(
        this IQueryable<Database.Entities.Booking> query,
        BookingSearchCriteria searchCriteria)
    {
        query = query.Where(item => !item.DeletedAt.HasValue);

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
            query = query.Where(item => item.To > searchCriteria.ToGT);
        }

        if (searchCriteria.ToGTE is not null)
        {
            query = query.Where(item => item.To >= searchCriteria.ToGTE);
        }

        if (searchCriteria.ToLT is not null)
        {
            query = query.Where(item => item.To < searchCriteria.ToLT);
        }

        if (searchCriteria.ToLTE is not null)
        {
            query = query.Where(item => item.To <= searchCriteria.ToLTE);
        }

        if (!string.IsNullOrWhiteSpace(searchCriteria.CustomerId))
        {
            query = query.Where(item => searchCriteria.CustomerId == item.Customer.Id);
        }

        if (searchCriteria.OrganizationIds.Count != 0)
        {
            query = query.Where(item =>
                item.Organization != null && searchCriteria.OrganizationIds.Contains(item.Organization.Id));
        }

        if (searchCriteria.LocationIds.Count != 0)
        {
            query = query.Where(item => item.Location != null && searchCriteria.LocationIds.Contains(item.Location.Id));
        }

        if (searchCriteria.TeamIds.Count != 0)
        {
            query = query.Where(item => item.Team != null && searchCriteria.TeamIds.Contains(item.Team.Id));
        }

        if (!string.IsNullOrWhiteSpace(searchCriteria.NotesContains))
        {
            query = query.Where(item =>
                item.Notes != null &&
                EF.Functions.ILike(item.Notes, $"%{searchCriteria.NotesContains}%"));
        }

        if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
        {
            query = query.Where(item =>
                (item.Customer.Name != null &&
                 EF.Functions.ILike(item.Customer.Name, $"%{searchCriteria.NameContains}%")) ||
                (item.Customer.GivenName != null &&
                 EF.Functions.ILike(item.Customer.GivenName, $"%{searchCriteria.NameContains}%")) ||
                (item.Customer.MiddleName != null &&
                 EF.Functions.ILike(item.Customer.MiddleName, $"%{searchCriteria.NameContains}%")) ||
                (item.Customer.FamilyName != null &&
                 EF.Functions.ILike(item.Customer.FamilyName, $"%{searchCriteria.NameContains}%")));
        }

        return query;
    }

    internal static IQueryable<Database.Entities.Booking> AddSortingOrders(
        this IQueryable<Database.Entities.Booking> originalQuery,
        ICollection<BookingOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return originalQuery.OrderBy(query => query.CreatedAt);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            BookingOrderField.From => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.From)
                : originalQuery.OrderByDescending(x => x.From),
            BookingOrderField.To => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.To)
                : originalQuery.OrderByDescending(x => x.To),
            BookingOrderField.Notes => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Notes)
                : originalQuery.OrderByDescending(x => x.Notes),
            BookingOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Customer.Name)
                : originalQuery.OrderByDescending(x => x.Customer.Name),
            BookingOrderField.GivenName => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Customer.GivenName)
                : originalQuery.OrderByDescending(x => x.Customer.GivenName),
            BookingOrderField.MiddleName => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Customer.MiddleName)
                : originalQuery.OrderByDescending(x => x.Customer.MiddleName),
            BookingOrderField.FamilyName => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Customer.FamilyName)
                : originalQuery.OrderByDescending(x => x.Customer.FamilyName),
            BookingOrderField.OrganizationName => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Organization.Name)
                : originalQuery.OrderByDescending(x => x.Organization.Name),
            BookingOrderField.LocationName => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Location.Name)
                : originalQuery.OrderByDescending(x => x.Location.Name),
            BookingOrderField.TeamName => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Team.Name)
                : originalQuery.OrderByDescending(x => x.Team.Name),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                BookingOrderField.From => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.From)
                    : query.ThenByDescending(x => x.From),
                BookingOrderField.To => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.To)
                    : query.ThenByDescending(x => x.To),
                BookingOrderField.Notes => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Notes)
                    : query.ThenByDescending(x => x.Notes),
                BookingOrderField.Name => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Customer.Name)
                    : query.ThenByDescending(x => x.Customer.Name),
                BookingOrderField.GivenName => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Customer.GivenName)
                    : query.ThenByDescending(x => x.Customer.GivenName),
                BookingOrderField.MiddleName => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Customer.MiddleName)
                    : query.ThenByDescending(x => x.Customer.MiddleName),
                BookingOrderField.FamilyName => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Customer.FamilyName)
                    : query.ThenByDescending(x => x.Customer.FamilyName),
                BookingOrderField.OrganizationName => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Organization.Name)
                    : query.ThenByDescending(x => x.Organization.Name),
                BookingOrderField.LocationName => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Location.Name)
                    : query.ThenByDescending(x => x.Location.Name),
                BookingOrderField.TeamName => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Team.Name)
                    : query.ThenByDescending(x => x.Team.Name),
                _ => throw new ArgumentOutOfRangeException()
            });
    }

    public static IQueryable<Database.Entities.Booking> ApplyPaginationFilters(
        this IQueryable<Database.Entities.Booking> query,
        PaginationInputParam paginationInputParam,
        ICollection<BookingOrder> orderByFields)
    {
        var orderByField = orderByFields.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(paginationInputParam.After))
        {
            query = orderByField?.Field switch
            {
                BookingOrderField.From => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.From.CompareTo(paginationInputParam.After.FromCursorToDateTimeOffset()) > 0)
                    : query.Where(item =>
                        item.From.CompareTo(paginationInputParam.After.FromCursorToDateTimeOffset()) < 0),
                BookingOrderField.To => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.To.CompareTo(paginationInputParam.After.FromCursorToDateTimeOffset()) > 0)
                    : query.Where(item =>
                        item.To.CompareTo(paginationInputParam.After.FromCursorToDateTimeOffset()) < 0),
                BookingOrderField.Notes => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Notes == null || item.Notes.CompareTo(paginationInputParam.After.FromCursor()) > 0)
                    : query.Where(item =>
                        item.Notes == null || item.Notes.CompareTo(paginationInputParam.After.FromCursor()) < 0),
                BookingOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.Name == null ||
                        item.Customer.Name.CompareTo(paginationInputParam.After.FromCursor()) > 0)
                    : query.Where(item =>
                        item.Customer.Name == null ||
                        item.Customer.Name.CompareTo(paginationInputParam.After.FromCursor()) < 0),
                BookingOrderField.GivenName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.GivenName == null ||
                        item.Customer.GivenName.CompareTo(paginationInputParam.After.FromCursor()) > 0)
                    : query.Where(item =>
                        item.Customer.GivenName == null ||
                        item.Customer.GivenName.CompareTo(paginationInputParam.After.FromCursor()) < 0),
                BookingOrderField.MiddleName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.MiddleName == null ||
                        item.Customer.MiddleName.CompareTo(paginationInputParam.After.FromCursor()) > 0)
                    : query.Where(item =>
                        item.Customer.MiddleName == null ||
                        item.Customer.MiddleName.CompareTo(paginationInputParam.After.FromCursor()) < 0),
                BookingOrderField.FamilyName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.FamilyName == null ||
                        item.Customer.FamilyName.CompareTo(paginationInputParam.After.FromCursor()) > 0)
                    : query.Where(item =>
                        item.Customer.FamilyName == null ||
                        item.Customer.FamilyName.CompareTo(paginationInputParam.After.FromCursor()) < 0),
                BookingOrderField.OrganizationName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Organization == null || item.Organization.Name == null ||
                        item.Organization.Name.CompareTo(paginationInputParam.After.FromCursor()) > 0)
                    : query.Where(item =>
                        item.Organization == null || item.Organization.Name == null ||
                        item.Organization.Name.CompareTo(paginationInputParam.After.FromCursor()) < 0),
                BookingOrderField.LocationName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Location == null || item.Location.Name == null ||
                        item.Location.Name.CompareTo(paginationInputParam.After.FromCursor()) > 0)
                    : query.Where(item =>
                        item.Location == null || item.Location.Name == null ||
                        item.Location.Name.CompareTo(paginationInputParam.After.FromCursor()) < 0),
                BookingOrderField.TeamName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Team == null || item.Team.Name == null ||
                        item.Team.Name.CompareTo(paginationInputParam.After.FromCursor()) > 0)
                    : query.Where(item =>
                        item.Team == null || item.Team.Name == null ||
                        item.Team.Name.CompareTo(paginationInputParam.After.FromCursor()) < 0),
                null => query.Where(item =>
                    item.CreatedAt.CompareTo(paginationInputParam.After.FromCursorToDateTimeOffset()) > 0),
                _ => query.Where(item =>
                    item.CreatedAt.CompareTo(paginationInputParam.After.FromCursorToDateTimeOffset()) > 0)
            };
        }
        else if (!string.IsNullOrWhiteSpace(paginationInputParam.Before))
        {
            query = orderByField?.Field switch
            {
                BookingOrderField.From => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.From.CompareTo(paginationInputParam.Before.FromCursorToDateTimeOffset()) < 0)
                    : query.Where(item =>
                        item.From.CompareTo(paginationInputParam.Before.FromCursorToDateTimeOffset()) > 0),
                BookingOrderField.To => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.To.CompareTo(paginationInputParam.Before.FromCursorToDateTimeOffset()) < 0)
                    : query.Where(item =>
                        item.To.CompareTo(paginationInputParam.Before.FromCursorToDateTimeOffset()) > 0),
                BookingOrderField.Notes => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Notes == null || item.Notes.CompareTo(paginationInputParam.Before.FromCursor()) < 0)
                    : query.Where(item =>
                        item.Notes == null || item.Notes.CompareTo(paginationInputParam.Before.FromCursor()) > 0),
                BookingOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.Name == null ||
                        item.Customer.Name.CompareTo(paginationInputParam.Before.FromCursor()) < 0)
                    : query.Where(item =>
                        item.Customer.Name == null ||
                        item.Customer.Name.CompareTo(paginationInputParam.Before.FromCursor()) > 0),
                BookingOrderField.GivenName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.GivenName == null ||
                        item.Customer.GivenName.CompareTo(paginationInputParam.Before.FromCursor()) < 0)
                    : query.Where(item =>
                        item.Customer.GivenName == null ||
                        item.Customer.GivenName.CompareTo(paginationInputParam.Before.FromCursor()) > 0),
                BookingOrderField.MiddleName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.MiddleName == null ||
                        item.Customer.MiddleName.CompareTo(paginationInputParam.Before.FromCursor()) < 0)
                    : query.Where(item =>
                        item.Customer.MiddleName == null ||
                        item.Customer.MiddleName.CompareTo(paginationInputParam.Before.FromCursor()) > 0),
                BookingOrderField.FamilyName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Customer.FamilyName == null ||
                        item.Customer.FamilyName.CompareTo(paginationInputParam.Before.FromCursor()) < 0)
                    : query.Where(item =>
                        item.Customer.FamilyName == null ||
                        item.Customer.FamilyName.CompareTo(paginationInputParam.Before.FromCursor()) > 0),
                BookingOrderField.OrganizationName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Organization == null || item.Organization.Name == null ||
                        item.Organization.Name.CompareTo(paginationInputParam.Before.FromCursor()) < 0)
                    : query.Where(item =>
                        item.Organization == null || item.Organization.Name == null ||
                        item.Organization.Name.CompareTo(paginationInputParam.Before.FromCursor()) > 0),
                BookingOrderField.LocationName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Location == null || item.Location.Name == null ||
                        item.Location.Name.CompareTo(paginationInputParam.Before.FromCursor()) < 0)
                    : query.Where(item =>
                        item.Location == null || item.Location.Name == null ||
                        item.Location.Name.CompareTo(paginationInputParam.Before.FromCursor()) > 0),
                BookingOrderField.TeamName => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Team == null || item.Team.Name == null ||
                        item.Team.Name.CompareTo(paginationInputParam.Before.FromCursor()) < 0)
                    : query.Where(item =>
                        item.Team == null || item.Team.Name == null ||
                        item.Team.Name.CompareTo(paginationInputParam.Before.FromCursor()) > 0),
                null => query.Where(item =>
                    item.CreatedAt.CompareTo(paginationInputParam.Before.FromCursorToDateTimeOffset()) < 0),
                _ => query.Where(item =>
                    item.CreatedAt.CompareTo(paginationInputParam.Before.FromCursorToDateTimeOffset()) < 0)
            };
        }

        if (paginationInputParam.First is not null)
        {
            query = query.Take(paginationInputParam.First.Value + 1);
        }
        else if (paginationInputParam.Last is not null)
        {
            query = query.Take(paginationInputParam.Last.Value + 1);
        }

        return query;
    }

    public static ICollection<Edge<Database.Entities.Booking>> ToEdges(
        this ICollection<Database.Entities.Booking> items,
        ICollection<BookingOrder> orderByFields) =>
        items.Select(item => orderByFields.FirstOrDefault()?.Field switch
        {
            BookingOrderField.From => new Edge<Database.Entities.Booking>(item.From.ToCursor(), item),
            BookingOrderField.To => new Edge<Database.Entities.Booking>(item.To.ToCursor(), item),
            BookingOrderField.Notes => new Edge<Database.Entities.Booking>(item.Notes.ToCursor(), item),
            BookingOrderField.Name => new Edge<Database.Entities.Booking>(item.Customer.Name.ToCursor(), item),
            BookingOrderField.GivenName =>
                new Edge<Database.Entities.Booking>(item.Customer.GivenName.ToCursor(), item),
            BookingOrderField.MiddleName => new Edge<Database.Entities.Booking>(item.Customer.MiddleName.ToCursor(),
                item),
            BookingOrderField.FamilyName => new Edge<Database.Entities.Booking>(item.Customer.FamilyName.ToCursor(),
                item),
            BookingOrderField.OrganizationName => new Edge<Database.Entities.Booking>(
                item.Organization is null ? string.Empty : item.Organization.Name.ToCursor(), item),
            BookingOrderField.LocationName => new Edge<Database.Entities.Booking>(
                item.Location is null ? string.Empty : item.Location.Name.ToCursor(), item),
            BookingOrderField.TeamName => new Edge<Database.Entities.Booking>(
                item.Team is null ? string.Empty : item.Team.Name.ToCursor(), item),
            null => new Edge<Database.Entities.Booking>(item.CreatedAt.ToCursor(), item),
            _ => new Edge<Database.Entities.Booking>(item.CreatedAt.ToCursor(), item)
        }).ToList();
}

public class BookingRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, Database.Entities.Booking>(dbContext), IBookingRepository
{
    public async Task<Database.Entities.Booking?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Booking
            .Where(query => query.Id == id)
            .AddDependentObjects()
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<ICollection<Database.Entities.Booking>> GetAllAsync(CancellationToken cancellationToken) =>
        await DbContext.Booking
            .Where(query => !query.DeletedAt.HasValue)
            .AddDependentObjects()
            .ToListAsync(cancellationToken);

    public Database.Entities.Booking Add(Database.Entities.Booking booking)
    {
        var now = timeProvider.GetUtcNow();
        booking.CreatedAt = now;
        return DbContext.Booking.Add(booking).Entity;
    }

    public Database.Entities.Booking Update(Database.Entities.Booking booking)
    {
        var now = timeProvider.GetUtcNow();
        booking.ModifiedAt = now;
        return DbContext.Booking.Update(booking).Entity;
    }

    public Database.Entities.Booking Remove(Database.Entities.Booking booking)
    {
        var now = timeProvider.GetUtcNow();
        booking.DeletedAt = now;
        return DbContext.Booking.Update(booking).Entity;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Booking>>, int)> GetPaginatedBookingsAsync(
        PaginationInputParam paginationInputParam,
        BookingSearchCriteria searchCriteria,
        ICollection<BookingOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var totalCount = await DbContext.Booking.AsQueryable().AddSearchCriteria(searchCriteria)
            .CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return (new PaginatedInfo(false, false, null, null), [], totalCount);
        }

        var (paginatedInfo, edges) = (await DbContext.Booking
                .AsQueryable()
                .AddSearchCriteria(searchCriteria)
                .AddSortingOrders(orderByFields)
                .ApplyPaginationFilters(paginationInputParam, orderByFields)
                .AddDependentObjects()
                .ToListAsync(cancellationToken))
            .ToEdges(orderByFields)
            .GetPaginatedInfo(paginationInputParam);
        return (paginatedInfo, edges, totalCount);
    }
}
