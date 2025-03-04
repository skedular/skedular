using Billing.Api.Mappers;
using Billing.Api.Services.Authorization;
using Billing.Shared.Models;
using Billing.Shared.Publishers;
using Billing.Shared.Repositories;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;

namespace Billing.Api.Services;

public interface IOrganizationBillingService
{
    Task<Organization> GetBillingInfoById(string organizationId, CancellationToken cancellationToken);

    Task<Organization> SetBillingInfoAsync(
        string organizationId,
        string? email,
        string? addressLine1,
        string? addressLine2,
        string? suburb,
        string? city,
        string? province,
        string? zipcode,
        string? country,
        CancellationToken cancellationToken);
}

public class OrganizationBillingService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ICachedCustomerService cachedCustomerService,
    ICustomerService customerService,
    IMapper mapper,
    IBillingOutboxPublisher billingOutboxPublisher)
    : IOrganizationBillingService
{
    public async Task<Organization> GetBillingInfoById(string organizationId, CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, false, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanViewBillingInfo(organization, customer))
        {
            throw new Unauthorized();
        }

        return mapper.MapTo(organization);
    }

    public async Task<Organization> SetBillingInfoAsync(
        string organizationId,
        string? email,
        string? addressLine1,
        string? addressLine2,
        string? suburb,
        string? city,
        string? province,
        string? zipcode,
        string? country,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, false, cancellationToken);
        if (existingOrganization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanManageBillingInfo(existingOrganization, customer))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        existingOrganization.BillingContactEmail = email;
        existingOrganization.BillingContactAddressLine1 = addressLine1;
        existingOrganization.BillingContactAddressLine2 = addressLine2;
        existingOrganization.BillingContactSuburb = suburb;
        existingOrganization.BillingContactCity = city;
        existingOrganization.BillingContactProvince = province;
        existingOrganization.BillingContactZipcode = zipcode;
        existingOrganization.BillingContactCountry = country;

        var organization = mapper.MapTo(repositoryFactory.OrganizationRepository.Update(existingOrganization));

        await billingOutboxPublisher.PublishOrganizationsBillingInfoAsync([organization], repositoryFactory.UnitOfWork, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return organization;
    }
}
