using Enterprise.Shared.Pagination;

namespace Organization.Shared.Models;

public record OrganizationBankAccountSearchCriteria(string? OrganizationId, string? OrganizationCustomDomain, string? NameContains);

public record OrganizationBankAccountOrder(OrderDirection Direction, OrganizationBankAccountOrderField Field);

public enum OrganizationBankAccountOrderField
{
    Name,
}
