using System.Reflection;
using Enterprise.Shared.Context;
using Payment.Api.Mappers;
using Payment.Api.Services;

namespace Payment.Api.GraphQL;

public class PaymentQuery(IServiceProvider serviceProvider, IMapper mapper)
{
    public Task<Version> PaymentVersionAsync(CancellationToken cancellationToken)
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

    public async Task<bool> PaymentCustomerRecordSyncedAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        return await service.DoesCustomerExistAsync(cancellationToken);
    }

    public async Task<OrganizationPaymentMethod[]?> OrganizationPaymentMethodsDetailsAsync(
        string organizationId,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var cachedCustomerService = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var service = scope.ServiceProvider.GetRequiredService<IOrganizationService>();
        var paymentMethods = await service.GetOrganizationPaymentMethodsAsync(organizationId, cancellationToken);
        return mapper.MapTo(paymentMethods).ToArray();
    }
}
