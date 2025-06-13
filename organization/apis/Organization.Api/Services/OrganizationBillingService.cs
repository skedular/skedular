using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Random;
using Organization.Api.Mappers;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;

namespace Organization.Api.Services;

public interface IOrganizationBillingService
{
    Task<OrganizationBillingDetails?> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken);
    Task<Shared.Models.Organization> AddAsync(OrganizationBillingDetails organizationBillingDetails, CancellationToken cancellationToken);
    Task<Shared.Models.Organization> UpdateAsync(OrganizationBillingDetails organizationBillingDetails, CancellationToken cancellationToken);
}

public class OrganizationBillingService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IRandomHelper randomHelper,
    IMapper mapper,
    IOrganizationOutboxPublisher organizationOutboxPublisher) : IOrganizationBillingService
{
    public async Task<OrganizationBillingDetails?> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (existingOrganization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanView(existingOrganization, customer))
        {
            throw new Unauthorized();
        }

        return mapper.MapTo(existingOrganization.OrganizationBillingDetails);
    }

    public async Task<Shared.Models.Organization> AddAsync(
        OrganizationBillingDetails organizationBillingDetails,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(organizationBillingDetails.Organization);
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationBillingDetails.Organization.Id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationBillingDetails.Organization.Id, cancellationToken);
        if (existingOrganization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanModify(existingOrganization, customer))
        {
            throw new Unauthorized();
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
                    throw new Unauthorized();
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

        existingOrganization.OrganizationBillingDetails = organizationBillingDetailsEntity;
        var mappedOrganization = mapper.MapTo(existingOrganization);

        organizationOutboxPublisher.PublishOrganizations([mapper.MapTo(existingOrganization)], repositoryFactory.UnitOfWork);

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
            cancellationToken);

        if (existingOrganizationBillingDetails is null)
        {
            throw new OrganizationBillingDetailsNotFound();
        }

        var existingOrganization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(existingOrganizationBillingDetails.Organization.Id, cancellationToken);
        if (existingOrganization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanModify(existingOrganization, customer))
        {
            throw new Unauthorized();
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

        var organizationBillingDetailsEntity =
            mapper.MergeToEntity(organizationBillingDetails, existingOrganizationBillingDetails, existingOrganization);
        repositoryFactory.OrganizationBillingDetailsRepository.Update(organizationBillingDetailsEntity);

        existingOrganization.OrganizationBillingDetails = organizationBillingDetailsEntity;

        var mappedOrganization = mapper.MapTo(existingOrganization);
        organizationOutboxPublisher.PublishOrganizations([mappedOrganization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mappedOrganization;
    }
}
