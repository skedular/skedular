using Booking.Shared.Configurations;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Email;
using Enterprise.Shared.Random;
using Microsoft.Extensions.Logging;
using Temporalio.Activities;
using Temporalio.Exceptions;
using MarketplaceBookingFailure = Booking.Shared.Database.Entities.MarketplaceBookingFailure;

namespace Booking.Shared.Activities;

public record DispatchMarketplaceBookingFailureNotificationsInput(string FailureId);

public class MarketplaceBookingFailureNotificationIntegrations(
    IRepositoryFactory repositoryFactory,
    IMarketplaceBookingFailureNotificationService notificationService,
    IEmailService emailService,
    EmailConfiguration emailConfiguration,
    IDbTransactionBuilder transactionBuilder,
    TimeProvider timeProvider,
    IRandomHelper randomHelper,
    ILogger<MarketplaceBookingFailureNotificationIntegrations> logger)
{
    [Activity]
    public async Task DispatchMarketplaceBookingFailureAsync(DispatchMarketplaceBookingFailureNotificationsInput input)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var failure = await repositoryFactory.MarketplaceBookingFailureRepository.GetByIdAsync(input.FailureId, cancellationToken);
        if (failure is null)
        {
            logger.LogInformation("Marketplace booking failure dispatch skipped because the failure was not found. FailureId={FailureId}",
                input.FailureId);
            return;
        }

        logger.LogInformation(
            "Dispatching marketplace booking failure notifications. FailureId={FailureId}, Category={Category}, Scope={Scope}, CorrelationId={CorrelationId}",
            failure.Id,
            failure.Category,
            failure.Scope,
            failure.CorrelationId);

        Exception? deliveryFailure = null;
        foreach (var delivery in failure.Deliveries.Where(item =>
                     item.Status is MarketplaceBookingFailureDeliveryStatusConstants.Pending
                         or MarketplaceBookingFailureDeliveryStatusConstants.Failed))
        {
            delivery.AttemptCount++;
            delivery.LastAttemptAt = timeProvider.GetUtcNow();
            if (delivery.Channel == MarketplaceBookingFailureDeliveryChannelConstants.InApplication)
            {
                delivery.Status = MarketplaceBookingFailureDeliveryStatusConstants.Sent;
                delivery.SentAt = delivery.LastAttemptAt;
                repositoryFactory.MarketplaceBookingFailureDeliveryRepository.Update(delivery);
                AppendDeliveryEvent(failure, delivery, true, null);
                continue;
            }

            if (string.IsNullOrWhiteSpace(delivery.RecipientEmail))
            {
                delivery.Status = MarketplaceBookingFailureDeliveryStatusConstants.Skipped;
                repositoryFactory.MarketplaceBookingFailureDeliveryRepository.Update(delivery);
                logger.LogWarning(
                    "Marketplace booking failure email delivery skipped because no verified email was retained. FailureId={FailureId}, Audience={Audience}",
                    failure.Id, delivery.Audience);
                continue;
            }

            try
            {
                var message = await notificationService.RenderAsync(failure,
                    delivery.Audience != MarketplaceBookingFailureDeliveryAudienceConstants.Customer, "there", cancellationToken);
                await emailService.SendRawEmailAsync(message.Subject, message.Text, message.Html, emailConfiguration.BookingInvoiceEmailSender,
                    [delivery.RecipientEmail], [], [], [],
                    cancellationToken);
                delivery.Status = MarketplaceBookingFailureDeliveryStatusConstants.Sent;
                delivery.SentAt = delivery.LastAttemptAt;
                delivery.LastError = null;
                AppendDeliveryEvent(failure, delivery, true, null);
                logger.LogInformation(
                    "Marketplace booking failure delivery succeeded. FailureId={FailureId}, Audience={Audience}, Channel={Channel}, Attempt={Attempt}",
                    failure.Id, delivery.Audience, delivery.Channel, delivery.AttemptCount);
            }
            catch (Exception exception)
            {
                delivery.Status = MarketplaceBookingFailureDeliveryStatusConstants.Failed;
                delivery.LastError = exception.Message;
                AppendDeliveryEvent(failure, delivery, false, exception.Message);
                logger.LogWarning(exception,
                    "Marketplace booking failure delivery failed and will be retried. FailureId={FailureId}, Audience={Audience}, Channel={Channel}, Attempt={Attempt}",
                    failure.Id, delivery.Audience, delivery.Channel, delivery.AttemptCount);
                deliveryFailure ??= exception;
            }

            repositoryFactory.MarketplaceBookingFailureDeliveryRepository.Update(delivery);
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        // Persist the failed delivery before failing the activity so Temporal schedules the
        // next attempt. Sent deliveries are skipped on replay, making this retry idempotent.
        if (deliveryFailure is not null)
        {
            throw new ApplicationFailureException(
                $"Marketplace booking failure notification delivery failed for {failure.Id}: {deliveryFailure.Message}");
        }
    }

    private void AppendDeliveryEvent(
        MarketplaceBookingFailure failure,
        MarketplaceBookingFailureDelivery delivery,
        bool succeeded,
        string? error) => repositoryFactory.MarketplaceBookingFailureEventRepository.Add(new MarketplaceBookingFailureEvent
    {
        Id = randomHelper.Generate(),
        MarketplaceBookingFailureId = failure.Id,
        EventType = succeeded
            ? MarketplaceBookingFailureEventTypeConstants.DeliverySucceeded
            : MarketplaceBookingFailureEventTypeConstants.DeliveryFailed,
        OccurredAt = delivery.LastAttemptAt ?? timeProvider.GetUtcNow(),
        Reason = $"{delivery.Audience}:{delivery.Channel}",
        LastError = error,
    });
}
