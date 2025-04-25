using Api.Shared.Clients.Events.Skedular.Billing.V1.Key;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Kafka.Consume;
using Microsoft.EntityFrameworkCore;
using Payment.Shared.Models;
using Payment.Shared.Repositories;
using Stripe;
using Event = Api.Shared.Clients.Events.Skedular.Billing.V1.Value.Event;
using Organization = Payment.Shared.Database.Entities.Organization;
using OrganizationOffering = Payment.Shared.Database.Entities.OrganizationOffering;
using OrganizationOfferingStripePaymentIntent =
    Payment.Shared.Database.Entities.OrganizationOfferingStripePaymentIntent;
using OrganizationStripePaymentMethod = Payment.Shared.Database.Entities.OrganizationStripePaymentMethod;
using Type = Api.Shared.Clients.Events.Skedular.Billing.V1.Value.Type;

namespace Payment.Processors.Subscribers;

public class BillingSubscriber(
    ILogger<BillingSubscriber> logger,
    IRepositoryFactory repositoryFactory,
    ICreatable<PaymentIntent, PaymentIntentCreateOptions> paymentIntentCreateService)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.BillingOrganizationOfferingUpserted:
                await HandleBillingOrganizationOfferingUpsertedEventAsync(@event, cancellationToken);

                break;

            case Type.OrganizationBillingInfoUpdated:
                // TODO: 20240601 - Morteza: Need to update Stripe customer billing info
                break;

            case Type.BillingOrganizationOfferingDeleted:
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleBillingOrganizationOfferingUpsertedEventAsync(Event @event, CancellationToken cancellationToken)
    {
        var organizationOfferingBilling = @event.Data.OrganizationOfferingBilling;
        if (organizationOfferingBilling.TotalCost == 0)
        {
            // Total cost is Zero, no need to try charge customer 
            return;
        }

        if (await repositoryFactory.OrganizationOfferingStripePaymentIntentRepository.Query(
                new Specification<OrganizationOfferingStripePaymentIntent>
                {
                    Criteria = query => query.OrganizationOffering.Id == organizationOfferingBilling.OfferingId
                }).AnyAsync(cancellationToken))
        {
            return;
        }

        var organizationOffering = await repositoryFactory.OrganizationOfferingRepository
            .Query(new Specification<OrganizationOffering> { Criteria = query => query.Id == organizationOfferingBilling.OfferingId }
                .ApplyOrderBy(query => query.Id))
            .FirstOrDefaultAsync(cancellationToken);
        if (organizationOffering is null)
        {
            logger.LogError(
                "no organization offering exist with given organization offering Id: {OfferingId}",
                organizationOfferingBilling.OfferingId);

            return;
        }

        var organization = await repositoryFactory.OrganizationRepository
            .Query(new Specification<Organization> { Criteria = query => query.Id == organizationOfferingBilling.OrganizationId }
                .ApplyOrderBy(query => query.Id))
            .FirstOrDefaultAsync(cancellationToken);
        if (organization is null)
        {
            logger.LogError("no organization exist with given organization Id: {OrganizationId}", organizationOfferingBilling.OrganizationId);

            return;
        }

        var organizationStripePaymentMethods = await repositoryFactory.OrganizationStripePaymentMethodRepository.Query(
            new Specification<OrganizationStripePaymentMethod>
            {
                Criteria = query =>
                    query.Organization.Id == organizationOfferingBilling.OrganizationId &&
                    query.Status == OrganizationStripePaymentMethodStatus.Confirmed
            }).ToListAsync(cancellationToken);
        if (organizationStripePaymentMethods.Count == 0)
        {
            logger.LogError(
                "no confirmed organization payment method exist with given organization Id:  {OrganizationId}",
                organizationOfferingBilling.OrganizationId);
        }

        if (organization.StripeCustomer is null)
        {
            throw new OrganizationStripeCustomerRelationshipIsNotSetYet();
        }

        // TODO: 20240601 : Morteza: Need to implement default payment methods in future
        var organizationStripePaymentMethod = organizationStripePaymentMethods.First();
        var amount = organizationOfferingBilling.TotalCost;
        var paymentIntent = await paymentIntentCreateService.CreateAsync(
            new PaymentIntentCreateOptions
            {
                Customer = organization.StripeCustomer.StripeCustomerId,
                PaymentMethod = organizationStripePaymentMethod.PaymentMethodId,
                Amount = amount,
                // TODO: 20240601 : Morteza: Currency should not be probably hard-coded
                Currency = "usd",
                Confirm = true,
                OffSession = true
            },
            new RequestOptions { IdempotencyKey = organizationOffering.Id },
            cancellationToken);

        _ = repositoryFactory.OrganizationOfferingStripePaymentIntentRepository.Add(
            new OrganizationOfferingStripePaymentIntent
            {
                Id = paymentIntent.Id,
                OrganizationOffering = organizationOffering,
                OrganizationStripePaymentMethod = organizationStripePaymentMethod,
                Amount = paymentIntent.Amount,
                Currency = paymentIntent.Currency,
                Organization = organizationOffering.Organization
            });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
