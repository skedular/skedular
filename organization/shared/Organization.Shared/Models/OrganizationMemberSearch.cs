using Enterprise.Shared.Pagination;

namespace Organization.Shared.Models;

public class OrganizationMemberSearchCriteria
{
    public OrganizationMemberSearchCriteria(string organizationId, string? nameContains, string? customerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);

        OrganizationId = organizationId;
        NameContains = nameContains;
        CustomerId = customerId;
    }

    public string OrganizationId { get; }
    public string? NameContains { get; }
    public string? CustomerId { get; }
}

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
