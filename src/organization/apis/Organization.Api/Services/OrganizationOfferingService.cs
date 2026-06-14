using Api.Shared.Services;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Organization.Api.Mappers;
using Organization.Api.Models;
using Organization.Api.Services.Authorization;
using Organization.Shared.Database.Entities;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services;
using Organization.Shared.Services.Cache;
using Organization.Shared.Services.Pricing;
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

    Task UpdateOfferingPatchAsync(OrganizationOfferingPatchRequest request, CancellationToken cancellationToken);
    Task CancelOfferingAsync(string? organizationId, string? organizationUniqueAlphanumericName, CancellationToken cancellationToken);
    Task RegenerateAllOfferingsAsync(CancellationToken cancellationToken);
    Task RerunAllOfferingsWorkflowsAsync(CancellationToken cancellationToken);

    Task SetEnterpriseOfferingAsync(
        string organizationId,
        int fixedPrice,
        Currency currency,
        int purchasedUserCapacity,
        int purchasedLocationCapacity,
        int purchasedTeamCapacity,
        CancellationToken cancellationToken);
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
    TimeProvider timeProvider,
    ICachedOrganizationService cachedOrganizationService) : IOrganizationOfferingService
{
    public async Task UpdateOfferingAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        OfferingCode offeringCode,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        var offering = offeringCode.GetOffering();
        if (offeringCode.IsEnterpriseOffering())
        {
            throw new InvalidOperationException("Enterprise offering terms must be set by a Skedular admin.");
        }

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

        if (!ignoreAuthorizationCheck && offering.UnitPrice is > 0 && organization.OrganizationStripePaymentMethods.Count == 0)
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
            matchingOffering = await repositoryFactory.OrganizationOfferingRepository.GetCurrentByCustomDomainAndCodeAsync(
                organizationUniqueAlphanumericName,
                offeringCode,
                now,
                true,
                cancellationToken);
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
                Start = now,
                End = now.GetOfferingPeriodStart().GetOfferingPeriodEnd(),
                AutoRenew = true,
                Organization = organization
            };
            organizationOffering.ApplyOfferingTemplate(offeringCode);
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
            matchingOffering.ApplyRenewalTemplate(offeringCode);
            repositoryFactory.OrganizationOfferingRepository.Update(matchingOffering);
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

        await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);
    }

    public async Task UpdateOfferingPatchAsync(OrganizationOfferingPatchRequest request, CancellationToken cancellationToken)
    {
        ValidatePatchRequest(request);
        await UpdateOfferingAsync(request.OrganizationId, request.OrganizationCustomDomain, request.OfferingCode!.Value, false, cancellationToken);
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
                    graphQlMapper.MapTo(
                        organization,
                        organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))
                ],
                repositoryFactory.UnitOfWork);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        foreach (var organization in organizations)
        {
            await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);
        }
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

    public async Task SetEnterpriseOfferingAsync(
        string organizationId,
        int fixedPrice,
        Currency currency,
        int purchasedUserCapacity,
        int purchasedLocationCapacity,
        int purchasedTeamCapacity,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);
        if (fixedPrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fixedPrice), fixedPrice, "Enterprise offering fixed price must be zero or greater.");
        }

        if (purchasedUserCapacity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(purchasedUserCapacity),
                purchasedUserCapacity,
                "Enterprise offering user capacity must be greater than zero.");
        }

        if (purchasedLocationCapacity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(purchasedLocationCapacity),
                purchasedLocationCapacity,
                "Enterprise offering location capacity must be greater than zero.");
        }

        if (purchasedTeamCapacity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(purchasedTeamCapacity),
                purchasedTeamCapacity,
                "Enterprise offering team capacity must be greater than zero.");
        }

        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               organizationId,
                               null,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        var now = timeProvider.GetUtcNow();

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var activeOffering = organization.OrganizationOfferings.SingleOrDefault();
        if (activeOffering is null)
        {
            activeOffering = new OrganizationOffering
            {
                Id = randomHelper.Generate(),
                Organization = organization,
                Start = now,
                End = now.GetOfferingPeriodStart().GetOfferingPeriodEnd(),
                AutoRenew = true
            };
            activeOffering.ApplyNegotiatedEnterpriseTerms(
                fixedPrice,
                currency,
                purchasedUserCapacity,
                purchasedLocationCapacity,
                purchasedTeamCapacity);
            repositoryFactory.OrganizationOfferingRepository.Add(activeOffering);
            temporalOutboxService.StartWorkflowScheduleRenewOrganizationOffering(
                new ScheduleRenewOrganizationOfferingInput(
                    organization.Id,
                    activeOffering.Id,
                    activeOffering.End.GetNextOfferingPeriodStart()),
                repositoryFactory.UnitOfWork);
        }
        else
        {
            activeOffering.ApplyNegotiatedEnterpriseTerms(
                fixedPrice,
                currency,
                purchasedUserCapacity,
                purchasedLocationCapacity,
                purchasedTeamCapacity);
            activeOffering.Start = now;
            activeOffering.End = now.GetOfferingPeriodStart().GetOfferingPeriodEnd();
            activeOffering.AutoRenew = true;
            repositoryFactory.OrganizationOfferingRepository.Update(activeOffering);
        }

        organizationOutboxPublisher.PublishOrganizations(
            [
                graphQlMapper.MapTo(organization,
                    organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))
            ],
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private static void ValidatePatchRequest(OrganizationOfferingPatchRequest request)
    {
        if (request.FieldsToUpdate.Count == 0)
        {
            throw new ArgumentException("Choose at least one organization offering field to update.", nameof(request));
        }

        foreach (var field in request.FieldsToUpdate)
        {
            if (!Enum.IsDefined(field))
            {
                throw new ArgumentOutOfRangeException(nameof(request), field, "This organization offering patch field is not supported.");
            }
        }

        if (!request.FieldsToUpdate.Contains(OrganizationOfferingPatchField.OfferingCode) || request.OfferingCode is null)
        {
            throw new ArgumentException("Organization offering code is required.", nameof(request));
        }
    }
}
