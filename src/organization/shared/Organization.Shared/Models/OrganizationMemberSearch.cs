using Enterprise.Shared.Pagination;

namespace Organization.Shared.Models;

public record OrganizationMemberSearchCriteria(
    string? OrganizationId,
    string? OrganizationCustomDomain,
    string? NameContains,
    string? CustomerId);

public record OrganizationMemberOrder(OrderDirection Direction, OrganizationMemberOrderField Field);

public enum OrganizationMemberOrderField
{
    Role,
    Status,
    Name,
    GivenName,
    MiddleName,
    FamilyName,
    PhoneNumber
}
