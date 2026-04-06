using Api.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Organization.Api.Mappers;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;

namespace Organization.Api.Services;

public interface IOrganizationBillingService
{
    Task<OrganizationBillingDetails?> GetAsync(string? organizationId, string? organizationCustomDomain, CancellationToken cancellationToken);
    Task<Shared.Models.Organization> AddAsync(OrganizationBillingDetails organizationBillingDetails, CancellationToken cancellationToken);
    Task<Shared.Models.Organization> UpdateAsync(OrganizationBillingDetails organizationBillingDetails, CancellationToken cancellationToken);
}

public class OrganizationBillingService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    IRandomHelper randomHelper,
    IMapper mapper,
    IOrganizationOutboxPublisher organizationOutboxPublisher) : IOrganizationBillingService
{
    public async Task<OrganizationBillingDetails?> GetAsync(
        string? organizationId,
        string? organizationCustomDomain,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                       organizationId,
                                       organizationCustomDomain,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();
        if (!await organizationAuthorizationService.CanViewAsync(existingOrganization, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        return mapper.MapTo(existingOrganization.BillingDetails);
    }

    public async Task<Shared.Models.Organization> AddAsync(OrganizationBillingDetails organizationBillingDetails, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(organizationBillingDetails.Organization);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                       organizationBillingDetails.Organization.Id,
                                       organizationBillingDetails.Organization.CustomDomain,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();

        if (!await organizationAuthorizationService.CanModifyAsync(existingOrganization, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        if (!string.IsNullOrWhiteSpace(organizationBillingDetails.Id))
        {
            var existingOrganizationBillingDetails = await repositoryFactory.OrganizationBillingDetailsRepository.GetByIdAsync(
                organizationBillingDetails.Id,
                cancellationToken);
            if (existingOrganizationBillingDetails is not null)
            {
                if (existingOrganizationBillingDetails.Organization.Id != existingOrganization.Id)
                {
                    throw new UnauthorizedAccessException();
                }

                return await UpdateInternalAsync(
                    organizationBillingDetails,
                    existingOrganizationBillingDetails,
                    existingOrganization,
                    cancellationToken);
            }
        }
        else
        {
            organizationBillingDetails.Id = randomHelper.Generate();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var organizationBillingDetailsEntity = mapper.MapTo(organizationBillingDetails, existingOrganization);
        repositoryFactory.OrganizationBillingDetailsRepository.Add(organizationBillingDetailsEntity);

        existingOrganization.BillingDetails = organizationBillingDetailsEntity;
        var mappedOrganization = mapper.MapTo(
            existingOrganization,
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(existingOrganization.Id));

        organizationOutboxPublisher.PublishOrganizations([mappedOrganization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mappedOrganization;
    }

    public async Task<Shared.Models.Organization> UpdateAsync(
        OrganizationBillingDetails organizationBillingDetails,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(organizationBillingDetails.Id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganizationBillingDetails = await repositoryFactory.OrganizationBillingDetailsRepository.GetByIdAsync(
            organizationBillingDetails.Id,
            cancellationToken) ?? throw new OrganizationBillingDetailsNotFound();

        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                       organizationBillingDetails.Organization.Id,
                                       organizationBillingDetails.Organization.CustomDomain,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();

        if (!await organizationAuthorizationService.CanModifyAsync(existingOrganization, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        return await UpdateInternalAsync(organizationBillingDetails, existingOrganizationBillingDetails, existingOrganization, cancellationToken);
    }

    private async Task<Shared.Models.Organization> UpdateInternalAsync(
        OrganizationBillingDetails organizationBillingDetails,
        Shared.Database.Entities.OrganizationBillingDetails existingOrganizationBillingDetails,
        Shared.Database.Entities.Organization existingOrganization,
        CancellationToken cancellationToken)
    {
        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        existingOrganizationBillingDetails =
            mapper.MergeToEntity(organizationBillingDetails, existingOrganizationBillingDetails, existingOrganization);
        repositoryFactory.OrganizationBillingDetailsRepository.Update(existingOrganizationBillingDetails);

        existingOrganization.BillingDetails = existingOrganizationBillingDetails;

        var mappedOrganization = mapper.MapTo(
            existingOrganization,
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(existingOrganization.Id));
        organizationOutboxPublisher.PublishOrganizations([mappedOrganization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mappedOrganization;
    }
}
