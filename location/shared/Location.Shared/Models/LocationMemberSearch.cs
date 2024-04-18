using Enterprise.Shared.Pagination;

namespace Location.Shared.Models;

public class LocationMemberSearchCriteria
{
    public LocationMemberSearchCriteria(string locationId, string? nameContains)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        LocationId = locationId;
        NameContains = nameContains;
    }

    public string LocationId { get; }
    public string? NameContains { get; }
}

public record LocationMemberOrder(OrderDirection Direction, LocationMemberOrderField Field);

public enum LocationMemberOrderField
{
    MembershipType,
    Name,
    GivenName,
    MiddleName,
    FamilyName
}
