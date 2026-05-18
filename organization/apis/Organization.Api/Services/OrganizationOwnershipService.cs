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
}

public class OrganizationOwnershipService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    IGraphQlMapper graphQlMapper,
    ICachedOrganizationService cachedOrganizationService) : IOrganizationOwnershipService
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
    }
}
