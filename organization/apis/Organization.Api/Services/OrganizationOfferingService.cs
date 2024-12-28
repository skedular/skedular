using Api.Shared.Services.Offering;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore;
using Organization.Api.Mappers;
using Organization.Api.Services.Authorization;
using Organization.Shared.Database.Entities;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;

namespace Organization.Api.Services;

public interface IOrganizationOfferingService
{
    Task UpdateOfferingAsync(string organizationId, OfferingCode offeringCode, CancellationToken cancellationToken);
    Task CancelOfferingAsync(string organizationId, CancellationToken cancellationToken);
}

public class OrganizationOfferingService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    IMapper mapper,
    TimeProvider timeProvider) : IOrganizationOfferingService
{
    public async Task UpdateOfferingAsync(
        string organizationId,
        OfferingCode offeringCode,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);
        var offering = offeringCode.GetOffering();

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanModify(organization, customer))
        {
            throw new Unauthorized();
        }

        if (offering.UnitPrice != 0 && !organization.HasAttachedPaymentMethod)
        {
            throw new PaymentMethodRequired();
        }

        var activeOffering = organization.OrganizationOfferings.SingleOrDefault();
        if (activeOffering is not null && activeOffering.Code == offeringCode)
        {
            return;
        }

        var now = timeProvider.GetUtcNow();
        var matchingOffering = await repositoryFactory.OrganizationOfferingRepository.Query(
            new Specification<OrganizationOffering>
            {
                Criteria = query =>
                    query.Organization.Id == organizationId && query.Code == offeringCode && query.Start <= now &&
                    query.End >= now
            }.ApplyOrderBy(query => query.Id)).FirstOrDefaultAsync(cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.OrganizationOfferingRepository.UnitOfWork,
            cancellationToken);

        if (activeOffering is not null && activeOffering.Code != offeringCode)
        {
            repositoryFactory.OrganizationOfferingRepository.Remove(activeOffering);
        }

        if (matchingOffering is null)
        {
            repositoryFactory.OrganizationOfferingRepository.Add(new OrganizationOffering
            {
                Id = randomHelper.Generate(),
                Code = offeringCode,
                Start = now,
                End = now.GetOfferingPeriodStart().GetOfferingPeriodEnd(),
                AutoRenew = true,
                UnitPrice = offering.UnitPrice,
                Organization = organization
            });
        }
        else
        {
            repositoryFactory.OrganizationOfferingRepository.Undelete(matchingOffering);
        }

        organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
        await organizationOutboxPublisher.PublishOrganizationAsync(
            [mapper.MapTo(organization!)],
            repositoryFactory.OrganizationRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.OrganizationOfferingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task CancelOfferingAsync(string organizationId, CancellationToken cancellationToken) =>
        await UpdateOfferingAsync(organizationId, OfferingCode.FreeTierV1, cancellationToken);
}
