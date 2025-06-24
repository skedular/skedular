using Enterprise.Shared.Pagination;

namespace Organization.Shared.Models;

public class OrganizationStripeConnectAccountSearchCriteria(string organizationId, string? nameContains, bool? onboardingCompleted)
{
    public string OrganizationId { get; } = organizationId;
    public string? NameContains { get; } = nameContains;
    public bool? OnboardingCompleted { get; set; } = onboardingCompleted;
}

public record OrganizationStripeConnectAccountOrder(OrderDirection Direction, OrganizationStripeConnectAccountOrderField Field);

public enum OrganizationStripeConnectAccountOrderField
{
    Name
}
