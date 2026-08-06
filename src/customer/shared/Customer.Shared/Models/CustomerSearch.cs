using Enterprise.Shared.Pagination;

namespace Customer.Shared.Models;

public record CustomerSearchCriteria(string? NameContains, string? LocationId);

public record CustomerOrder(OrderDirection Direction, CustomerOrderField Field);

public enum CustomerOrderField
{
    Designation,
    Title,
    Name,
    GivenName,
    MiddleName,
    FamilyName,
    Timezone,
    Locale,
    PhoneNumber,
}
