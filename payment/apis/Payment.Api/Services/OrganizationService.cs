using Api.Shared.Services.Models;
using Enterprise.Shared.Exceptions;
using Payment.Api.Mappers;
using Payment.Api.Services.Authorization;
using Payment.Shared.Models;
using Payment.Shared.Repositories;

namespace Payment.Api.Services;

public interface IOrganizationService
{
    Task<ICollection<StripePaymentMethod>> GetOrganizationPaymentMethodsAsync(string organizationId, CancellationToken cancellationToken);
}

public class OrganizationService(
    IRepositoryFactory repositoryFactory,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ICachedCustomerService cachedCustomerService,
    IMapper mapper)
    : IOrganizationService
{
    public async Task<ICollection<StripePaymentMethod>> GetOrganizationPaymentMethodsAsync(
        string organizationId,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, false, false, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanViewPaymentMethod(organization, customer))
        {
            return [];
        }

        return mapper
            .MapTo(organization.StripePaymentMethods.Where(item => item.Status == StripePaymentMethodStatusConstants.Confirmed))
            .ToList();
    }
}
