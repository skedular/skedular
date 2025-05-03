using Api.Shared.Clients.Events.Skedular.Booking.V1.Key;
using Api.Shared.Services.Models;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Kafka.Consume;
using Enterprise.Shared.Random;
using Payment.Processors.Mappers;
using Payment.Shared.Database.Entities;
using Payment.Shared.Publishers;
using Payment.Shared.Repositories;
using Payment.Shared.Services;
using Stripe;
using Stripe.Checkout;
using Event = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Event;
using Booking = Payment.Shared.Database.Entities.Booking;
using Type = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Type;

namespace Payment.Processors.Subscribers;

public class BookingSubscriber(
    ApplicationConfiguration applicationConfiguration,
    ILogger<BookingSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IOrganizationStripeConnectAccountHelper organizationStripeConnectAccountHelper,
    ICreatable<Session, SessionCreateOptions> sessionCreateService,
    IStripeCustomerService stripeCustomerService,
    IPaymentPublisher paymentPublisher,
    IStripeProductPricingService stripeProductPricingService) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.BookingUpserted:
                {
                    var booking = mapper.MapTo(@event);
                    if (!booking.IsPaymentRequired || booking.LineItems.Count == 0 ||
                        (booking.PaidByCustomer is null && booking.PaidByOrganization is null))
                    {
                        await HandleBookingDeletedEventAsync(booking, cancellationToken);
                    }
                    else
                    {
                        var existingBooking = await repositoryFactory.BookingRepository.UpsertNakedAsync(booking.Id, cancellationToken);
                        if (existingBooking.EventRaisedAt > booking.EventRaisedAt)
                        {
                            logger.LogInformation("Ignoring Booking event. Event timestamp is older that what is already processed.");

                            return EventSubscriberResults.Success;
                        }

                        await HandleBookingUpsertedEventAsync(@event, booking, existingBooking, cancellationToken);
                    }
                }
                break;

            case Type.BookingDeleted:
                {
                    var booking = mapper.MapTo(@event);
                    await HandleBookingDeletedEventAsync(booking, cancellationToken);
                }
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleBookingUpsertedEventAsync(
        Event @event,
        Shared.Models.Booking booking,
        Booking existingBooking,
        CancellationToken cancellationToken)
    {
        var productVersionIds = booking.LineItems.Select(item => item.ProductVersionId).Distinct().ToList();
        var productVersions = await repositoryFactory.ProductVersionRepository.GetByIdsAsync(productVersionIds, cancellationToken);
        if (productVersions.Count != productVersionIds.Count)
        {
            throw new InvalidOperationException();
        }

        if (productVersions.Any(item => item.StripePrice is null))
        {
            throw new StripePriceRelationshipIsNotSetYet();
        }

        var organizationIds = productVersions.Select(item => item.Product.Organization.Id).Distinct().ToList();
        if (organizationIds.Count > 1)
        {
            throw new CrossOrganizationProductBookingNotAllowed();
        }

        var stripeConnectAccount = organizationStripeConnectAccountHelper.GetStripeAccount(productVersions.First().Product.Organization);

        foreach (var productVersion in productVersions)
        {
            if (productVersion.StripeProduct is not null && productVersion.StripePrice is not null)
            {
                continue;
            }

            var (stripeProduct, stripePrice) = await stripeProductPricingService.UpsertProductPricingAsync(
                mapper.MapTo(productVersion),
                productVersion,
                stripeConnectAccount,
                cancellationToken);

            productVersion.StripeProduct = stripeProduct;
            productVersion.StripePrice = stripePrice;
            _ = repositoryFactory.ProductVersionRepository.Update(productVersion);
        }

        var lineItems = booking.LineItems.SelectMany(item =>
        {
            var productVersion = productVersions.First(productVersion => productVersion.Id == item.ProductVersionId);
            return booking.Schedules.Select(schedule =>
            {
                var quantity = productVersion.PriceUnit switch
                {
                    PriceUnitConstants.PerMinute => Convert.ToInt32((schedule.Until - schedule.From).TotalMinutes) * item.Quantity,
                    PriceUnitConstants.PerHour => Convert.ToInt32((schedule.Until - schedule.From).TotalHours) * item.Quantity,
                    PriceUnitConstants.PerUse => item.Quantity,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return new SessionLineItemOptions { Price = productVersion.StripePrice!.StripePriceId, Quantity = quantity };
            });
        }).ToList();

        StripeCustomer stripeCustomer;
        if (booking.PaidByCustomer is not null)
        {
            var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(booking.PaidByCustomer.Id, cancellationToken);
            if (customer is null)
            {
                throw new CustomerNotFound();
            }

            stripeCustomer = await stripeCustomerService.UpsertCustomerAsync(
                mapper.MapTo(customer),
                customer,
                stripeConnectAccount,
                @event.Metadata.Id,
                cancellationToken);
        }
        else if (booking.PaidByOrganization is not null)
        {
            var organization =
                await repositoryFactory.OrganizationRepository.GetByIdAsync(booking.PaidByOrganization.Id, false, false, cancellationToken);
            if (organization is null)
            {
                throw new OrganizationNotFound();
            }

            stripeCustomer = await stripeCustomerService.UpsertCustomerAsync(
                mapper.MapTo(organization)!,
                organization,
                stripeConnectAccount,
                @event.Metadata.Id,
                cancellationToken);
        }
        else
        {
            throw new InvalidOperationException();
        }

        StripeCheckoutSession stripeCheckoutSession;
        if (existingBooking.StripeCheckoutSession is null)
        {
            var session = await sessionCreateService.CreateAsync(
                new SessionCreateOptions
                {
                    Customer = stripeCustomer.StripeCustomerId,
                    LineItems = lineItems,
                    Mode = "payment",
                    UiMode = "hosted",
                    PaymentMethodTypes = ["card"],
                    ClientReferenceId = booking.Id,
                    SuccessUrl = applicationConfiguration.WebAppBaseDomain,
                    CancelUrl = applicationConfiguration.WebAppBaseDomain
                },
                new RequestOptions { IdempotencyKey = booking.Id, StripeAccount = stripeConnectAccount.StripeAccountId },
                cancellationToken);
            stripeCheckoutSession = new StripeCheckoutSession
            {
                Id = randomHelper.Generate(),
                StripeCheckoutSessionId = session.Id,
                CheckoutUrl = session.Url,
                PaymentStatus = session.PaymentStatus switch
                {
                    "no_payment_required" => PaymentStatusConstants.NoPaymentRequired,
                    "unpaid" => PaymentStatusConstants.Pending,
                    "paid" => PaymentStatusConstants.Paid,
                    _ => throw new ArgumentOutOfRangeException()
                },
                StripeCustomer = stripeCustomer
            };

            stripeCheckoutSession = repositoryFactory.StripeCheckoutSessionRepository.Add(stripeCheckoutSession);
        }
        else
        {
            stripeCheckoutSession = existingBooking.StripeCheckoutSession;
        }

        _ = repositoryFactory.BookingRepository.Update(mapper.MergeToEntity(booking, existingBooking, stripeCheckoutSession));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await paymentPublisher.PublishBookingPaymentAsync([mapper.MapTo(stripeCheckoutSession)], cancellationToken);
    }

    private async Task HandleBookingDeletedEventAsync(Shared.Models.Booking booking, CancellationToken cancellationToken)
    {
        var existingBooking = await repositoryFactory.BookingRepository.GetByIdAsync(booking.Id, cancellationToken);
        if (existingBooking is not null && existingBooking.EventRaisedAt > booking.EventRaisedAt)
        {
            logger.LogInformation("Ignoring Booking event. Event timestamp is older that what is already processed.");

            return;
        }

        if (existingBooking is null)
        {
            return;
        }

        _ = repositoryFactory.BookingRepository.Remove(existingBooking);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
