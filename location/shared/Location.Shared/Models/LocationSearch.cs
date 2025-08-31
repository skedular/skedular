using Api.Shared.Services.Models;
using Enterprise.Shared.Pagination;

namespace Location.Shared.Models;

public record LocationSearchCriteria(
    string? OrganizationId,
    string? OrganizationUniqueAlphanumericName,
    ICollection<string> LocationIds,
    string? NameContains,
    ICollection<string> TagIds,
    string? CustomerId,
    ICollection<LocationType> Types,
    Polygon? SearchBoundaries);

public record LocationOrder(OrderDirection Direction, LocationOrderField Field);

public enum LocationOrderField
{
    Name,
    About,
    Timezone,
    Type
}
