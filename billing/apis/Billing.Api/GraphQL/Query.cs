using Billing.Api.Mappers;
using Billing.Api.Services;
using Enterprise.Shared.Version;
using HotChocolate;
using HotChocolate.Types;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Billing.Api.GraphQL;

[QueryType]
public class Query(IMapper mapper, IVersionService versionService)
{
    [UseResolverScope]
    public Version BillingVersion()
    {
        var version = versionService.GetVersion();

        return new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision };
    }

    [UseResolverScope]
    public async Task<bool> BillingCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) => await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationBillingContactDetails> OrganizationBillingContactDetailsAsync(
        string organizationId,
        [Service] IOrganizationBillingService organizationBillingService,
        CancellationToken cancellationToken)
    {
        var organization = await organizationBillingService.GetByOrganizationIdAsync(organizationId, cancellationToken);
        return mapper.MapTo(organization);
    }

    [UseResolverScope]
    public async Task<CustomerBillingContactDetails> MyBillingContactDetailsAsync(
        [Service] ICustomerBillingService customerBillingService,
        CancellationToken cancellationToken)
    {
        var customer = await customerBillingService.GetMyBillingContactAsync(cancellationToken);
        return mapper.MapTo(customer);
    }
}
