using System.Reflection;
using Billing.Api.Mappers;
using Billing.Api.Services;
using Enterprise.Shared.Context;

namespace Billing.Api.GraphQL;

public class BillingQuery(IServiceProvider serviceProvider, IMapper mapper)
{
    public Task<Version> BillingVersionAsync(CancellationToken cancellationToken)
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return Task.FromResult(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }

    public async Task<bool> BillingCustomerRecordSyncedAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        return await service.DoesCustomerExistAsync(cancellationToken);
    }

    public Task<OrganizationCurrentOfferingChargesDetails[]?> OrganizationCurrentOfferingChargesAsync(
        string organizationId,
        CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public async Task<OrganizationBillingInfo?> OrganizationBillingInfoAsync(
        string organizationId,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var customerService = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        var customerExist = await customerService.DoesCustomerExistAsync(cancellationToken);
        if (!customerExist)
        {
            return null;
        }

        var organizationBillingService = scope.ServiceProvider.GetRequiredService<IOrganizationBillingService>();
        var organization = await organizationBillingService.GetBillingInfoById(organizationId, cancellationToken);
        return mapper.MapTo(organization);
    }
}
