using System.Globalization;
using Api.Shared.Services.OpenApi.Skedular.Booking.StripeWebhook.V1;
using Booking.Shared.Publishers;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using StripeConfiguration = Enterprise.Shared.Payment.Configurations.StripeConfiguration;

namespace Booking.Api.Controllers;

[ApiController]
public class BookingStripeWebhookController(
    StripeConfiguration stripeConfiguration,
    IBookingInternalPublisher bookingInternalPublisher,
    TimeProvider timeProvider,
    ILogger<BookingStripeWebhookController> logger)
    : BookingStripeWebhookControllerBase
{
    private static readonly string s_homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    public override async Task<IActionResult> ProcessStripePlatformAccountEvent(
        // ReSharper disable once InconsistentNaming
        string? stripe_Signature,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync(cancellationToken);

            if (stripeConfiguration.LogStripePlatformAccountWebhookMessages)
            {
                var tempFileDirectoryPath = Path.Combine(s_homeDirectory, "stripe-logs/booking/platform");
                Directory.CreateDirectory(tempFileDirectoryPath);
                var tempFilePath = Path.Combine(
                    tempFileDirectoryPath,
                    $"{timeProvider.GetUtcNow().ToString("o", CultureInfo.InvariantCulture)}.json");
                await System.IO.File.WriteAllTextAsync(tempFilePath, json, cancellationToken);
                logger.LogInformation("Stripe Platform account event JSON logged to file: {FilePath}", tempFilePath);
            }

            var stripeEvent = EventUtility.ConstructEvent(json, stripe_Signature, stripeConfiguration.BookingPlatformAccountWebhookKey,
                throwOnApiVersionMismatch: false);
            await PublishBookingEventAsync(stripeEvent, json, cancellationToken);

            return Ok();
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Failed to process Stripe Platform event.");

            return BadRequest();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process Stripe Platform event.");

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public override async Task<IActionResult> ProcessStripeConnectAccountEvent(
        // ReSharper disable once InconsistentNaming
        string? stripe_Signature,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync(cancellationToken);

            if (stripeConfiguration.LogStripeConnectAccountWebhookMessages)
            {
                var tempFileDirectoryPath = Path.Combine(s_homeDirectory, "stripe-logs/booking/connect");
                Directory.CreateDirectory(tempFileDirectoryPath);
                var tempFilePath = Path.Combine(
                    tempFileDirectoryPath,
                    $"{timeProvider.GetUtcNow().ToString("o", CultureInfo.InvariantCulture)}.json");
                await System.IO.File.WriteAllTextAsync(tempFilePath, json, cancellationToken);
                logger.LogInformation("Stripe Connect account event JSON logged to file: {FilePath}", tempFilePath);
            }

            var stripeEvent = EventUtility.ConstructEvent(
                json,
                stripe_Signature,
                stripeConfiguration.BookingConnectAccountWebhookKey,
                throwOnApiVersionMismatch: false);
            await PublishBookingEventAsync(stripeEvent, json, cancellationToken);

            return Ok();
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Failed to process Stripe event.");

            return BadRequest();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process Stripe event.");

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    private async Task PublishBookingEventAsync(
        Event stripeEvent,
        string json,
        CancellationToken cancellationToken)
    {
        var eventObjectId = stripeEvent.Type switch
        {
            EventTypes.CheckoutSessionCompleted or EventTypes.CheckoutSessionExpired
                when stripeEvent.Data.Object is Session session => session.Id,
            "refund.created" or "refund.updated"
                when stripeEvent.Data.Object is Refund refund => refund.Id,
            _ => null
        };

        if (!string.IsNullOrWhiteSpace(eventObjectId))
        {
            await bookingInternalPublisher.PublishStripeConnectAccountWebhookEventReceivedAsync(
                eventObjectId,
                json,
                cancellationToken);
        }
    }
}
