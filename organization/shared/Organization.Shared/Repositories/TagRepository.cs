using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization.Shared.Database;
using Organization.Shared.Models;
using Tag = Organization.Shared.Database.Entities.Tag;

namespace Organization.Shared.Repositories;

public interface ITagRepository : IRepository<Tag>
{
    Task<Tag?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Tag>> GetByIdsAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken);

    Task<bool> ExistsActiveWithNameAsync(
        string organizationId,
        string type,
        string name,
        string? excludeId,
        CancellationToken cancellationToken);

    Tag Add(Tag tag);
    Tag Update(Tag tag);
    void RemoveRange(IReadOnlyList<Tag> tags);
    Tag Remove(Tag tag);

    Task<(PaginatedInfo, IReadOnlyList<Edge<Tag>>, int)> GetPaginatedTagsAsync(
        PaginationInputParam paginationInputParam,
        TagSearchCriteria searchCriteria,
        IReadOnlyList<TagOrder> orderByFields,
        CancellationToken cancellationToken);
}

public static class TagExtensions
{
    extension(IQueryable<Tag> originalQuery)
    {
        public IIncludableQueryable<Tag, Database.Entities.Organization> AddDependentObjects() =>
            originalQuery
                .AsSingleQuery()
                .Include(query => query.Organization);

        public IQueryable<Tag> AddSearchCriteria(TagSearchCriteria searchCriteria)
        {
            if (!string.IsNullOrWhiteSpace(searchCriteria.OrganizationId))
            {
                originalQuery = originalQuery.Where(item => !item.DeletedAt.HasValue && item.Organization.Id == searchCriteria.OrganizationId);
            }

            if (!string.IsNullOrWhiteSpace(searchCriteria.OrganizationCustomDomain))
            {
                originalQuery = originalQuery.Where(item =>
                    !item.DeletedAt.HasValue && item.Organization.CustomDomain != null &&
                    item.Organization.CustomDomain == searchCriteria.OrganizationCustomDomain);
            }

            if (searchCriteria.Types.Count != 0)
            {
                originalQuery = originalQuery.Where(item => searchCriteria.Types.Contains(item.Type));
            }

            if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
            {
                originalQuery = originalQuery.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
            }

            return originalQuery;
        }
    }
}

public class TagRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, Tag>(dbContext, timeProvider), ITagRepository
{
    private const string LikeEscapeCharacter = "\\";

    public async Task<Tag?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Tag.AddDependentObjects().FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Tag>> GetByIdsAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken) =>
        await DbContext.Tag.Where(query => ids.Contains(query.Id)).AddDependentObjects().ToListAsync(cancellationToken);

    /// <summary>
    ///     Checks whether an active organization tag already exists with the supplied name and type for an organization.
    /// </summary>
    /// <param name="organizationId">The organization identifier that owns the tag name namespace.</param>
    /// <param name="type">The tag type that must also match.</param>
    /// <param name="name">The candidate tag name to validate.</param>
    /// <param name="excludeId">An optional tag identifier to exclude from the duplicate check, typically the tag currently being updated.</param>
    /// <param name="cancellationToken">The cancellation token for the database query.</param>
    /// <returns><see langword="true" /> when another active tag with the same effective name and type exists; otherwise <see langword="false" />.</returns>
    /// <remarks>
    ///     This exact-name check stays in the repository so tag create and update flows can reuse the same duplicate rule without reviving the old
    ///     specification abstraction.
    /// </remarks>
    public async Task<bool> ExistsActiveWithNameAsync(
        string organizationId,
        string type,
        string name,
        string? excludeId,
        CancellationToken cancellationToken) =>
        await DbContext.Tag.AnyAsync(
            query =>
                !query.DeletedAt.HasValue &&
                query.Organization.Id == organizationId &&
                query.Type == type &&
                EF.Functions.ILike(query.Name, EscapeLikePattern(name), LikeEscapeCharacter) &&
                (excludeId == null || query.Id != excludeId),
            cancellationToken);

    public Tag Add(Tag tag)
    {
        var now = TimeProvider.GetUtcNow();
        tag.CreatedAt = now;
        return DbContext.Tag.Add(tag).Entity;
    }

    public void RemoveRange(IReadOnlyList<Tag> tags)
    {
        var now = TimeProvider.GetUtcNow();
        tags.ForEach(tag => tag.DeletedAt = now);
        DbContext.Tag.UpdateRange(tags);
    }

    public Tag Remove(Tag tag)
    {
        var now = TimeProvider.GetUtcNow();
        tag.DeletedAt = now;
        return DbContext.Tag.Update(tag).Entity;
    }

    public Tag Update(Tag tag)
    {
        var now = TimeProvider.GetUtcNow();
        tag.ModifiedAt = now;
        return DbContext.Tag.Update(tag).Entity;
    }

    public async Task<(PaginatedInfo, IReadOnlyList<Edge<Tag>>, int)> GetPaginatedTagsAsync(
        PaginationInputParam paginationInputParam,
        TagSearchCriteria searchCriteria,
        IReadOnlyList<TagOrder> orderByFields,
        CancellationToken cancellationToken) =>
        await DbContext.Tag
            .AddSearchCriteria(searchCriteria)
            .AddDependentObjects()
            .ToPaginatedAsync(paginationInputParam, GetPaginationFields(orderByFields), cancellationToken);

    private static List<KeysetPaginationField<Tag>> GetPaginationFields(IReadOnlyList<TagOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return
            [
                KeysetPaginationField<Tag>.Create(
                    nameof(Tag.Name),
                    query => query.Name,
                    OrderDirection.Ascending)
            ];
        }

        return orderByFields.Select(orderField => orderField.Field switch
            {
                OrganizationTagOrderField.Name => KeysetPaginationField<Tag>.Create(
                    nameof(Tag.Name),
                    query => query.Name,
                    orderField.Direction),
                OrganizationTagOrderField.Description => KeysetPaginationField<Tag>.Create(
                    nameof(Tag.Description),
                    query => query.Description,
                    orderField.Direction),
                OrganizationTagOrderField.Type => KeysetPaginationField<Tag>.Create(
                    nameof(Tag.Type),
                    query => query.Type,
                    orderField.Direction),
                _ => throw new ArgumentOutOfRangeException()
            })
            .ToList();
    }

    /// <summary>
    ///     Escapes SQL LIKE wildcard characters in a user-supplied name so an exact-name comparison can safely use <c>ILIKE</c>.
    /// </summary>
    /// <param name="value">The raw user-supplied value that may contain wildcard characters.</param>
    /// <returns>The escaped value safe to pass into an exact-match <c>ILIKE</c> predicate.</returns>
    private static string EscapeLikePattern(string value) =>
        value
            .Replace(LikeEscapeCharacter, LikeEscapeCharacter + LikeEscapeCharacter, StringComparison.Ordinal)
            .Replace("%", LikeEscapeCharacter + "%", StringComparison.Ordinal)
            .Replace("_", LikeEscapeCharacter + "_", StringComparison.Ordinal);
}
