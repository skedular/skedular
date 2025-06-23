using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;
using Location.Shared.Database;
using Location.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using FloorPlan = Location.Shared.Database.Entities.FloorPlan;
using Resource = Location.Shared.Database.Entities.Resource;

namespace Location.Shared.Repositories;

public interface IFloorPlanRepository : IRepository<FloorPlan>
{
    Task<FloorPlan?> GetByIdAsync(string id, CancellationToken cancellationToken);
    void Add(FloorPlan floorPlan);
    void Update(FloorPlan floorPlan);
    void Remove(FloorPlan floorPlan);

    Task<(PaginatedInfo, ICollection<Edge<FloorPlan>>, int )> GetPaginatedFloorPlansAsync(
        PaginationInputParam paginationInputParam,
        FloorPlanSearchCriteria searchCriteria,
        ICollection<FloorPlanOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class FloorPlanExtensions
{
    internal static IIncludableQueryable<FloorPlan, Resource> AddDependentObjects(
        this IQueryable<FloorPlan> originalQuery) =>
        originalQuery
            .Include(query => query.Location)
            .Include(query => query.ResourcePositions)
            .ThenInclude(query => query.Resource);

    internal static IQueryable<FloorPlan> AddSearchCriteria(
        this IQueryable<FloorPlan> query,
        FloorPlanSearchCriteria searchCriteria)
    {
        query = query.Where(item => !item.DeletedAt.HasValue);

        if (!string.IsNullOrWhiteSpace(searchCriteria.LocationId))
        {
            query = query.Where(item => !item.Location.DeletedAt.HasValue && item.Location.Id == searchCriteria.LocationId);
        }

        return query;
    }

    internal static IQueryable<FloorPlan> AddSortingOrders(
        this IQueryable<FloorPlan> originalQuery,
        ICollection<FloorPlanOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return originalQuery.OrderBy(query => query.Name).ThenBy(query => query.Id);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            FloorPlanOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Name)
                : originalQuery.OrderByDescending(x => x.Name),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                FloorPlanOrderField.Name => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Name)
                    : query.ThenByDescending(x => x.Name),
                _ => throw new ArgumentOutOfRangeException()
            }).ThenBy(query => query.Id);
    }
}

public class FloorPlanRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, FloorPlan>(dbContext, timeProvider), IFloorPlanRepository
{
    public async Task<FloorPlan?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.FloorPlan
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id && !query.DeletedAt.HasValue, cancellationToken);

    public void Add(FloorPlan floorPlan)
    {
        var now = TimeProvider.GetUtcNow();
        floorPlan.CreatedAt = now;
        DbContext.FloorPlan.Add(floorPlan);
    }

    public void Update(FloorPlan floorPlan)
    {
        var now = TimeProvider.GetUtcNow();
        floorPlan.ModifiedAt = now;
        DbContext.FloorPlan.Update(floorPlan);
    }

    public void Remove(FloorPlan floorPlan)
    {
        var now = TimeProvider.GetUtcNow();
        floorPlan.DeletedAt = now;
        DbContext.FloorPlan.Update(floorPlan);
    }

    public async Task<(PaginatedInfo, ICollection<Edge<FloorPlan>>, int)> GetPaginatedFloorPlansAsync(
        PaginationInputParam paginationInputParam,
        FloorPlanSearchCriteria searchCriteria,
        ICollection<FloorPlanOrder> orderByFields,
        CancellationToken cancellationToken) =>
        (await DbContext.FloorPlan
            .AddSearchCriteria(searchCriteria)
            .AddSortingOrders(orderByFields)
            .AddDependentObjects()
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);
}
