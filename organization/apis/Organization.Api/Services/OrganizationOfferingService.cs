using Api.Shared.Services;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Organization.Api.Mappers;
using Organization.Api.Services.Authorization;
using Organization.Shared.Database.Entities;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services;
using Organization.Shared.Workflows;

namespace Organization.Api.Services;

public interface IOrganizationOfferingService
{
    Task UpdateOfferingAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        OfferingCode offeringCode,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);

    Task CancelOfferingAsync(string? organizationId, string? organizationUniqueAlphanumericName, CancellationToken cancellationToken);
    Task RegenerateAllOfferingsAsync(CancellationToken cancellationToken);
    Task RerunAllOfferingsWorkflowsAsync(CancellationToken cancellationToken);
}

public class OrganizationOfferingService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    ITemporalOutboxService temporalOutboxService,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    IGraphQlMapper graphQlMapper,
    TimeProvider timeProvider) : IOrganizationOfferingService
{
    public async Task UpdateOfferingAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        OfferingCode offeringCode,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        var offering = offeringCode.GetOffering();
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               organizationId,
                               organizationUniqueAlphanumericName,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        if (!ignoreAuthorizationCheck)
        {
            var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
            if (!await organizationAuthorizationService.CanModifyAsync(organization, customer.Id, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        if (!ignoreAuthorizationCheck && offering.UnitPrice != 0 && organization.OrganizationStripePaymentMethods.Count == 0)
        {
            throw new PaymentMethodRequired();
        }

        var activeOffering = organization.OrganizationOfferings.SingleOrDefault();
        if (activeOffering is not null && activeOffering.Code == offeringCode)
        {
            return;
        }

        var now = timeProvider.GetUtcNow();
        OrganizationOffering? matchingOffering;
        if (!string.IsNullOrWhiteSpace(organizationId))
        {
            matchingOffering = await repositoryFactory.OrganizationOfferingRepository
                .GetCurrentByOrganizationIdAndCodeAsync(organizationId, offeringCode, now, true, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(organizationUniqueAlphanumericName))
        {
            matchingOffering = await repositoryFactory.OrganizationOfferingRepository
                .GetCurrentByCustomDomainAndCodeAsync(organizationUniqueAlphanumericName, offeringCode, now, true, cancellationToken);
        }
        else
        {
            throw new InvalidOperationException("Either organizationId or organizationUniqueAlphanumericName must be provided.");
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (activeOffering is not null && activeOffering.Code != offeringCode)
        {
            temporalOutboxService.SignalWorkflowScheduleRenewOrganizationOfferingCancelOffering(activeOffering.Id, repositoryFactory.UnitOfWork);

            repositoryFactory.OrganizationOfferingRepository.Remove(activeOffering);
        }

        if (matchingOffering is null)
        {
            var organizationOffering = new OrganizationOffering
            {
                Id = randomHelper.Generate(),
                Code = offeringCode,
                Start = now,
                End = now.GetOfferingPeriodStart().GetOfferingPeriodEnd(),
                AutoRenew = true,
                UnitPrice = offering.UnitPrice,
                Organization = organization
            };
            repositoryFactory.OrganizationOfferingRepository.Add(organizationOffering);
            temporalOutboxService.StartWorkflowScheduleRenewOrganizationOffering(
                new ScheduleRenewOrganizationOfferingInput(
                    organization.Id,
                    organizationOffering.Id,
                    organizationOffering.End.GetNextOfferingPeriodStart()),
                repositoryFactory.UnitOfWork);
        }
        else
        {
            repositoryFactory.OrganizationOfferingRepository.Undelete(matchingOffering);
            temporalOutboxService.StartWorkflowScheduleRenewOrganizationOffering(
                new ScheduleRenewOrganizationOfferingInput(
                    organization.Id,
                    matchingOffering.Id,
                    matchingOffering.End.GetNextOfferingPeriodStart()),
                repositoryFactory.UnitOfWork);
        }

        organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
            organizationId,
            organizationUniqueAlphanumericName,
            cancellationToken);
        organizationOutboxPublisher.PublishOrganizations(
            [
                graphQlMapper.MapTo(organization!,
                    organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization!.Id))
            ],
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task CancelOfferingAsync(string? organizationId, string? organizationUniqueAlphanumericName, CancellationToken cancellationToken) =>
        await UpdateOfferingAsync(organizationId, organizationUniqueAlphanumericName, OfferingCode.FreeTierV1, false, cancellationToken);

    public async Task RegenerateAllOfferingsAsync(CancellationToken cancellationToken)
    {
        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var now = timeProvider.GetUtcNow();
        var organizations = await repositoryFactory.OrganizationRepository.GetAllAsync(cancellationToken);
        foreach (var organization in organizations)
        {
            var offering = organization.OrganizationOfferings.First();
            offering.Start = now.GetOfferingPeriodStart();
            offering.End = offering.Start.GetOfferingPeriodStart().GetOfferingPeriodEnd();

            repositoryFactory.OrganizationOfferingRepository.Update(offering);

            organizationOutboxPublisher.PublishOrganizations(
                [
                    graphQlMapper.MapTo(organization,
                        organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))
                ],
                repositoryFactory.UnitOfWork);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task RerunAllOfferingsWorkflowsAsync(CancellationToken cancellationToken)
    {
        var organizations = await repositoryFactory.OrganizationRepository.GetAllAsync(cancellationToken);
        foreach (var organization in organizations)
        {
            var offering = organization.OrganizationOfferings.First();
            temporalOutboxService.StartWorkflowScheduleRenewOrganizationOffering(
                new ScheduleRenewOrganizationOfferingInput(
                    organization.Id,
                    offering.Id,
                    offering.End.GetNextOfferingPeriodStart()),
                repositoryFactory.UnitOfWork);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
