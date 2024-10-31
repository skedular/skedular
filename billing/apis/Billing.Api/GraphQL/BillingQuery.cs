using System.Reflection;
using Api.Shared.Services.GraphQL.UnityHub.V1.Billing;
using Billing.Api.Mappers;
using Billing.Api.Services;
using Enterprise.Shared.Context;
using Version = Api.Shared.Services.GraphQL.UnityHub.V1.Billing.Version;

namespace Billing.Api.GraphQL;

public class BillingQuery(IMapper mapper) : Query
{
    public override Task<Version> BillingVersionAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
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

    public override async Task<bool> BillingCustomerRecordSyncedAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        return await service.DoesCustomerExistAsync(cancellationToken);
    }

    public override Task<OrganizationCurrentOfferingChargesDetails[]?> OrganizationCurrentOfferingChargesAsync(
        string organizationId, IServiceProvider serviceProvider,
        CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public override async Task<OrganizationBillingInfo?> OrganizationBillingInfoAsync(
        string organizationId,
        IServiceProvider serviceProvider,
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
