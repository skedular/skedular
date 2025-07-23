using Enterprise.Shared.Context;
using Organization.Shared.Models;

namespace Organization.Api.Services.Authorization;

public interface IOrganizationSsoAuthorizationService
{
    bool IsSsoValid(Shared.Database.Entities.Organization organization, Customer customer);
}

public class OrganizationSsoAuthorizationService(IContext context) : IOrganizationSsoAuthorizationService
{
    public bool IsSsoValid(Shared.Database.Entities.Organization organization, Customer customer)
    {
        if (organization.OrganizationSsoSettings is null || !organization.OrganizationSsoSettings.IsActive)
        {
            return true;
        }

        var userSso = context.GetUserSsoContext(organization.Id);
        return userSso is not null && customer.Identities.Select(item => item.Email).Contains(userSso.Email);
    }
}
