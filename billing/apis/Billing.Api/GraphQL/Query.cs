using System.Reflection;
using Billing.Api.Mappers;
using Billing.Api.Services;
using HotChocolate;
using HotChocolate.Types;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Billing.Api.GraphQL;

[QueryType]
public class Query(IMapper mapper)
{
    [UseResolverScope]
    public Version BillingVersion()
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision };
    }

    [UseResolverScope]
    public async Task<bool> BillingCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) => await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationBillingInfo?> OrganizationBillingInfoAsync(
        string organizationId,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IOrganizationBillingService organizationBillingService,
        CancellationToken cancellationToken)
    {
        var customerExist = await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);
        if (!customerExist)
        {
            return null;
        }

        var organization = await organizationBillingService.GetBillingInfoById(organizationId, cancellationToken);
        return mapper.MapTo(organization);
    }
}
