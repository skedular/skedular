namespace Organization.Shared.Models;

public static class Extension
{
    public static bool IsOnboardingCompleted(this Database.Entities.OrganizationStripeConnectAccount organizationStripeConnectAccount) =>
        organizationStripeConnectAccount is
            { DetailsSubmitted: true, OrganizationStripeConnectAccountAuthorization.IsAuthorized: true, ChargesEnabled: true, PayoutsEnabled: true };

    public static bool IsOnboardingCompleted(this OrganizationStripeConnectAccount organizationStripeConnectAccount) =>
        organizationStripeConnectAccount is
            { DetailsSubmitted: true, OrganizationStripeConnectAccountAuthorization.IsAuthorized: true, ChargesEnabled: true, PayoutsEnabled: true };
}
