using Enterprise.Shared.Pagination;

namespace Payment.Shared.Models;

public class OrganizationStripeConnectAccountSearchCriteria(string organizationId, string? nameContains)
{
    public string OrganizationId { get; } = organizationId;
    public string? NameContains { get; } = nameContains;
}

public record OrganizationStripeConnectAccountOrder(OrderDirection Direction, OrganizationStripeConnectAccountOrderField Field);

public enum OrganizationStripeConnectAccountOrderField
{
    Name
}
