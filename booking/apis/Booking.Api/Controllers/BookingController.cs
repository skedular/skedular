using System.Globalization;
using Api.Shared.Services.OpenApi.Skedular.Booking.V1;
using Booking.Api.Services;
using Booking.Shared.Publishers;
using Booking.Shared.Services;
using Enterprise.Shared.Version;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using StripeConfiguration = Enterprise.Shared.Payment.Configurations.StripeConfiguration;
using Version = Api.Shared.Services.OpenApi.Skedular.Booking.V1.Version;

namespace Booking.Api.Controllers;

[ApiController]
public class BookingController(
    IVersionService versionService,
    StripeConfiguration stripeConfiguration,
    IWorkaroundService workaroundService,
    ILocationResourceBookingSlotsHelperService locationResourceBookingSlotsHelperService,
    IBookingInternalPublisher bookingInternalPublisher,
    TimeProvider timeProvider,
    ILogger<BookingController> logger)
    : BookingControllerBase
{
    private static readonly string s_homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    public override Task<ActionResult<Version>> GetVersion(CancellationToken cancellationToken = default)
    {
        var version = versionService.GetVersion();

        return Task.FromResult<ActionResult<Version>>(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }

    public override async Task<IActionResult> Republish(string bookingId, CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishBookingAsync(bookingId, cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RepublishAll(CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishAllBookingsAsync(cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RepublishAllLocationsResourcesSlots(CancellationToken cancellationToken = default)
    {
        await locationResourceBookingSlotsHelperService.GenerateAllAsync(cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RepublishLocationResourcesSlots(string locationId, CancellationToken cancellationToken = default)
    {
        await locationResourceBookingSlotsHelperService.GenerateAsync(locationId, cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> ProcessStripePlatformAccountEvent(
        // ReSharper disable once InconsistentNaming
        string? stripe_Signature,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync(cancellationToken);

            if (stripeConfiguration.LogStripPlatformAccountWebhookMessages)
            {
                var tempFileDirectoryPath = Path.Combine(s_homeDirectory, "stripe-logs/booking/platform");
                Directory.CreateDirectory(tempFileDirectoryPath);
                var tempFilePath = Path.Combine(tempFileDirectoryPath,
                    $"{timeProvider.GetUtcNow().ToString("o", CultureInfo.InvariantCulture)}.json");
                await System.IO.File.WriteAllTextAsync(tempFilePath, json, cancellationToken);
                logger.LogInformation("Stripe Platform account event JSON logged to file: {FilePath}", tempFilePath);
            }

            _ = EventUtility.ConstructEvent(json, stripe_Signature, stripeConfiguration.BookingPlatformAccountWebhookKey,
                throwOnApiVersionMismatch: false);

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
                var tempFilePath = Path.Combine(tempFileDirectoryPath,
                    $"{timeProvider.GetUtcNow().ToString("o", CultureInfo.InvariantCulture)}.json");
                await System.IO.File.WriteAllTextAsync(tempFilePath, json, cancellationToken);
                logger.LogInformation("Stripe Connect account event JSON logged to file: {FilePath}", tempFilePath);
            }

            var stripeEvent = EventUtility.ConstructEvent(
                json,
                stripe_Signature,
                stripeConfiguration.BookingConnectAccountWebhookKey,
                throwOnApiVersionMismatch: false);
            switch (stripeEvent.Type)
            {
                case EventTypes.CheckoutSessionCompleted:
                case EventTypes.CheckoutSessionExpired:
                    var session = stripeEvent.Data.Object as Session;
                    ArgumentNullException.ThrowIfNull(session);

                    await bookingInternalPublisher.PublishStripeConnectAccountWebhookEventReceivedAsync(session.Id, json, cancellationToken);
                    break;
            }

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
}
