using Organization.Api.Mappers;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;

namespace Organization.Api.Services;

public interface IWorkaroundService
{
    Task RepublishOrganizationAsync(string organizationId, CancellationToken cancellationToken);
    Task RepublishAllOrganizationsAsync(CancellationToken cancellationToken);
}

public class WorkaroundService(
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IOrganizationPublisher organizationPublisher,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService)
    : IWorkaroundService
{
    public async Task RepublishOrganizationAsync(string organizationId, CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (organization is null)
        {
            return;
        }

        await organizationPublisher.PublishOrganizationsAsync(
            [mapper.MapTo(organization, organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))],
            cancellationToken);
    }

    public async Task RepublishAllOrganizationsAsync(CancellationToken cancellationToken)
    {
        var organizations = await repositoryFactory.OrganizationRepository.GetAllAsync(cancellationToken);
        await organizationPublisher.PublishOrganizationsAsync(
            organizations.Select(item =>
                mapper.MapTo(item, organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(item.Id))), cancellationToken);
    }
}
