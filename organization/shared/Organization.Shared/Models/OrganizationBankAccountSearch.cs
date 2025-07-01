using Enterprise.Shared.Pagination;

namespace Organization.Shared.Models;

public class OrganizationBankAccountSearchCriteria(string organizationId, string? nameContains)
{
    public string OrganizationId { get; } = organizationId;
    public string? NameContains { get; } = nameContains;
}

public record OrganizationBankAccountOrder(OrderDirection Direction, OrganizationBankAccountOrderField Field);

public enum OrganizationBankAccountOrderField
{
    Name
}
