using Enterprise.Shared.Pagination;

namespace Organization.Shared.Models;

public class ResourceTypeSearchCriteria
{
    public ResourceTypeSearchCriteria(string organizationId, string? nameContains)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);

        OrganizationId = organizationId;
        NameContains = nameContains;
    }

    public string OrganizationId { get; }
    public string? NameContains { get; }
}

public record ResourceTypeOrder(OrderDirection Direction, OrganizationResourceTypeOrderField Field);

public enum OrganizationResourceTypeOrderField
{
    Name,
    Description
}
