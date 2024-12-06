using Enterprise.Shared.Pagination;

namespace Organization.Shared.Models;

public class OrganizationMemberSearchCriteria
{
    public OrganizationMemberSearchCriteria(string organizationId, string? nameContains)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);

        OrganizationId = organizationId;
        NameContains = nameContains;
    }

    public string OrganizationId { get; }
    public string? NameContains { get; }
}

public record OrganizationMemberOrder(OrderDirection Direction, OrganizationMemberOrderField Field);

public enum OrganizationMemberOrderField
{
    MembershipType,
    Name,
    GivenName,
    MiddleName,
    FamilyName,
    PhoneNumber
}
