namespace Organization.Shared.Models;

public static class Extension
{
    public static bool IsAuthorized(this Database.Entities.OrganizationStripeConnectAccount organizationStripeConnectAccount) =>
        organizationStripeConnectAccount is { OrganizationStripeConnectAccountAuthorization.IsAuthorized: true };

    public static bool IsAuthorized(this OrganizationStripeConnectAccount organizationStripeConnectAccount) =>
        organizationStripeConnectAccount is { OrganizationStripeConnectAccountAuthorization.IsAuthorized: true };

    public static bool IsOnboardingCompleted(this Database.Entities.OrganizationStripeConnectAccount organizationStripeConnectAccount) =>
        organizationStripeConnectAccount.IsAuthorized() && organizationStripeConnectAccount is
            { DetailsSubmitted: true, ChargesEnabled: true, PayoutsEnabled: true };

    public static bool IsOnboardingCompleted(this OrganizationStripeConnectAccount organizationStripeConnectAccount) =>
        organizationStripeConnectAccount.IsAuthorized() && organizationStripeConnectAccount is
            { DetailsSubmitted: true, ChargesEnabled: true, PayoutsEnabled: true };
}
