using Enterprise.Shared.Exceptions;
using Payment.Api.Mappers;
using Payment.Api.Services.Authorization;
using Payment.Shared.Models;
using Payment.Shared.Repositories;

namespace Payment.Api.Services;

public interface IOrganizationService
{
    Task<ICollection<OrganizationStripePaymentMethod>> GetOrganizationPaymentMethodsAsync(
        string organizationId,
        CancellationToken cancellationToken);
}

public class OrganizationService(
    IRepositoryFactory repositoryFactory,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ICustomerService customerService,
    IMapper mapper)
    : IOrganizationService
{
    public async Task<ICollection<OrganizationStripePaymentMethod>> GetOrganizationPaymentMethodsAsync(
        string organizationId,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanViewPaymentMethod(organization, customer))
        {
            return [];
        }

        return mapper
            .MapTo(
                organization.OrganizationStripePaymentMethods
                    .Where(item => item.Status == OrganizationStripePaymentMethodStatus.Confirmed))
            .ToList();
    }
}
