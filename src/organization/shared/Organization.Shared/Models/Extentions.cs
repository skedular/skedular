namespace Organization.Shared.Models;

public static class Extension
{
    extension(Database.Entities.OrganizationStripeConnectAccount organizationStripeConnectAccount)
    {
        public bool IsAuthorized() =>
            organizationStripeConnectAccount is { OrganizationStripeConnectAccountAuthorization.IsAuthorized: true };

        public bool IsOnboardingCompleted() =>
            organizationStripeConnectAccount.IsAuthorized() && organizationStripeConnectAccount is
                { DetailsSubmitted: true, ChargesEnabled: true, PayoutsEnabled: true };
    }

    extension(OrganizationStripeConnectAccount organizationStripeConnectAccount)
    {
        public bool IsAuthorized() =>
            organizationStripeConnectAccount is { OrganizationStripeConnectAccountAuthorization.IsAuthorized: true };

        public bool IsOnboardingCompleted() =>
            organizationStripeConnectAccount.IsAuthorized() && organizationStripeConnectAccount is
                { DetailsSubmitted: true, ChargesEnabled: true, PayoutsEnabled: true };
    }
}
