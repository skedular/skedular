using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Azure;

[QueryType]
public class Query(IMapper mapper)
{
    [UseResolverScope]
    public async Task<bool> IsAzureTenantInstalledAsync([Service] IAzureTenantService azureTenantService, CancellationToken cancellationToken) =>
        await azureTenantService.DoesTenantExistAsync(cancellationToken);

    [UseResolverScope]
    public async Task<string> AzureTenantAdminConsentUrlAsync(
        [Service] IAzureTenantService azureTenantService,
        CancellationToken cancellationToken) =>
        await azureTenantService.GenerateAdminConsentUrlAsync(cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationDetails?> AzureTenantOrganizationAsync(
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken) =>
        mapper.MapTo(await organizationService.GetByAzureTenantAsync(cancellationToken));
}
