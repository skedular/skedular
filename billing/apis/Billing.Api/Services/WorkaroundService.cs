using Billing.Api.Mappers;
using Billing.Shared.Publishers;
using Billing.Shared.Repositories;

namespace Billing.Api.Services;

public interface IWorkaroundService
{
    Task RepublishOrganizationBillingInfoAsync(string organizationId, CancellationToken cancellationToken);
    Task RepublishAllOrganizationsBillingInfoAsync(CancellationToken cancellationToken);
}

public class WorkaroundService(IRepositoryFactory repositoryFactory, IMapper mapper, IBillingPublisher billingPublisher) : IWorkaroundService
{
    public async Task RepublishOrganizationBillingInfoAsync(string organizationId, CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, false, cancellationToken);
        if (organization is null)
        {
            return;
        }

        await billingPublisher.PublishOrganizationsBillingInfoAsync([mapper.MapTo(organization)], cancellationToken);
    }

    public async Task RepublishAllOrganizationsBillingInfoAsync(CancellationToken cancellationToken)
    {
        var organizations = await repositoryFactory.OrganizationRepository.GetAllAsync(cancellationToken);
        await billingPublisher.PublishOrganizationsBillingInfoAsync(organizations.Select(mapper.MapTo), cancellationToken);
    }
}
