using Api.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface ITermsOfUseRepository : IRepository<TermsOfUse>
{
    Task<TermsOfUse> GetActiveAsync(CancellationToken cancellationToken);
    Task<TermsOfUse> GetActiveUntrackedAsync(CancellationToken cancellationToken);
}

public class TermsOfUseRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, TermsOfUse>(dbContext, timeProvider), ITermsOfUseRepository
{
    /// <summary>
    ///     Returns the single active terms of use record.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the database query.</param>
    /// <returns>The single active terms of use record.</returns>
    /// <exception cref="ActiveTermsOfUseNotFoundException">Thrown when no active terms of use record exists.</exception>
    /// <exception cref="MultipleActiveTermsOfUseFoundException">Thrown when more than one active terms of use record exists.</exception>
    /// <remarks>
    ///     This repository-owned invariant check replaces the shared specification lookup so callers either receive one authoritative active terms record or
    ///     a clear domain failure.
    /// </remarks>
    public async Task<TermsOfUse> GetActiveAsync(CancellationToken cancellationToken)
    {
        var activeTermsOfUse = await DbContext.TermsOfUse
            .Where(query => !query.DeletedAt.HasValue && query.Active)
            .ToListAsync(cancellationToken);

        return activeTermsOfUse.Count switch
        {
            1 => activeTermsOfUse[0],
            0 => throw new ActiveTermsOfUseNotFoundException(),
            _ => throw new MultipleActiveTermsOfUseFoundException(),
        };
    }

    /// <summary>
    ///     Returns the single active terms of use record.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the database query.</param>
    /// <returns>The single active terms of use record.</returns>
    /// <exception cref="ActiveTermsOfUseNotFoundException">Thrown when no active terms of use record exists.</exception>
    /// <exception cref="MultipleActiveTermsOfUseFoundException">Thrown when more than one active terms of use record exists.</exception>
    /// <remarks>
    ///     This repository-owned invariant check replaces the shared specification lookup so callers either receive one authoritative active terms record or
    ///     a clear domain failure.
    /// </remarks>
    public async Task<TermsOfUse> GetActiveUntrackedAsync(CancellationToken cancellationToken)
    {
        var activeTermsOfUse = await DbContext.TermsOfUse
            .AsNoTracking()
            .Where(query => !query.DeletedAt.HasValue && query.Active)
            .ToListAsync(cancellationToken);

        return activeTermsOfUse.Count switch
        {
            1 => activeTermsOfUse[0],
            0 => throw new ActiveTermsOfUseNotFoundException(),
            _ => throw new MultipleActiveTermsOfUseFoundException(),
        };
    }
}
