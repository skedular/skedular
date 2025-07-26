using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Sso;

[QueryType]
public class RootQuery
{
    [UseResolverScope]
    public async Task<bool> IsOrganizationSsoTokenValidAsync(
        string id,
        [Service] IOrganizationSsoService organizationSsoService,
        CancellationToken cancellationToken) =>
        await organizationSsoService.IsOrganizationSsoTokenValidAsync(id, cancellationToken);


    [UseResolverScope]
    public async Task<string> SsoLoginUrlAsync(
        string id,
        string redirectUrl,
        [Service] IOrganizationSsoService organizationSsoService,
        CancellationToken cancellationToken) =>
        await organizationSsoService.SsoLoginAsync(id, redirectUrl, cancellationToken);
}
