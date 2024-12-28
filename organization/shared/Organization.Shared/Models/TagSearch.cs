using Enterprise.Shared.Pagination;

namespace Organization.Shared.Models;

public class TagSearchCriteria
{
    public TagSearchCriteria(string organizationId, string? type, string? nameContains)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);

        OrganizationId = organizationId;
        Type = type;
        NameContains = nameContains;
    }

    public string OrganizationId { get; }
    public string? Type { get; }
    public string? NameContains { get; }
}

public record TagOrder(OrderDirection Direction, OrganizationTagOrderField Field);

public enum OrganizationTagOrderField
{
    Name,
    Description,
    TagType
}
