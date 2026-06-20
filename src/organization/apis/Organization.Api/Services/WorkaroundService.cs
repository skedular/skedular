using Enterprise.Shared.Database;
using Organization.Api.Mappers;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services;
using Organization.Shared.Workflows;

namespace Organization.Api.Services;

public interface IWorkaroundService
{
    Task RepublishOrganizationAsync(string customDomain, CancellationToken cancellationToken);
    Task RepublishAllOrganizationsAsync(CancellationToken cancellationToken);
    Task ReSyncAzureTenantAsync(string tenantId, CancellationToken cancellationToken);
    Task ReSyncAllAzureTenantsAsync(CancellationToken cancellationToken);
    Task RegenerateAllDailyAnalyticsAsync(CancellationToken cancellationToken);
    Task RegenerateDailyAnalyticsAsync(string customDomain, CancellationToken cancellationToken);
}

public class WorkaroundService(
    IRepositoryFactory repositoryFactory,
    IGraphQlMapper graphQlMapper,
    IOrganizationPublisher organizationPublisher,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    ITemporalService temporalService)
    : IWorkaroundService
{
    public async Task RepublishOrganizationAsync(string customDomain, CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(null, customDomain, cancellationToken);
        if (organization is null)
        {
            return;
        }

        await organizationPublisher.PublishOrganizationsAsync(
            [graphQlMapper.MapTo(organization, organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))],
            cancellationToken);
    }

    public async Task RepublishAllOrganizationsAsync(CancellationToken cancellationToken)
    {
        var organizations = await repositoryFactory.OrganizationRepository.GetAllUntrackedAsync(cancellationToken);
        await organizationPublisher.PublishOrganizationsAsync(
            organizations.Select(item =>
                graphQlMapper.MapTo(item, organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(item.Id))).ToList(),
            cancellationToken);
    }

    public async Task ReSyncAzureTenantAsync(string tenantId, CancellationToken cancellationToken)
    {
        var tenant = await repositoryFactory.AzureTenantRepository.GetByIdAsync(tenantId, cancellationToken);
        if (tenant is null || tenant.IsDeleted())
        {
            return;
        }

        await temporalService.StartWorkflowReSyncAzureTenantAsync(new ReSyncAzureTenantInput(tenant.Id, null), cancellationToken);
    }

    public async Task ReSyncAllAzureTenantsAsync(CancellationToken cancellationToken)
    {
        var tenants = await repositoryFactory.AzureTenantRepository.GetAllAsync(cancellationToken);

        foreach (var tenant in tenants)
        {
            await temporalService.StartWorkflowReSyncAzureTenantAsync(new ReSyncAzureTenantInput(tenant.Id, null), cancellationToken);
        }
    }

    public async Task RegenerateAllDailyAnalyticsAsync(CancellationToken cancellationToken)
    {
        var organizations = await repositoryFactory.OrganizationRepository.GetAllUntrackedAsync(cancellationToken);

        foreach (var organization in organizations)
        {
            await temporalService.StartWorkflowGenerateOrganizationDailyAnalyticsAsync(
                new GenerateOrganizationDailyAnalyticsInput(organization.Id, null),
                cancellationToken);
        }
    }

    public async Task RegenerateDailyAnalyticsAsync(string customDomain, CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(null, customDomain, cancellationToken);
        if (organization is null || organization.IsDeleted())
        {
            return;
        }

        await temporalService.StartWorkflowGenerateOrganizationDailyAnalyticsAsync(
            new GenerateOrganizationDailyAnalyticsInput(organization.Id, null),
            cancellationToken);
    }
}
