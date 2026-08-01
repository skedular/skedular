using System.Security.Cryptography;
using System.Text;
using Booking.Api.Controllers;
using Booking.Shared.Publishers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;
using StripeConfiguration = Enterprise.Shared.Payment.Configurations.StripeConfiguration;

namespace Booking.Api.UnitTests.Controllers;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class BookingStripeWebhookControllerShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Publish_Payout_State_Events(
        [Frozen] StripeConfiguration stripeConfiguration,
        [Frozen] IBookingInternalPublisher publisher,
        [Frozen] ILogger<BookingStripeWebhookController> logger,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var sut = new BookingStripeWebhookController(stripeConfiguration, publisher, timeProvider, logger);
        var eventTypes = new[] { "payout.paid", "payout.reconciliation_completed", "payout.failed", "payout.canceled", "payout.updated" };
        foreach (var eventType in eventTypes)
        {
            var stripeEvent = new Event { Type = eventType, Data = new EventData { Object = new Payout { Id = "po_1" } } };

            await sut.PublishBookingEventAsync(stripeEvent, "{}", cancellationToken);
        }

        A.CallTo(() => publisher.PublishStripeConnectAccountWebhookEventReceivedAsync(
                "po_1", "{}", cancellationToken))
            .MustHaveHappened(5, Times.Exactly);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Accept_And_Publish_Signed_Stripe_Webhook(
        [Frozen] StripeConfiguration stripeConfiguration,
        [Frozen] IBookingInternalPublisher publisher,
        [Frozen] ILogger<BookingStripeWebhookController> logger,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        const string secret = "whsec_test";
        stripeConfiguration.BookingPlatformAccountWebhookKey = secret;
        var sut = new BookingStripeWebhookController(stripeConfiguration, publisher, timeProvider, logger);
        var webhooks = new[]
        {
            (EventType: "charge.succeeded", ObjectId: "ch_1", ObjectType: "charge"),
            (EventType: "payout.paid", ObjectId: "po_1", ObjectType: "payout"),
            (EventType: "payout.failed", ObjectId: "po_1", ObjectType: "payout"),
            (EventType: "payout.canceled", ObjectId: "po_1", ObjectType: "payout"),
            (EventType: "payout.updated", ObjectId: "po_1", ObjectType: "payout")
        };

        foreach (var webhook in webhooks)
        {
            var json =
                $"{{\"id\":\"evt_1\",\"object\":\"event\",\"api_version\":\"2025-01-27\",\"created\":1700000000,\"data\":{{\"object\":{{\"id\":\"{webhook.ObjectId}\",\"object\":\"{webhook.ObjectType}\"}}}},\"livemode\":false,\"pending_webhooks\":0,\"type\":\"{webhook.EventType}\"}}";
            sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { Request = { Body = new MemoryStream(Encoding.UTF8.GetBytes(json)) } }
            };
            var timestamp = TimeProvider.System.GetUtcNow().ToUnixTimeSeconds();
            var signedPayload = $"{timestamp}.{json}";
            await using var signedPayloadStream = new MemoryStream(Encoding.UTF8.GetBytes(signedPayload));
            var digest = await HMACSHA256.HashDataAsync(
                Encoding.UTF8.GetBytes(secret), signedPayloadStream, cancellationToken);
            var signature = $"t={timestamp},v1={Convert.ToHexString(digest).ToLowerInvariant()}";
            var result = await sut.ProcessStripePlatformAccountEvent(signature, cancellationToken);

            result.ShouldBeOfType<OkResult>();
            A.CallTo(() => publisher.PublishStripeConnectAccountWebhookEventReceivedAsync(
                    webhook.ObjectId, json, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
        }
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Rejected_Stripe_Webhook(
        [Frozen] IBookingInternalPublisher publisher,
        [Frozen] ILogger<BookingStripeWebhookController> logger,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var controller = new BookingStripeWebhookController(
            new StripeConfiguration { BookingPlatformAccountWebhookKey = "whsec-test" },
            publisher,
            timeProvider,
            logger)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { Request = { Body = new MemoryStream(Encoding.UTF8.GetBytes("{}")) } }
            }
        };

        var result = await controller.ProcessStripePlatformAccountEvent("invalid-signature", cancellationToken);

        result.ShouldNotBeNull();
        A.CallTo(logger)
            .Where(call => call.Method.Name == nameof(ILogger.Log) && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened();
        A.CallTo(() => publisher.PublishStripeConnectAccountWebhookEventReceivedAsync(A<string>._, A<string>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }
}
