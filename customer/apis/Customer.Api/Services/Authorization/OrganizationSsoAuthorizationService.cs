using Customer.Shared.Database.Entities;
using Enterprise.Shared.Context;

namespace Customer.Api.Services.Authorization;

public interface IOrganizationSsoAuthorizationService
{
    bool IsSsoValid(Organization organization, Shared.Database.Entities.Customer customer);
}

public class OrganizationSsoAuthorizationService(IContext context) : IOrganizationSsoAuthorizationService
{
    public bool IsSsoValid(Organization organization, Shared.Database.Entities.Customer customer)
    {
        if (organization.OrganizationSsoSettings is null || !organization.OrganizationSsoSettings.IsActive)
        {
            return true;
        }

        var userSso = context.GetUserSsoContext(organization.Id);
        return userSso is not null && customer.Identities.Select(item => item.Email).Contains(userSso.Email);
    }
}
