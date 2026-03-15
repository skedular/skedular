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
    extension(IQueryable<FloorPlan> originalQuery)
    {
        internal IIncludableQueryable<FloorPlan, Resource> AddDependentObjects() =>
            originalQuery
                .Include(query => query.Location)
                .Include(query => query.ResourcePositions)
                .ThenInclude(query => query.Resource);

        internal IQueryable<FloorPlan> AddSearchCriteria(FloorPlanSearchCriteria searchCriteria)
        {
            originalQuery = originalQuery.Where(item => !item.DeletedAt.HasValue);

            if (!string.IsNullOrWhiteSpace(searchCriteria.LocationId))
            {
                originalQuery = originalQuery.Where(item => !item.Location.DeletedAt.HasValue && item.Location.Id == searchCriteria.LocationId);
            }

            return originalQuery;
        }
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
        await DbContext.FloorPlan
            .AddSearchCriteria(searchCriteria)
            .AddDependentObjects()
            .ToPaginatedAsync(paginationInputParam, GetPaginationFields(orderByFields), cancellationToken);

    private static List<KeysetPaginationField<FloorPlan>> GetPaginationFields(ICollection<FloorPlanOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return
            [
                KeysetPaginationField<FloorPlan>.Create(
                    nameof(FloorPlan.Name),
                    query => query.Name,
                    OrderDirection.Ascending)
            ];
        }

        return orderByFields.Select(orderField => orderField.Field switch
            {
                FloorPlanOrderField.Name => KeysetPaginationField<FloorPlan>.Create(
                    nameof(FloorPlan.Name),
                    query => query.Name,
                    orderField.Direction),
                _ => throw new ArgumentOutOfRangeException()
            })
            .ToList();
    }
}
