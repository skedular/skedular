using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Location.Api.Models;
using Location.Api.Services.Authorization;
using Location.Shared.Mappers;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Location.Shared.Services;
using Location.Shared.Services.Cache;
using Location.Shared.Workflows;
using OrganizationTagEntity = Location.Shared.Database.Entities.OrganizationTag;
using ResourceEntity = Location.Shared.Database.Entities.Resource;
using Resource = Location.Shared.Models.Resource;

namespace Location.Api.Services;

public interface IBulkAddResourcesService
{
    Task<IReadOnlyList<BulkAddRowResult>> ImportAsync(BulkAddResources input, CancellationToken cancellationToken);
}

public class BulkAddResourcesService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    IEntityMapper entityMapper,
    ILocationOutboxPublisher locationOutboxPublisher,
    ITemporalOutboxService temporalOutboxService,
    ILogger<BulkAddResourcesService> logger) : IBulkAddResourcesService
{
    private const int MaxTotalQuantity = 100;

    private const string FailureReasonQuantityLessThanOne = "Quantity must be at least 1.";
    private const string FailureReasonResourceTypeNotFound = "Resource type not found or invalid.";
    private const string FailureReasonInvalidTagIdentifiers = "One or more tag identifiers are invalid.";
    private const string FailureReasonOnlyOneResourceTypePerRow = "Only one resource type is allowed per row.";

    public async Task<IReadOnlyList<BulkAddRowResult>> ImportAsync(BulkAddResources input, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Bulk import batch received for location {LocationId}: {RowCount} rows",
            input.LocationId,
            input.Rows.Count);

        if (input.Rows.Count == 0)
        {
            throw new ArgumentException("Rows list must not be empty.");
        }

        var results = new List<BulkAddRowResult>();

        // Per-row quantity validation — collect failures without aborting other rows
        var validRows = new List<(int Index, BulkAddResourceRow Row)>();
        for (var i = 0; i < input.Rows.Count; i++)
        {
            var row = input.Rows[i];
            if (row.Quantity < 1)
            {
                logger.LogInformation("Row {RowIndex} rejected: quantity less than 1", i);
                results.Add(new BulkAddRowResult(i, [], FailureReasonQuantityLessThanOne));
            }
            else
            {
                validRows.Add((i, row));
            }
        }

        // Total quantity check uses only valid rows to prevent negative quantities from masking the limit
        var totalValidQuantity = validRows.Sum(x => x.Row.Quantity);
        if (totalValidQuantity > MaxTotalQuantity)
        {
            throw new ArgumentException($"Total quantity {totalValidQuantity} exceeds the maximum of {MaxTotalQuantity}.");
        }

        if (validRows.Count == 0)
        {
            return results;
        }

        // Fetch the location once (needed for org ID and outbox publish)
        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(input.LocationId, cancellationToken)
                               ?? throw new LocationNotFound();

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);

        if (!await organizationOfferingService.IsMoreInteractionAllowedAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (!await organizationAuthorizationService.CanModifyAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        // Validate resource-type tags for all valid rows in one repository call
        var allTypeTagIds = validRows.Select(x => x.Row.OrganizationResourceTypeTagId).Distinct().ToList();
        var foundTypeTags = await repositoryFactory.OrganizationTagRepository.GetActiveByIdsForOrganizationAsync(
            allTypeTagIds,
            existingLocation.OrganizationId,
            null,
            cancellationToken);

        var typeTagIndex = foundTypeTags.ToDictionary(t => t.Id);

        var rowsWithValidTags = new List<(int Index, BulkAddResourceRow Row, string TypeTagName)>();
        foreach (var (idx, row) in validRows)
        {
            if (!typeTagIndex.TryGetValue(row.OrganizationResourceTypeTagId, out var typeTag) ||
                typeTag.Type is null || OrganizationTagTypeConstants.ResourceTypes.All(rt => rt != typeTag.Type!.ToOrganizationTagType()))
            {
                logger.LogInformation("Row {RowIndex} rejected: invalid resource-type tag {TagId}", idx, row.OrganizationResourceTypeTagId);
                results.Add(new BulkAddRowResult(idx, [], FailureReasonResourceTypeNotFound));
            }
            else
            {
                rowsWithValidTags.Add((idx, row, typeTag.Name ?? row.OrganizationResourceTypeTagId));
            }
        }

        if (rowsWithValidTags.Count == 0)
        {
            return results;
        }

        // Pre-load all existing names for this location once
        var existingNames = await repositoryFactory.ResourceRepository.GetActiveNamesByLocationIdAsync(
            input.LocationId,
            cancellationToken);

        // Allocated-name set starts with existing names; grows as we assign names within this batch
        var allocatedNames = new HashSet<string>(existingNames, StringComparer.OrdinalIgnoreCase);

        // Compute next suffix per base name (always-append strategy)
        var maxSuffixByBase = ComputeMaxSuffixes(existingNames);

        // Collect all rows with their validated non-type tag sets
        var allNonTypeTagIds = rowsWithValidTags
            .SelectMany(x => x.Row.CustomTagIds.Concat(x.Row.ZoneIds).Concat(x.Row.ProductTagIds))
            .Distinct()
            .ToList();

        var foundNonTypeTags = allNonTypeTagIds.Count > 0
            ? await repositoryFactory.OrganizationTagRepository.GetActiveByIdsForOrganizationAsync(
                allNonTypeTagIds,
                existingLocation.OrganizationId,
                null,
                cancellationToken)
            : (IReadOnlyList<OrganizationTagEntity>)[];

        var nonTypeTagIndex = foundNonTypeTags.ToDictionary(t => t.Id);

        // Single transaction for all successful rows
        var rowsToCreate = new List<(int RowIdx, List<ResourceEntity> Entities)>();

        foreach (var (idx, row, typeTagName) in rowsWithValidTags)
        {
            var requestedNonTypeTagIds = row.CustomTagIds.Concat(row.ZoneIds).Concat(row.ProductTagIds).ToList();
            var resolvedNonTypeTags = requestedNonTypeTagIds
                .Where(nonTypeTagIndex.ContainsKey)
                .Select(id => nonTypeTagIndex[id])
                .ToList();

            if (requestedNonTypeTagIds.Count != resolvedNonTypeTags.Count)
            {
                logger.LogInformation("Row {RowIndex} rejected: one or more tag identifiers are invalid", idx);
                results.Add(new BulkAddRowResult(idx, [], FailureReasonInvalidTagIdentifiers));
                continue;
            }

            // Ensure none of the custom/zone/product tag IDs is itself a resource-type tag
            var hasResourceTypeInNonTypeTags = resolvedNonTypeTags
                .Any(t => !string.IsNullOrWhiteSpace(t.Type) &&
                          OrganizationTagTypeConstants.ResourceTypes.Any(rt => rt == t.Type!.ToOrganizationTagType()));

            if (hasResourceTypeInNonTypeTags)
            {
                logger.LogInformation("Row {RowIndex} rejected: non-type tag IDs contain a resource-type tag", idx);
                results.Add(new BulkAddRowResult(idx, [], FailureReasonOnlyOneResourceTypePerRow));
                continue;
            }

            var effectiveBaseName = string.IsNullOrWhiteSpace(row.BaseName) ? typeTagName : row.BaseName;
            var rowEntities = new List<ResourceEntity>(row.Quantity);

            // Build resources for this row
            maxSuffixByBase.TryAdd(effectiveBaseName, 0);

            for (var n = 0; n < row.Quantity; n++)
            {
                maxSuffixByBase[effectiveBaseName]++;
                var candidateName = $"{effectiveBaseName}-{maxSuffixByBase[effectiveBaseName]}";

                // Skip any suffix that is already taken (can happen if gaps exist; we just advance further)
                while (allocatedNames.Contains(candidateName))
                {
                    maxSuffixByBase[effectiveBaseName]++;
                    candidateName = $"{effectiveBaseName}-{maxSuffixByBase[effectiveBaseName]}";
                }

                allocatedNames.Add(candidateName);

                var resourceModel = new Resource
                {
                    Id = randomHelper.Generate(),
                    Name = candidateName,
                    Inactive = false,
                    RequireBookingApproval = false,
                    Capacity = 1,
                    Location = entityMapper.MapTo(existingLocation)
                };

                var allTagEntities = new List<OrganizationTagEntity> { typeTagIndex[row.OrganizationResourceTypeTagId] };
                allTagEntities.AddRange(resolvedNonTypeTags);

                rowEntities.Add(entityMapper.MapTo(resourceModel, existingLocation, allTagEntities));
            }

            rowsToCreate.Add((idx, rowEntities));
        }

        if (rowsToCreate.Count == 0)
        {
            return results;
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var createdByRow = new Dictionary<int, List<Resource>>();

        foreach (var (rowIdx, entities) in rowsToCreate)
        {
            var createdModels = new List<Resource>(entities.Count);
            createdModels
                .AddRange(entities
                    .Select(entity => repositoryFactory.ResourceRepository.Add(entity))
                    .Select(added => entityMapper.MapTo(added, entityMapper.MapTo(existingLocation))));

            createdByRow[rowIdx] = createdModels;
        }

        locationOutboxPublisher.PublishLocations([entityMapper.MapTo(existingLocation)], repositoryFactory.UnitOfWork);

        temporalOutboxService.StartComputeOrganizationLocationsAndProductsRelationships(
            new ComputeOrganizationLocationsAndProductsRelationshipsInput(existingLocation.OrganizationId),
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var created = createdByRow.Values.Sum(x => x.Count);
        var rejected = results.Count;

        logger.LogInformation(
            "Bulk import completed for location {LocationId}: {Created} resources created, {Rejected} rows rejected",
            input.LocationId,
            created,
            rejected);

        foreach (var (rowIdx, createdModels) in createdByRow)
        {
            results.Add(new BulkAddRowResult(rowIdx, createdModels, null));
        }

        return results.OrderBy(r => r.RowIndex).ToList();
    }

    /// <summary>
    ///     Scans existing resource names and extracts the maximum numeric suffix per base name.
    ///     E.g., ["Desk-1", "Desk-3", "Room-2"] → {"Desk": 3, "Room": 2}.
    /// </summary>
    private static Dictionary<string, int> ComputeMaxSuffixes(IReadOnlyList<string> existingNames)
    {
        var maxSuffixByBase = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var name in existingNames)
        {
            var lastDash = name.LastIndexOf('-');
            if (lastDash < 1)
            {
                continue;
            }

            var basePart = name[..lastDash];
            var suffixPart = name[(lastDash + 1)..];

            if (int.TryParse(suffixPart, out var suffix))
            {
                if (!maxSuffixByBase.TryGetValue(basePart, out var current) || suffix > current)
                {
                    maxSuffixByBase[basePart] = suffix;
                }
            }
        }

        return maxSuffixByBase;
    }
}
