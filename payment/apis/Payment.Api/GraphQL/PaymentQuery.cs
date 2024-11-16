using System.Reflection;
using HotChocolate;
using HotChocolate.Types;
using Payment.Api.Mappers;
using Payment.Api.Services;

namespace Payment.Api.GraphQL;

public class PaymentQuery
{
    [UseServiceScope]
    public Version PaymentVersion()
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

    [UseServiceScope]
    public async Task<bool> PaymentCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);

    [UseServiceScope]
    public async Task<OrganizationPaymentMethod[]?> OrganizationPaymentMethodsDetailsAsync(
        string organizationId,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IOrganizationService organizationService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var paymentMethods =
            await organizationService.GetOrganizationPaymentMethodsAsync(organizationId, cancellationToken);
        return mapper.MapTo(paymentMethods).ToArray();
    }
}
