using Api.Shared.Services;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Microsoft.Extensions.Logging;
using Organization.Shared.Database.Entities;
using Organization.Shared.Logging;
using Organization.Shared.Mappers;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services;
using Organization.Shared.Services.Pricing;
using Organization.Shared.Workflows;
using Stripe;
using Temporalio.Activities;
using DatabaseOrganization = Organization.Shared.Database.Entities.Organization;

namespace Organization.Shared.Activities;

public record PayForOrganizationOffering(string OrganizationOfferingId);

public record RenewAutoRenewableOrganizationOfferingAsyncInput(string OrganizationId, string OrganizationOfferingId);

public class OrganizationOfferings(
    IRepositoryFactory repositoryFactory,
    IDbTransactionBuilder transactionBuilder,
    IRandomHelper randomHelper,
    IEntityMapper entityMapper,
    TimeProvider timeProvider,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    ITemporalOutboxService temporalOutboxService,
    ICreatable<PaymentIntent, PaymentIntentCreateOptions> paymentIntentCreateService,
    ILogger<OrganizationOfferings> logger)
{
    [Activity]
    public async Task PayForOrganizationOfferingAsync(PayForOrganizationOffering args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;

        var organizationOffering = await repositoryFactory.OrganizationOfferingRepository.GetByIdAsync(
            args.OrganizationOfferingId,
            cancellationToken);
        if (organizationOffering is null || organizationOffering.IsDeleted())
        {
            return;
        }

        var totalCost = organizationOffering.GetBillingAmount();
        if (totalCost == 0)
        {
            // The total cost is Zero, no need to try charge customer 
            return;
        }

        if (organizationOffering.Organization.OrganizationStripeCustomer is null ||
            organizationOffering.Organization.OrganizationStripeCustomer.IsDeleted())
        {
            throw new OrganizationStripeCustomerRelationshipIsNotSetYet();
        }

        if (organizationOffering.Organization.OrganizationStripePaymentMethods.Count == 0)
        {
            throw new OrganizationPaymentMethodNotFound();
        }

        var organizationStripePaymentMethod = organizationOffering.Organization.OrganizationStripePaymentMethods.First();
        var paymentIntent = await paymentIntentCreateService.CreateAsync(
            new PaymentIntentCreateOptions
            {
                Customer = organizationOffering.Organization.OrganizationStripeCustomer.StripeCustomerId,
                PaymentMethod = organizationStripePaymentMethod.PaymentMethodId,
                Amount = totalCost,
                Currency = organizationOffering.Currency,
                Confirm = true,
                OffSession = true
            },
            new RequestOptions { IdempotencyKey = organizationOffering.Id },
            cancellationToken);

        repositoryFactory.OrganizationStripePaymentIntentRepository.Add(
            new OrganizationStripePaymentIntent
            {
                Id = paymentIntent.Id,
                OrganizationStripePaymentMethod = organizationStripePaymentMethod,
                Amount = paymentIntent.Amount,
                Currency = paymentIntent.Currency,
                OrganizationOffering = organizationOffering
            });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            SpacesTrialLogEvents.BillingTransitionCompleted,
            "Organization offering charge completed. OrganizationId: {OrganizationId}, OrganizationOfferingId: {OrganizationOfferingId}",
            organizationOffering.Organization.Id,
            organizationOffering.Id);
    }

    [Activity]
    public async Task RenewAndPayAutoRenewableOrganizationOfferingAsync(RenewAutoRenewableOrganizationOfferingAsyncInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
            args.OrganizationId,
            null,
            cancellationToken);
        var organizationOffering = await repositoryFactory.OrganizationOfferingRepository.GetByIdAsync(
            args.OrganizationOfferingId,
            cancellationToken);
        if (organization is null || organization.IsDeleted() || organizationOffering is null ||
            organizationOffering.IsDeleted() || !organizationOffering.AutoRenew || organizationOffering.End > timeProvider.GetUtcNow())
        {
            return;
        }

        var newOrganizationOffering = CreateRenewedOffering(organization, organizationOffering);
        var paymentIntent = await CreatePaymentIntentAsync(
            newOrganizationOffering,
            $"Renew-{organizationOffering.Id}-{organizationOffering.End.UtcTicks}",
            cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);
        repositoryFactory.OrganizationOfferingRepository.Add(newOrganizationOffering);
        repositoryFactory.OrganizationOfferingRepository.Remove(organizationOffering);
        if (paymentIntent is not null)
        {
            repositoryFactory.OrganizationStripePaymentIntentRepository.Add(
                new OrganizationStripePaymentIntent
                {
                    Id = paymentIntent.Id,
                    OrganizationStripePaymentMethod = organization.OrganizationStripePaymentMethods.First(),
                    Amount = paymentIntent.Amount,
                    Currency = paymentIntent.Currency,
                    OrganizationOffering = newOrganizationOffering
                });
        }

        PublishRenewal(organization, newOrganizationOffering);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        logger.LogInformation(
            SpacesTrialLogEvents.BillingTransitionCompleted,
            "Organization offering charged and renewed. OrganizationId: {OrganizationId}, PreviousOfferingId: {PreviousOfferingId}, NewOfferingId: {NewOfferingId}",
            organization.Id,
            organizationOffering.Id,
            newOrganizationOffering.Id);
    }

    [Activity]
    public async Task<string?> RenewAutoRenewableOrganizationOfferingAsync(RenewAutoRenewableOrganizationOfferingAsyncInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;

        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
            args.OrganizationId,
            null,
            cancellationToken);
        if (organization is null || organization.IsDeleted())
        {
            return null;
        }

        var organizationOffering =
            await repositoryFactory.OrganizationOfferingRepository.GetByIdAsync(args.OrganizationOfferingId, cancellationToken);
        if (organizationOffering is null || organizationOffering.IsDeleted() || !organizationOffering.AutoRenew)
        {
            return null;
        }

        var now = timeProvider.GetUtcNow();
        if (organizationOffering.End > now)
        {
            return null;
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var newOrganizationOffering = CreateRenewedOffering(organization, organizationOffering);
        repositoryFactory.OrganizationOfferingRepository.Add(newOrganizationOffering);
        repositoryFactory.OrganizationOfferingRepository.Remove(organizationOffering);
        PublishRenewal(organization, newOrganizationOffering);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        logger.LogInformation(
            SpacesTrialLogEvents.BillingTransitionCompleted,
            "Organization offering renewed. OrganizationId: {OrganizationId}, PreviousOfferingId: {PreviousOfferingId}, NewOfferingId: {NewOfferingId}",
            organization.Id,
            organizationOffering.Id,
            newOrganizationOffering.Id);
        return newOrganizationOffering.Id;
    }

    private OrganizationOffering CreateRenewedOffering(DatabaseOrganization organization, OrganizationOffering organizationOffering)
    {
        var start = organizationOffering.End.GetNextOfferingPeriodStart();
        return new OrganizationOffering
        {
            Id = randomHelper.Generate(),
            Code = organizationOffering.Code,
            Start = start,
            End = start.GetOfferingPeriodStart().GetOfferingPeriodEnd(),
            AutoRenew = organizationOffering.AutoRenew,
            UnitPrice = organizationOffering.UnitPrice,
            FixedPrice = organizationOffering.FixedPrice,
            Currency = organizationOffering.Currency,
            PurchasedUserCapacity = organizationOffering.PurchasedUserCapacity,
            PurchasedLocationCapacity = organizationOffering.PurchasedLocationCapacity,
            PurchasedTeamCapacity = organizationOffering.PurchasedTeamCapacity,
            CatalogVersion = organizationOffering.CatalogVersion ?? organizationOffering.Code.GetCurrentCatalogVersion(),
            DiscountPercentage = organizationOffering.DiscountPercentage,
            Organization = organization
        };
    }

    private async Task<PaymentIntent?> CreatePaymentIntentAsync(
        OrganizationOffering organizationOffering,
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        var totalCost = organizationOffering.GetBillingAmount();
        if (totalCost == 0)
        {
            return null;
        }

        var organization = organizationOffering.Organization;
        if (organization.OrganizationStripeCustomer is null || organization.OrganizationStripeCustomer.IsDeleted())
        {
            throw new OrganizationStripeCustomerRelationshipIsNotSetYet();
        }

        if (organization.OrganizationStripePaymentMethods.Count == 0)
        {
            throw new OrganizationPaymentMethodNotFound();
        }

        return await paymentIntentCreateService.CreateAsync(
            new PaymentIntentCreateOptions
            {
                Customer = organization.OrganizationStripeCustomer.StripeCustomerId,
                PaymentMethod = organization.OrganizationStripePaymentMethods.First().PaymentMethodId,
                Amount = totalCost,
                Currency = organizationOffering.Currency,
                Confirm = true,
                OffSession = true
            },
            new RequestOptions { IdempotencyKey = idempotencyKey },
            cancellationToken);
    }

    private void PublishRenewal(DatabaseOrganization organization, OrganizationOffering newOrganizationOffering)
    {
        var mappedOrganization = entityMapper.MapTo(organization);
        mappedOrganization.OrganizationOfferings = [entityMapper.MapTo(newOrganizationOffering, mappedOrganization)];
        organizationOutboxPublisher.PublishOrganizations([mappedOrganization], repositoryFactory.UnitOfWork);
        temporalOutboxService.StartWorkflowScheduleRenewOrganizationOffering(
            new ScheduleRenewOrganizationOfferingInput(
                organization.Id,
                newOrganizationOffering.Id,
                newOrganizationOffering.End.GetNextOfferingPeriodStart()),
            repositoryFactory.UnitOfWork);
    }
}
