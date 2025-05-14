using System.Globalization;
using Api.Shared.Services.OpenApi.Skedular.Payment.V1;
using Microsoft.AspNetCore.Mvc;
using Payment.Api.Services;
using Payment.Shared.Publishers;
using Stripe;
using Stripe.Checkout;
using StripeConfiguration = Payment.Shared.Configurations.StripeConfiguration;

namespace Payment.Api.Controllers;

[ApiController]
public class PaymentController(
    StripeConfiguration stripeConfiguration,
    IWorkaroundService workaroundService,
    IOrganizationPaymentService organizationPaymentService,
    ICustomerPaymentService customerPaymentService,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    IPaymentInternalPublisher paymentInternalPublisher,
    ILogger<PaymentController> logger,
    TimeProvider timeProvider) : PaymentControllerBase
{
    private static readonly string s_homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    public override async Task<IActionResult> AddOrganizationPaymentMethod(
        // ReSharper disable InconsistentNaming
        string setup_intent,
        string setup_intent_client_secret,
        string redirect_status,
        // ReSharper restore InconsistentNaming
        CancellationToken cancellationToken = default) =>
        Redirect(await organizationPaymentService.HandleStripePaymentMethodEventAsync(
            setup_intent,
            setup_intent_client_secret,
            redirect_status,
            cancellationToken));

    public override async Task<IActionResult> AddCustomerPaymentMethod(
        // ReSharper disable InconsistentNaming
        string setup_intent,
        string setup_intent_client_secret,
        string redirect_status,
        // ReSharper restore InconsistentNaming
        CancellationToken cancellationToken = default) =>
        Redirect(await customerPaymentService.HandleStripePaymentMethodEventAsync(
            setup_intent,
            setup_intent_client_secret,
            redirect_status,
            cancellationToken));

    public override async Task<IActionResult> RefreshOrganizationStripeConnectAccountOnboarding(
        string code,
        CancellationToken cancellationToken = default)
    {
        var onboardingUrl = await organizationStripeConnectAccountService.GetNewOnboardingUrlAsync(code, cancellationToken);
        return Redirect(onboardingUrl);
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
                var tempFileDirectoryPath = Path.Combine(s_homeDirectory, "stripe-logs/platform");
                Directory.CreateDirectory(tempFileDirectoryPath);
                var tempFilePath = Path.Combine(tempFileDirectoryPath,
                    $"{timeProvider.GetUtcNow().ToString("o", CultureInfo.InvariantCulture)}.json");
                await System.IO.File.WriteAllTextAsync(tempFilePath, json, cancellationToken);
                logger.LogInformation("Stripe Platform account event JSON logged to file: {FilePath}", tempFilePath);
            }

            _ = EventUtility.ConstructEvent(json, stripe_Signature, stripeConfiguration.PlatformAccountWebhookKey, throwOnApiVersionMismatch: false);

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
                var tempFileDirectoryPath = Path.Combine(s_homeDirectory, "stripe-logs/connect");
                Directory.CreateDirectory(tempFileDirectoryPath);
                var tempFilePath = Path.Combine(tempFileDirectoryPath,
                    $"{timeProvider.GetUtcNow().ToString("o", CultureInfo.InvariantCulture)}.json");
                await System.IO.File.WriteAllTextAsync(tempFilePath, json, cancellationToken);
                logger.LogInformation("Stripe Connect account event JSON logged to file: {FilePath}", tempFilePath);
            }

            var stripeEvent = EventUtility.ConstructEvent(
                json,
                stripe_Signature,
                stripeConfiguration.ConnectAccountWebhookKey,
                throwOnApiVersionMismatch: false);
            switch (stripeEvent.Type)
            {
                case EventTypes.AccountApplicationAuthorized:
                case EventTypes.AccountApplicationDeauthorized:
                    {
                        await paymentInternalPublisher.PublishStripeConnectAccountWebhookEventReceivedAsync(
                            stripeEvent.Account,
                            json,
                            cancellationToken);
                    }
                    break;

                case EventTypes.AccountExternalAccountCreated:
                case EventTypes.AccountExternalAccountDeleted:
                case EventTypes.AccountExternalAccountUpdated:
                    await paymentInternalPublisher.PublishStripeConnectAccountWebhookEventReceivedAsync(
                        stripeEvent.Account,
                        json,
                        cancellationToken);

                    break;

                case EventTypes.AccountUpdated:
                    {
                        var stripeAccount = stripeEvent.Data.Object as Account;
                        ArgumentNullException.ThrowIfNull(stripeAccount);

                        await paymentInternalPublisher.PublishStripeConnectAccountWebhookEventReceivedAsync(
                            stripeAccount.Id,
                            json,
                            cancellationToken);
                    }

                    break;

                case EventTypes.CustomerCreated:
                case EventTypes.CustomerUpdated:
                case EventTypes.CustomerDeleted:
                    var customer = stripeEvent.Data.Object as Customer;
                    ArgumentNullException.ThrowIfNull(customer);

                    await paymentInternalPublisher.PublishStripeConnectAccountWebhookEventReceivedAsync(customer.Id, json, cancellationToken);
                    break;

                case EventTypes.ChargePending:
                case EventTypes.ChargeSucceeded:
                case EventTypes.ChargeUpdated:
                case EventTypes.ChargeExpired:
                case EventTypes.ChargeFailed:
                    var charge = stripeEvent.Data.Object as Charge;
                    ArgumentNullException.ThrowIfNull(charge);

                    await paymentInternalPublisher.PublishStripeConnectAccountWebhookEventReceivedAsync(charge.Id, json, cancellationToken);
                    break;

                case EventTypes.CheckoutSessionCompleted:
                case EventTypes.CheckoutSessionExpired:
                case EventTypes.CheckoutSessionAsyncPaymentSucceeded:
                case EventTypes.CheckoutSessionAsyncPaymentFailed:
                    var session = stripeEvent.Data.Object as Session;
                    ArgumentNullException.ThrowIfNull(session);

                    await paymentInternalPublisher.PublishStripeConnectAccountWebhookEventReceivedAsync(session.Id, json, cancellationToken);
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

    public override async Task<IActionResult> RepublishAllOrganizationStripeConnectAccounts(CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishAllOrganizationStripeConnectAccountsAsync(cancellationToken);

        return Ok();
    }
}
