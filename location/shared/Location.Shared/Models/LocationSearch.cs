using Api.Shared.Services.Models;
using Enterprise.Shared.Pagination;

namespace Location.Shared.Models;

public record LocationSearchCriteria(
    string? OrganizationId,
    string? OrganizationCustomDomain,
    IReadOnlyList<string> LocationIds,
    string? NameContains,
    IReadOnlyList<string> TagIds,
    string? CustomerId,
    IReadOnlyList<LocationType> Types,
    Polygon? SearchBoundaries,
    bool? NotContactedYet,
    OrganizationTagType? ResourceType,
    bool? FilterThoseWithUnverifiedOrganization,
    IReadOnlyList<string> ProductIds);

public record LocationOrder(OrderDirection Direction, LocationOrderField Field);

public enum LocationOrderField
{
    Name,
    Timezone,
    Type
}
