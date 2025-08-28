using Api.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Organization.Api.Mappers;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;

namespace Organization.Api.Services;

public interface IOrganizationTaxDetailsService
{
    Task<Shared.Models.Organization> UpdateAsync(OrganizationTaxDetails taxDetails, CancellationToken cancellationToken);

    Task<Shared.Models.Organization> RemoveAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        CancellationToken cancellationToken);
}

public class OrganizationTaxDetailsService(
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    IMapper mapper,
    IDbTransactionBuilder transactionBuilder,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    IRandomHelper randomHelper) : IOrganizationTaxDetailsService
{
    public async Task<Shared.Models.Organization> UpdateAsync(OrganizationTaxDetails taxDetails, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(taxDetails.Organization);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                               taxDetails.Organization.Id,
                               taxDetails.Organization.UniqueAlphanumericName,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        if (!organizationAuthorizationService.CanModify(organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (organization.OrganizationTaxDetails is null)
        {
            taxDetails.Id = randomHelper.Generate();
            repositoryFactory.OrganizationTaxDetailsRepository.Add(mapper.MapToEntity(taxDetails, organization));
        }
        else
        {
            taxDetails.Id = organization.OrganizationTaxDetails.Id;
            repositoryFactory.OrganizationTaxDetailsRepository.Update(
                mapper.MergeToEntity(taxDetails, organization.OrganizationTaxDetails, organization));
        }

        var mappedOrganization = mapper.MapTo(
            organization,
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id));
        organizationOutboxPublisher.PublishOrganizations([mappedOrganization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mappedOrganization;
    }

    public async Task<Shared.Models.Organization> RemoveAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                               organizationId,
                               organizationUniqueAlphanumericName,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        if (!organizationAuthorizationService.CanModify(organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        if (organization.OrganizationTaxDetails is null)
        {
            return mapper.MapTo(organization, organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id));
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        _ = repositoryFactory.OrganizationTaxDetailsRepository.Remove(organization.OrganizationTaxDetails);
        organization.OrganizationTaxDetails = null;

        var mappedOrganization = mapper.MapTo(
            organization,
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id));
        organizationOutboxPublisher.PublishOrganizations([mappedOrganization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mappedOrganization;
    }
}
