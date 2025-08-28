using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Sso;

[QueryType]
public class RootQuery
{
    [UseResolverScope]
    public async Task<bool> IsOrganizationSsoTokenValidAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        [Service] IOrganizationSsoService organizationSsoService,
        CancellationToken cancellationToken) =>
        await organizationSsoService.IsOrganizationSsoTokenValidAsync(organizationId, organizationUniqueAlphanumericName, cancellationToken);


    [UseResolverScope]
    public async Task<string> OrganizationSsoLoginUrlAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        string redirectUrl,
        [Service] IOrganizationSsoService organizationSsoService,
        CancellationToken cancellationToken) =>
        await organizationSsoService.SsoLoginAsync(organizationId, organizationUniqueAlphanumericName, redirectUrl, cancellationToken);
}
