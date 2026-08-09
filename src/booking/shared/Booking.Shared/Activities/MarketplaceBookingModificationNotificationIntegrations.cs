using System.Net.Mail;
using Api.Shared.Services.Models;
using Booking.Shared.Configurations;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Email;
using Microsoft.Extensions.Logging;
using Temporalio.Activities;
using Temporalio.Exceptions;

namespace Booking.Shared.Activities;

public record DispatchMarketplaceBookingModificationNotificationInput(string ModificationId);

public class MarketplaceBookingModificationNotificationIntegrations(
    IRepositoryFactory repositoryFactory,
    IMarketplaceBookingModificationNotificationService notificationService,
    IEmailService emailService,
    EmailConfiguration emailConfiguration,
    IDbTransactionBuilder transactionBuilder,
    TimeProvider timeProvider,
    ILogger<MarketplaceBookingModificationNotificationIntegrations> logger)
{
    [Activity]
    public async Task DispatchMarketplaceBookingModificationAsync(DispatchMarketplaceBookingModificationNotificationInput input)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var modification = await repositoryFactory.MarketplaceBookingModificationRepository.GetByIdAsync(input.ModificationId, cancellationToken);
        if (modification is null)
        {
            logger.LogInformation(
                "Marketplace booking modification notification skipped because the modification was not found. ModificationId={ModificationId}",
                input.ModificationId);
            return;
        }

        logger.LogInformation(
            "Marketplace booking modification notification activity started. ModificationId={ModificationId}, DeliveryCount={DeliveryCount}, PendingDeliveryCount={PendingDeliveryCount}",
            modification.Id,
            modification.NotificationDeliveries.Count,
            modification.NotificationDeliveries.Count(item =>
                item.Status is MarketplaceBookingModificationNotificationDeliveryStatusConstants.Pending
                    or MarketplaceBookingModificationNotificationDeliveryStatusConstants.RecoveryRequired));

        var retryRequired = false;
        foreach (var delivery in modification.NotificationDeliveries.Where(item =>
                     item.Status is MarketplaceBookingModificationNotificationDeliveryStatusConstants.Pending or
                         MarketplaceBookingModificationNotificationDeliveryStatusConstants.RecoveryRequired))
        {
            delivery.AttemptCount++;
            delivery.LastAttemptAt = timeProvider.GetUtcNow();
            logger.LogInformation(
                "Marketplace booking modification notification delivery attempting. ModificationId={ModificationId}, DeliveryId={DeliveryId}, DeliveryKey={DeliveryKey}, Attempt={Attempt}, HasCustomerRecipient={HasCustomerRecipient}, HasEmailRecipient={HasEmailRecipient}, Status={Status}",
                modification.Id,
                delivery.Id,
                delivery.DeliveryKey,
                delivery.AttemptCount,
                !string.IsNullOrWhiteSpace(delivery.RecipientCustomerId),
                !string.IsNullOrWhiteSpace(delivery.RecipientEmail),
                delivery.Status);
            if (string.IsNullOrWhiteSpace(delivery.RecipientCustomerId) && string.IsNullOrWhiteSpace(delivery.RecipientEmail))
            {
                delivery.Status = MarketplaceBookingModificationNotificationDeliveryStatusConstants.RecoveryRequired;
                delivery.LastError = "A customer or email recipient must be assigned before delivery can be retried.";
                repositoryFactory.MarketplaceBookingModificationRepository.UpdateDelivery(delivery);
                continue;
            }

            var customer = delivery.RecipientCustomerId is not null
                ? await repositoryFactory.CustomerRepository.GetByIdAsync(delivery.RecipientCustomerId, true, cancellationToken)
                : null;
            var email = delivery.RecipientEmail ?? customer?.Identities
                .Select(identity => identity.Email)
                .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value) && IsValidEmailAddress(value!));
            if (string.IsNullOrWhiteSpace(email) || !IsValidEmailAddress(email))
            {
                delivery.Status = MarketplaceBookingModificationNotificationDeliveryStatusConstants.RecoveryRequired;
                delivery.LastError = "No verified email is available for the customer.";
                repositoryFactory.MarketplaceBookingModificationRepository.UpdateDelivery(delivery);
                logger.LogWarning(
                    "Marketplace booking modification notification requires recovery because no verified customer email is available. ModificationId={ModificationId}, Attempt={Attempt}",
                    modification.Id, delivery.AttemptCount);
                continue;
            }

            try
            {
                var message = await notificationService.RenderAsync(modification,
                    delivery.RecipientName ?? customer?.ToDisplayableName().Trim() ?? "there",
                    delivery.RecipientCustomerId is null, cancellationToken);
                await emailService.SendRawEmailAsync(
                    message.Subject,
                    message.Text,
                    message.Html,
                    emailConfiguration.BookingInvoiceEmailSender,
                    [email], [], [], [], cancellationToken);
                delivery.Status = MarketplaceBookingModificationNotificationDeliveryStatusConstants.Sent;
                delivery.SentAt = delivery.LastAttemptAt;
                delivery.LastError = null;
                logger.LogInformation("Marketplace booking modification notification delivered. ModificationId={ModificationId}, Attempt={Attempt}",
                    modification.Id, delivery.AttemptCount);
            }
            catch (Exception exception)
            {
                delivery.Status = MarketplaceBookingModificationNotificationDeliveryStatusConstants.RecoveryRequired;
                delivery.LastError = "Email delivery failed. Retry scheduled.";
                retryRequired = true;
                logger.LogWarning(
                    "Marketplace booking modification notification requires recovery. ModificationId={ModificationId}, Attempt={Attempt}, ErrorType={ErrorType}",
                    modification.Id, delivery.AttemptCount, exception.GetType().Name);
            }

            repositoryFactory.MarketplaceBookingModificationRepository.UpdateDelivery(delivery);
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        if (retryRequired)
        {
            throw new ApplicationFailureException(
                $"Marketplace booking modification notification delivery requires retry for {modification.Id}.");
        }
    }

    private static bool IsValidEmailAddress(string value)
    {
        try
        {
            _ = new MailAddress(value);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
