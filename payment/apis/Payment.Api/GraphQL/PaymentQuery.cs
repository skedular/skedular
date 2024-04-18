using System.Reflection;
using Api.Shared.Services.GraphQL.UnityHub.V1.Payment;
using Enterprise.Shared.Context;
using Payment.Api.Mappers;
using Payment.Api.Services;
using Version = Api.Shared.Services.GraphQL.UnityHub.V1.Payment.Version;

namespace Payment.Api.GraphQL;

public class PaymentQuery(IMapper mapper) : Query
{
    public override Task<Version> PaymentVersionAsync(
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

    public override async Task<bool> PaymentCustomerRecordSyncedAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerService>();
        return await service.DoesCustomerExistAsync(cancellationToken);
    }

    public override async Task<OrganizationPaymentMethod[]> OrganizationPaymentMethodsDetailsAsync(
        string organizationId,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerService>();
        if (!await service.DoesCustomerExistAsync(cancellationToken))
        {
            return [];
        }

        var organizationService = scope.ServiceProvider.GetRequiredService<IOrganizationService>();
        return mapper
            .MapTo(await organizationService.GetOrganizationPaymentMethodsAsync(organizationId, cancellationToken))
            .ToArray();
    }
}
