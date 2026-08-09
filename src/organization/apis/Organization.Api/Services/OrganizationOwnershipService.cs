using Api.Shared.Services;
using Enterprise.Shared.Database;
using Organization.Api.Mappers;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services.Cache;

namespace Organization.Api.Services;

public interface IOrganizationOwnershipService
{
    Task VerifyAsync(string? id, string? customDomain, CancellationToken cancellationToken);
    Task UnverifyAsync(string? id, string? customDomain, CancellationToken cancellationToken);
}

public class OrganizationOwnershipService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    IGraphQlMapper graphQlMapper,
    ICachedOrganizationService cachedOrganizationService,
    ILogger<OrganizationOwnershipService> logger) : IOrganizationOwnershipService
{
    public async Task VerifyAsync(string? id, string? customDomain, CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(id, customDomain, cancellationToken) ??
                           throw new OrganizationNotFound();

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        organization.IsOwnershipVerified = true;

        organizationOutboxPublisher.PublishOrganizations(
            [graphQlMapper.MapTo(organization, organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))],
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);
        await cachedOrganizationService.RemoveMyOrganizationsByCustomerIdsAsync(
            [.. organization.OrganizationMembers.Select(item => item.CustomerId)],
            cancellationToken);

        logger.LogInformation(
            "Organization ownership verified. OrganizationId: {OrganizationId}, OrganizationType: {OrganizationType}",
            organization.Id,
            organization.Type);
    }

    public async Task UnverifyAsync(string? id, string? customDomain, CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(id, customDomain, cancellationToken) ??
                           throw new OrganizationNotFound();

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        organization.IsOwnershipVerified = false;

        organizationOutboxPublisher.PublishOrganizations(
            [graphQlMapper.MapTo(organization, organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))],
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);
        await cachedOrganizationService.RemoveMyOrganizationsByCustomerIdsAsync(
            [.. organization.OrganizationMembers.Select(item => item.CustomerId)],
            cancellationToken);

        logger.LogWarning(
            "Organization ownership verification removed; public listings will be hidden while existing bookings remain active. OrganizationId: {OrganizationId}, OrganizationType: {OrganizationType}",
            organization.Id,
            organization.Type);
    }
}
