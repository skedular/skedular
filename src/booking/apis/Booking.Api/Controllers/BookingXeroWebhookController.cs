using System.Globalization;
using Api.Shared.Services.OpenApi.Skedular.Booking.XeroWebhook.V1;
using Booking.Shared.Publishers;
using Booking.Shared.Services;
using Enterprise.Shared.Accounting.Configurations;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
public class BookingXeroWebhookController(
    XeroConfiguration xeroConfiguration,
    IBookingInternalPublisher bookingInternalPublisher,
    IXeroWebhookService xeroWebhookService,
    TimeProvider timeProvider,
    ILogger<BookingXeroWebhookController> logger)
    : BookingXeroWebhookControllerBase
{
    private static readonly string s_homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    public override async Task<IActionResult> ProcessXeroWebhookEvent(
        // ReSharper disable once InconsistentNaming
        string? x_xero_signature,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync(cancellationToken);
            if (xeroConfiguration.LogWebhookMessages)
            {
                var tempFileDirectoryPath = Path.Combine(s_homeDirectory, "xero-logs/booking/webhook");
                Directory.CreateDirectory(tempFileDirectoryPath);
                var tempFilePath = Path.Combine(
                    tempFileDirectoryPath,
                    $"{timeProvider.GetUtcNow().ToString("o", CultureInfo.InvariantCulture)}.json");
                await System.IO.File.WriteAllTextAsync(tempFilePath, json, cancellationToken);
                logger.LogInformation("Xero webhook event JSON logged to file: {FilePath}", tempFilePath);
            }

            var normalizedXeroSignature = x_xero_signature?.Trim();

            if (!xeroWebhookService.IsSignatureValid(json, normalizedXeroSignature))
            {
                return Unauthorized();
            }

            await bookingInternalPublisher.PublishXeroWebhookEventReceivedAsync(normalizedXeroSignature!, json, cancellationToken);

            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process Xero webhook event.");

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
