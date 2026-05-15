using Resource = Location.Shared.Models.Resource;

namespace Location.Api.Models;

public record BulkAddResourceRow(
    string OrganizationResourceTypeTagId,
    string? BaseName,
    int Quantity,
    IReadOnlyList<string> CustomTagIds,
    IReadOnlyList<string> ZoneIds,
    IReadOnlyList<string> ProductTagIds);

public record BulkAddResources(
    string LocationId,
    IReadOnlyList<BulkAddResourceRow> Rows);

public record BulkAddRowResult(
    int RowIndex,
    IReadOnlyList<Resource> CreatedResources,
    string? FailureReason);
