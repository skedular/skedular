using Api.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Organization.Api.Mappers;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;

namespace Organization.Api.Services;

public interface IOrganizationPhysicalAddressService
{
    Task<Shared.Models.Organization> AddAsync(OrganizationPhysicalAddress organizationPhysicalAddress, CancellationToken cancellationToken);
    Task<Shared.Models.Organization> UpdateAsync(OrganizationPhysicalAddress organizationPhysicalAddress, CancellationToken cancellationToken);
}

public class OrganizationPhysicalAddressService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    IRandomHelper randomHelper,
    IMapper mapper,
    IOrganizationOutboxPublisher organizationOutboxPublisher) : IOrganizationPhysicalAddressService
{
    public async Task<Shared.Models.Organization> AddAsync(OrganizationPhysicalAddress organizationPhysicalAddress,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(organizationPhysicalAddress.Organization);
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationPhysicalAddress.Organization.Id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationPhysicalAddress.Organization.Id, cancellationToken) ??
            throw new OrganizationNotFound();

        if (!organizationAuthorizationService.CanModify(existingOrganization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        if (!string.IsNullOrWhiteSpace(organizationPhysicalAddress.Id))
        {
            var existingOrganizationPhysicalAddress = await repositoryFactory.OrganizationPhysicalAddressRepository.GetByIdAsync(
                organizationPhysicalAddress.Id,
                cancellationToken);
            if (existingOrganizationPhysicalAddress is not null)
            {
                if (existingOrganizationPhysicalAddress.Organization.Id != existingOrganization.Id)
                {
                    throw new UnauthorizedAccessException();
                }

                return await UpdateInternalAsync(
                    organizationPhysicalAddress,
                    existingOrganizationPhysicalAddress,
                    existingOrganization,
                    cancellationToken);
            }
        }
        else
        {
            organizationPhysicalAddress.Id = randomHelper.Generate();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var organizationPhysicalAddressEntity = mapper.MapTo(organizationPhysicalAddress, existingOrganization);
        repositoryFactory.OrganizationPhysicalAddressRepository.Add(organizationPhysicalAddressEntity);

        existingOrganization.PhysicalAddress = organizationPhysicalAddressEntity;
        var mappedOrganization = mapper.MapTo(
            existingOrganization,
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(existingOrganization.Id));

        organizationOutboxPublisher.PublishOrganizations([mappedOrganization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mappedOrganization;
    }

    public async Task<Shared.Models.Organization> UpdateAsync(
        OrganizationPhysicalAddress organizationPhysicalAddress,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(organizationPhysicalAddress.Id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganizationPhysicalAddress = await repositoryFactory.OrganizationPhysicalAddressRepository.GetByIdAsync(
            organizationPhysicalAddress.Id,
            cancellationToken) ?? throw new OrganizationPhysicalAddressNotFound();

        var existingOrganization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(existingOrganizationPhysicalAddress.Organization.Id, cancellationToken) ??
            throw new OrganizationNotFound();

        if (!organizationAuthorizationService.CanModify(existingOrganization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        return await UpdateInternalAsync(organizationPhysicalAddress, existingOrganizationPhysicalAddress, existingOrganization, cancellationToken);
    }

    private async Task<Shared.Models.Organization> UpdateInternalAsync(
        OrganizationPhysicalAddress organizationPhysicalAddress,
        Shared.Database.Entities.OrganizationPhysicalAddress existingOrganizationPhysicalAddress,
        Shared.Database.Entities.Organization existingOrganization,
        CancellationToken cancellationToken)
    {
        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        existingOrganizationPhysicalAddress = mapper.MergeTo(organizationPhysicalAddress, existingOrganizationPhysicalAddress, existingOrganization);
        repositoryFactory.OrganizationPhysicalAddressRepository.Update(existingOrganizationPhysicalAddress);

        existingOrganization.PhysicalAddress = existingOrganizationPhysicalAddress;

        var mappedOrganization = mapper.MapTo(
            existingOrganization,
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(existingOrganization.Id));
        organizationOutboxPublisher.PublishOrganizations([mappedOrganization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mappedOrganization;
    }
}
