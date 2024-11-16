using System.Reflection;
using Billing.Api.Mappers;
using Billing.Api.Services;
using HotChocolate;

namespace Billing.Api.GraphQL;

public class BillingQuery
{
    public Version BillingVersion()
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        };
    }

    public async Task<bool> BillingCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) => await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);

    public Task<OrganizationCurrentOfferingChargesDetails[]?> OrganizationCurrentOfferingChargesAsync(
        string organizationId,
        CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public async Task<OrganizationBillingInfo?> OrganizationBillingInfoAsync(
        string organizationId,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IOrganizationBillingService organizationBillingService,
        [Service] IMapper mapper,
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
