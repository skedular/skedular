using Api.Shared.Clients.Events.Skedular.Billing.V1.Key;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Kafka.Consume;
using Microsoft.EntityFrameworkCore;
using Payment.Shared.Repositories;
using Stripe;
using Event = Api.Shared.Clients.Events.Skedular.Billing.V1.Value.Event;
using Organization = Payment.Shared.Database.Entities.Organization;
using OrganizationOffering = Payment.Shared.Database.Entities.OrganizationOffering;
using StripePaymentIntent = Payment.Shared.Database.Entities.StripePaymentIntent;
using StripePaymentMethod = Payment.Shared.Database.Entities.StripePaymentMethod;
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
            // The total cost is Zero, no need to try charge customer 
            return;
        }

        if (await repositoryFactory.OrganizationOfferingRepository.Query(
                new Specification<OrganizationOffering>
                {
                    Criteria = query => query.Id == organizationOfferingBilling.OfferingId && query.StripePaymentIntent != null
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
                "No organization offering exist with given organization offering Id: {OfferingId}",
                organizationOfferingBilling.OfferingId);

            return;
        }

        var organization = await repositoryFactory.OrganizationRepository
            .Query(new Specification<Organization> { Criteria = query => query.Id == organizationOfferingBilling.OrganizationId }
                .ApplyOrderBy(query => query.Id))
            .Include(query => query.StripeCustomers.Where(item => item.StripeConnectAccount == null))
            .FirstOrDefaultAsync(cancellationToken);
        if (organization is null)
        {
            logger.LogError("No organization exist with given organization Id: {OrganizationId}", organizationOfferingBilling.OrganizationId);

            return;
        }

        var stripePaymentMethods = await repositoryFactory.StripePaymentMethodRepository.Query(
            new Specification<StripePaymentMethod>
            {
                Criteria = query => query.Organization != null && query.Organization.Id == organizationOfferingBilling.OrganizationId &&
                                    query.Status == StripePaymentMethodStatusConstants.Confirmed
            }).ToListAsync(cancellationToken);
        if (stripePaymentMethods.Count == 0)
        {
            logger.LogError(
                "No confirmed organization payment method exist with given organization Id: {OrganizationId}",
                organizationOfferingBilling.OrganizationId);

            return;
        }

        if (organization.StripeCustomers.Count != 1)
        {
            throw new OrganizationStripeCustomerRelationshipIsNotSetYet();
        }

        // TODO: 20240601 : Morteza: Need to implement default payment methods in future
        var stripePaymentMethod = stripePaymentMethods.First();
        var amount = organizationOfferingBilling.TotalCost;
        var paymentIntent = await paymentIntentCreateService.CreateAsync(
            new PaymentIntentCreateOptions
            {
                Customer = organization.StripeCustomers.Single().StripeCustomerId,
                PaymentMethod = stripePaymentMethod.PaymentMethodId,
                Amount = amount,
                // TODO: 20240601 : Morteza: Currency should not be probably hard-coded
                Currency = "usd",
                Confirm = true,
                OffSession = true
            },
            new RequestOptions { IdempotencyKey = organizationOffering.Id },
            cancellationToken);

        repositoryFactory.StripePaymentIntentRepository.Add(
            new StripePaymentIntent
            {
                Id = paymentIntent.Id,
                StripePaymentMethod = stripePaymentMethod,
                Amount = paymentIntent.Amount,
                Currency = paymentIntent.Currency,
                OrganizationOffering = organizationOffering
            });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
