using System.Reflection;
using HotChocolate;
using HotChocolate.Types;
using Payment.Api.Mappers;
using Payment.Api.Services;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Payment.Api.GraphQL;

[QueryType]
public class Query(IMapper mapper)
{
    [UseResolverScope]
    public Version PaymentVersion()
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision };
    }

    [UseResolverScope]
    public async Task<bool> PaymentCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationPaymentMethod[]?> OrganizationPaymentMethodsDetailsAsync(
        string organizationId,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var paymentMethods = await organizationService.GetOrganizationPaymentMethodsAsync(organizationId, cancellationToken);
        return mapper.MapTo(paymentMethods).ToArray();
    }
}
