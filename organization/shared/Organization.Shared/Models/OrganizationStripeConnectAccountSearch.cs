using Enterprise.Shared.Pagination;

namespace Organization.Shared.Models;

public record OrganizationStripeConnectAccountSearchCriteria(
    string? OrganizationId,
    string? OrganizationUniqueAlphanumericName,
    string? NameContains,
    bool? OnboardingCompleted);

public record OrganizationStripeConnectAccountOrder(OrderDirection Direction, OrganizationStripeConnectAccountOrderField Field);

public enum OrganizationStripeConnectAccountOrderField
{
    Name
}
