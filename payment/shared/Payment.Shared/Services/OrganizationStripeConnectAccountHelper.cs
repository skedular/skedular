using Enterprise.Shared.Exceptions;
using Payment.Shared.Database.Entities;

namespace Payment.Shared.Services;

public interface IOrganizationStripeConnectAccountHelper
{
    StripeConnectAccount GetStripeAccount(Organization organization);
}

public class OrganizationStripeConnectAccountHelper : IOrganizationStripeConnectAccountHelper
{
    public StripeConnectAccount GetStripeAccount(Organization organization)
    {
        // TODO: 20250530 - Morteza: Need to implement the default Stripe Connect account and pick that instead of the first random one 
        var account = organization.StripeConnectAccounts.OrderByDescending(item => item.CreatedAt).FirstOrDefault(item => item.DeletedAt is null);
        if (account is null)
        {
            throw new NoStripeConnectAccountFoundForOrganization();
        }

        return account;
    }
}
