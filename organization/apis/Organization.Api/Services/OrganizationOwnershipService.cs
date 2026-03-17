using Api.Shared.Services;
using Enterprise.Shared.Database;
using Organization.Api.Mappers;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;

namespace Organization.Api.Services;

public interface IOrganizationOwnershipService
{
    Task VerifyAsync(string? organizationId, string? organizationUniqueAlphanumericNam, CancellationToken cancellationToken);
}

public class OrganizationOwnershipService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    IMapper mapper) : IOrganizationOwnershipService
{
    public async Task VerifyAsync(string? organizationId, string? organizationUniqueAlphanumericNam, CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               organizationId,
                               organizationUniqueAlphanumericNam,
                               cancellationToken) ??
                           throw new OrganizationNotFound();

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        organization.IsOwnershipVerified = true;

        organizationOutboxPublisher.PublishOrganizations(
            [mapper.MapTo(organization, organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))],
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
