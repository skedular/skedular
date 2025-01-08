using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Location.Shared.Database;
using Location.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Desk = Location.Shared.Database.Entities.Desk;
using OrganizationTag = Location.Shared.Database.Entities.OrganizationTag;

namespace Location.Shared.Repositories;

public interface IDeskRepository : IRepository<Desk>
{
    Task<Desk?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Desk Add(Desk desk);
    Desk Update(Desk desk);
    void RemoveRange(ICollection<Desk> desks);
    Desk Remove(Desk desk);

    Task<(PaginatedInfo, ICollection<Edge<Desk>>, int )> GetPaginatedDesksAsync(
        PaginationInputParam paginationInputParam,
        DeskSearchCriteria searchCriteria,
        ICollection<DeskOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class DeskExtensions
{
    internal static IIncludableQueryable<Desk, IEnumerable<OrganizationTag>> AddDependentObjects(
        this IQueryable<Desk> originalQuery) =>
        originalQuery
            .Include(query => query.Location)
            .Include(query => query.OrganizationTags);

    internal static IQueryable<Desk> AddSearchCriteria(
        this IQueryable<Desk> query,
        DeskSearchCriteria searchCriteria)
    {
        query = query.Where(item => !item.DeletedAt.HasValue && item.Location.Id == searchCriteria.LocationId);

        if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
        {
            query = query.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
        }

        if (searchCriteria.ZoneIds.Count != 0)
        {
            query = query.Where(item =>
                searchCriteria.ZoneIds.All(zoneId => item.OrganizationTags.Any(tag => tag.Id == zoneId)));
        }

        if (searchCriteria.DeskTypeIds.Count != 0)
        {
            query = query.Where(item =>
                searchCriteria.DeskTypeIds.All(deskTypeId => item.OrganizationTags.Any(tag => tag.Id == deskTypeId)));
        }

        return query;
    }

    internal static IQueryable<Desk> AddSortingOrders(
        this IQueryable<Desk> originalQuery,
        ICollection<DeskOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return originalQuery.OrderBy(query => query.Name).ThenBy(query => query.Id);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            DeskOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Name)
                : originalQuery.OrderByDescending(x => x.Name),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                DeskOrderField.Name => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Name)
                    : query.ThenByDescending(x => x.Name),
                _ => throw new ArgumentOutOfRangeException()
            }).ThenBy(query => query.Id);
    }
}

public class DeskRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, Desk>(dbContext, timeProvider), IDeskRepository
{
    public Desk Add(Desk desk)
    {
        var now = TimeProvider.GetUtcNow();
        desk.CreatedAt = now;
        return DbContext.Desk.Add(desk).Entity;
    }

    public void RemoveRange(ICollection<Desk> desks)
    {
        var now = TimeProvider.GetUtcNow();
        desks.ForEach(desk => desk.DeletedAt = now);
        DbContext.Desk.UpdateRange(desks);
    }

    public Desk Remove(Desk desk)
    {
        var now = TimeProvider.GetUtcNow();
        desk.DeletedAt = now;
        return DbContext.Desk.Update(desk).Entity;
    }

    public Desk Update(Desk desk)
    {
        var now = TimeProvider.GetUtcNow();
        desk.ModifiedAt = now;
        return DbContext.Desk.Update(desk).Entity;
    }

    public async Task<Desk?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Desk
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<(PaginatedInfo, ICollection<Edge<Desk>>, int)> GetPaginatedDesksAsync(
        PaginationInputParam paginationInputParam,
        DeskSearchCriteria searchCriteria,
        ICollection<DeskOrder> orderByFields,
        CancellationToken cancellationToken) =>
        (await DbContext.Desk
            .AddSearchCriteria(searchCriteria)
            .AddSortingOrders(orderByFields)
            .AddDependentObjects()
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);
}
