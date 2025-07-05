using Api.Shared.Services;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Enterprise.Shared.Security.Sso;
using Organization.Api.Mappers;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Constants = Enterprise.Shared.Security.Sso.Models.Constants;
using OrganizationSsoValidationResult = Organization.Shared.Configurations.OrganizationSsoValidationResult;

namespace Organization.Api.Services;

public interface IOrganizationTaxDetailsService
{
    Task<Shared.Models.Organization> UpdateAsync(OrganizationTaxDetails taxDetails, CancellationToken cancellationToken);
    Task<Shared.Models.Organization> RemoveAsync(string organizationId, CancellationToken cancellationToken);
}

public class OrganizationTaxDetailsService(
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IMapper mapper,
    IDbTransactionBuilder transactionBuilder,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    IRandomHelper randomHelper) : IOrganizationTaxDetailsService
{
    public async Task<Shared.Models.Organization> UpdateAsync(OrganizationTaxDetails taxDetails, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(taxDetails.Organization);
        ArgumentException.ThrowIfNullOrWhiteSpace(taxDetails.Organization.Id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(taxDetails.Organization.Id, cancellationToken) ??
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

        organizationOutboxPublisher.PublishOrganizations([mapper.MapTo(organization)], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo(organization);
    }

    public async Task<Shared.Models.Organization> RemoveAsync(string organizationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken) ??
                           throw new OrganizationNotFound();
        if (!organizationAuthorizationService.CanModify(organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        if (organization.OrganizationTaxDetails is null)
        {
            return mapper.MapTo(organization);
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        _ = repositoryFactory.OrganizationTaxDetailsRepository.Remove(organization.OrganizationTaxDetails);
        organization.OrganizationTaxDetails = null;

        organizationOutboxPublisher.PublishOrganizations([mapper.MapTo(organization)], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo(organization);
    }
}
