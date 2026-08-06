using System.Net.Mail;
using Api.Shared.Services.Models;
using Booking.Shared.Configurations;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared;
using Enterprise.Shared.Email;
using Microsoft.EntityFrameworkCore;
using Organization = Booking.Shared.Database.Entities.Organization;

namespace Booking.Shared.Services;

public interface IMarketplaceRefundNotificationService
{
    Task NotifyStatusChangedAsync(MarketplaceRefund refund, CancellationToken cancellationToken);
}

public class MarketplaceRefundNotificationService(
    EmailConfiguration emailConfiguration,
    IRepositoryFactory repositoryFactory,
    IEmailService emailService,
    TimeProvider timeProvider) : IMarketplaceRefundNotificationService
{
    public async Task NotifyStatusChangedAsync(MarketplaceRefund refund, CancellationToken cancellationToken)
    {
        if (!ShouldNotify(refund.Status))
        {
            return;
        }

        if (string.Equals(refund.LastNotificationStatus, refund.Status, StringComparison.Ordinal))
        {
            return;
        }

        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
            refund.OrganizationId,
            null,
            false,
            false,
            cancellationToken);
        if (organization is null)
        {
            return;
        }

        var customer = string.IsNullOrWhiteSpace(refund.RequestedByCustomerId)
            ? null
            : await repositoryFactory.CustomerRepository.GetByIdAsync(refund.RequestedByCustomerId, true, cancellationToken);

        var customerEmail = customer?.Identities
            .Where(item => !string.IsNullOrWhiteSpace(item.Email) && item.EmailVerified == true && IsValidEmailAddress(item.Email!))
            .Select(item => item.Email!)
            .FirstOrDefault();
        var internalEmails = ResolveInternalEmails(
            organization,
            customerEmail,
            organization.RefundNotificationEmails.ToSafeCollection());

        var notificationDetails = BuildNotificationDetails(refund, organization);
        var eventType = refund.Status;

        if (!string.IsNullOrWhiteSpace(customerEmail))
        {
            var customerDelivery = await repositoryFactory.MarketplaceRefundRepository
                .GetNotificationDeliveryAsync(refund.Id, eventType, customerEmail, cancellationToken);
            if (customerDelivery is not null && customerDelivery.Status == "Sent")
            {
                customerEmail = null;
            }
            else if (customerDelivery is not null && customerDelivery.Status == "Sending" &&
                     customerDelivery.ModifiedAt > timeProvider.GetUtcNow().AddMinutes(-10))
            {
                customerEmail = null;
            }
            else if (customerDelivery is null)
            {
                repositoryFactory.MarketplaceRefundRepository.AddNotificationDelivery(new MarketplaceRefundNotificationDelivery
                {
                    Id = Guid.NewGuid().ToString("N"),
                    MarketplaceRefundId = refund.Id,
                    EventType = eventType,
                    RecipientId = customerEmail,
                    Status = "Sending",
                    AttemptCount = 1,
                });
            }
            else
            {
                customerDelivery.Status = "Sending";
                customerDelivery.AttemptCount++;
            }

            if (!string.IsNullOrWhiteSpace(customerEmail))
            {
                try
                {
                    await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateException)
                {
                    // Another worker claimed this exact delivery key first.
                    customerEmail = null;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(customerEmail))
        {
            var recipientName = customer?.ToDisplayableName().Trim() ?? "there";
            var (text, html) = await LoadTemplatesAsync(
                "Booking.Shared.EmailTemplates.RefundStatusCustomer.template",
                cancellationToken);
            text = ApplyTemplateValues(text, recipientName, notificationDetails, false);
            html = ApplyTemplateValues(html, recipientName, notificationDetails, false);
            await emailService.SendRawEmailAsync(
                notificationDetails.CustomerSubject,
                text,
                html,
                $"{organization.Name ?? "Skedular"} {emailConfiguration.BookingInvoiceEmailSender}",
                [customerEmail],
                [],
                [],
                [],
                cancellationToken);
            var sentCustomerDelivery = await repositoryFactory.MarketplaceRefundRepository
                .GetNotificationDeliveryAsync(refund.Id, eventType, customerEmail, cancellationToken);
            if (sentCustomerDelivery is null)
            {
                repositoryFactory.MarketplaceRefundRepository.AddNotificationDelivery(new MarketplaceRefundNotificationDelivery
                {
                    Id = Guid.NewGuid().ToString("N"),
                    MarketplaceRefundId = refund.Id,
                    EventType = eventType,
                    RecipientId = customerEmail,
                    Status = "Sent",
                    AttemptCount = 1,
                    SentAt = timeProvider.GetUtcNow(),
                });
            }
            else
            {
                sentCustomerDelivery.Status = "Sent";
                sentCustomerDelivery.SentAt = timeProvider.GetUtcNow();
                sentCustomerDelivery.AttemptCount++;
            }
        }

        var pendingInternalEmails = new List<string>();
        foreach (var internalEmail in internalEmails)
        {
            var internalDelivery = await repositoryFactory.MarketplaceRefundRepository
                .GetNotificationDeliveryAsync(refund.Id, eventType, internalEmail, cancellationToken);
            if (internalDelivery is not null && internalDelivery.Status == "Sent")
            {
                continue;
            }

            pendingInternalEmails.Add(internalEmail);
            if (internalDelivery is null)
            {
                repositoryFactory.MarketplaceRefundRepository.AddNotificationDelivery(new MarketplaceRefundNotificationDelivery
                {
                    Id = Guid.NewGuid().ToString("N"),
                    MarketplaceRefundId = refund.Id,
                    EventType = eventType,
                    RecipientId = internalEmail,
                    Status = "Sending",
                    AttemptCount = 1,
                });
            }
            else
            {
                internalDelivery.Status = "Sending";
                internalDelivery.AttemptCount++;
            }
        }

        if (pendingInternalEmails.Count != 0)
        {
            try
            {
                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                pendingInternalEmails.Clear();
            }

            if (pendingInternalEmails.Count == 0)
            {
                return;
            }

            var (text, html) = await LoadTemplatesAsync(
                "Booking.Shared.EmailTemplates.RefundStatusInternal.template",
                cancellationToken);
            text = ApplyTemplateValues(text, organization.Name ?? "team", notificationDetails, true);
            html = ApplyTemplateValues(html, organization.Name ?? "team", notificationDetails, true);
            await emailService.SendRawEmailAsync(
                notificationDetails.InternalSubject,
                text,
                html,
                $"{organization.Name ?? "Skedular"} {emailConfiguration.BookingInvoiceEmailSender}",
                pendingInternalEmails,
                [],
                [],
                [],
                cancellationToken);
            foreach (var internalEmail in pendingInternalEmails)
            {
                var internalDelivery = await repositoryFactory.MarketplaceRefundRepository
                    .GetNotificationDeliveryAsync(refund.Id, eventType, internalEmail, cancellationToken);
                if (internalDelivery is null)
                {
                    repositoryFactory.MarketplaceRefundRepository.AddNotificationDelivery(new MarketplaceRefundNotificationDelivery
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        MarketplaceRefundId = refund.Id,
                        EventType = eventType,
                        RecipientId = internalEmail,
                        Status = "Sent",
                        AttemptCount = 1,
                        SentAt = timeProvider.GetUtcNow(),
                    });
                }
                else
                {
                    internalDelivery.Status = "Sent";
                    internalDelivery.SentAt = timeProvider.GetUtcNow();
                    internalDelivery.AttemptCount++;
                }
            }
        }

        refund.LastNotificationStatus = refund.Status;
        repositoryFactory.MarketplaceRefundRepository.Update(refund);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static bool ShouldNotify(string status) =>
        status is MarketplaceRefundStatusConstants.Requested
            or MarketplaceRefundStatusConstants.UnderReview
            or MarketplaceRefundStatusConstants.Approved
            or MarketplaceRefundStatusConstants.Rejected
            or MarketplaceRefundStatusConstants.ProviderPending
            or MarketplaceRefundStatusConstants.Processing
            or MarketplaceRefundStatusConstants.Completed
            or MarketplaceRefundStatusConstants.Failed
            or MarketplaceRefundStatusConstants.Cancelled
            or MarketplaceRefundStatusConstants.ReconciliationRequired;

    private static RefundNotificationDetails BuildNotificationDetails(MarketplaceRefund refund, Organization organization)
    {
        var entityLabel = refund.LocalEntityType == MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription ? "subscription" : "booking";
        var amountLabel = refund.RefundAmount.HasValue && !string.IsNullOrWhiteSpace(refund.Currency)
            ? $"{refund.RefundAmount.Value} {refund.Currency}"
            : refund.RefundAmount?.ToString() ?? "Not set";
        var reference = string.IsNullOrWhiteSpace(refund.ExternalRefundNumber) ? "Not available" : refund.ExternalRefundNumber;
        var note = string.IsNullOrWhiteSpace(refund.Reason) ? "Not provided" : refund.Reason;
        var error = string.IsNullOrWhiteSpace(refund.LastError) ? "None" : refund.LastError;

        return refund.Status switch
        {
            MarketplaceRefundStatusConstants.Requested => new RefundNotificationDetails(
                $"Refund requested with {organization.Name ?? "Skedular"}",
                $"Refund requested for {entityLabel}",
                $"Your {entityLabel} refund request is now under review.",
                $"A {entityLabel} refund has been requested and is awaiting review.",
                entityLabel,
                amountLabel,
                reference,
                note,
                error,
                refund.Status),
            MarketplaceRefundStatusConstants.UnderReview or MarketplaceRefundStatusConstants.Approved
                or MarketplaceRefundStatusConstants.ProviderPending or MarketplaceRefundStatusConstants.Processing
                => BuildInProgressDetails(refund, organization, entityLabel, amountLabel, reference, note, error),
            MarketplaceRefundStatusConstants.Rejected or MarketplaceRefundStatusConstants.Cancelled
                => BuildClosedDetails(refund, organization, entityLabel, amountLabel, reference, note, error),
            MarketplaceRefundStatusConstants.ReconciliationRequired
                => BuildReconciliationDetails(refund, organization, entityLabel, amountLabel, reference, note, error),
            MarketplaceRefundStatusConstants.Completed => new RefundNotificationDetails(
                $"Refund completed with {organization.Name ?? "Skedular"}",
                $"Refund completed for {entityLabel}",
                BuildCompletedCustomerSummary(refund, entityLabel),
                BuildCompletedInternalSummary(refund, entityLabel),
                entityLabel,
                amountLabel,
                reference,
                note,
                error,
                refund.Status),
            _ => new RefundNotificationDetails(
                $"Refund update needs attention with {organization.Name ?? "Skedular"}",
                $"Refund failed for {entityLabel}",
                BuildFailedCustomerSummary(refund, entityLabel),
                BuildFailedInternalSummary(refund, entityLabel),
                entityLabel,
                amountLabel,
                reference,
                note,
                error,
                refund.Status),
        };
    }

    private static RefundNotificationDetails BuildInProgressDetails(MarketplaceRefund refund, Organization organization, string entityLabel,
        string amount, string reference, string note, string error) =>
        new($"Refund update with {organization.Name ?? "Skedular"}", $"Refund update for {entityLabel}",
            $"Your {entityLabel} refund is still being processed.", $"A {entityLabel} refund is still being processed.", entityLabel, amount,
            reference, note, error, refund.Status);

    private static RefundNotificationDetails BuildClosedDetails(MarketplaceRefund refund, Organization organization, string entityLabel,
        string amount, string reference, string note, string error) =>
        new($"Refund update with {organization.Name ?? "Skedular"}", $"Refund closed for {entityLabel}",
            $"Your {entityLabel} refund was {ToClosedStatusLabel(refund.Status)}.",
            $"A {entityLabel} refund was {ToClosedStatusLabel(refund.Status)}.", entityLabel, amount, reference, note, error, refund.Status);

    private static string ToClosedStatusLabel(string status) =>
        status == MarketplaceRefundStatusConstants.Cancelled ? "canceled" : status.ToLowerInvariant();

    private static RefundNotificationDetails BuildReconciliationDetails(MarketplaceRefund refund, Organization organization, string entityLabel,
        string amount, string reference, string note, string error) =>
        new($"Refund needs reconciliation with {organization.Name ?? "Skedular"}", $"Refund needs reconciliation for {entityLabel}",
            $"Your {entityLabel} refund needs additional payment-provider reconciliation.",
            $"A {entityLabel} refund needs payment-provider reconciliation.", entityLabel, amount, reference, note, error, refund.Status);

    private static string BuildCompletedCustomerSummary(MarketplaceRefund refund, string entityLabel) =>
        string.IsNullOrWhiteSpace(refund.AccountingProvider)
            ? $"Your {entityLabel} refund has been completed and recorded locally."
            : $"Your {entityLabel} refund has been completed through {refund.AccountingProvider}.";

    private static string BuildCompletedInternalSummary(MarketplaceRefund refund, string entityLabel) =>
        string.IsNullOrWhiteSpace(refund.AccountingProvider)
            ? $"A {entityLabel} refund has been completed locally without a provider reference."
            : $"A {entityLabel} refund has completed through {refund.AccountingProvider}.";

    private static string BuildFailedCustomerSummary(MarketplaceRefund refund, string entityLabel) =>
        string.IsNullOrWhiteSpace(refund.LastError)
            ? $"Your {entityLabel} refund needs manual follow-up before it can complete."
            : $"Your {entityLabel} refund needs manual follow-up before it can complete. Current issue: {refund.LastError}";

    private static string BuildFailedInternalSummary(MarketplaceRefund refund, string entityLabel) =>
        string.IsNullOrWhiteSpace(refund.LastError)
            ? $"A {entityLabel} refund failed and needs manual follow-up."
            : $"A {entityLabel} refund failed and needs manual follow-up. Current issue: {refund.LastError}";

    private static async Task<(string Text, string Html)> LoadTemplatesAsync(string baseResourceName, CancellationToken cancellationToken)
    {
        await using var htmlTemplateStream =
            typeof(MarketplaceRefundNotificationService).Assembly.GetManifestResourceStream($"{baseResourceName}.html");
        ArgumentNullException.ThrowIfNull(htmlTemplateStream);
        using var htmlReader = new StreamReader(htmlTemplateStream);
        var html = await htmlReader.ReadToEndAsync(cancellationToken);

        await using var textTemplateStream =
            typeof(MarketplaceRefundNotificationService).Assembly.GetManifestResourceStream($"{baseResourceName}.txt");
        ArgumentNullException.ThrowIfNull(textTemplateStream);
        using var textReader = new StreamReader(textTemplateStream);
        var text = await textReader.ReadToEndAsync(cancellationToken);

        return (text, html);
    }

    private static string ApplyTemplateValues(
        string template,
        string recipientName,
        RefundNotificationDetails details,
        bool includeInternalContext) =>
        template
            .Replace("{{RECIPIENT_NAME}}", recipientName)
            .Replace("{{SUMMARY}}", includeInternalContext ? details.InternalSummary : details.CustomerSummary)
            .Replace("{{ENTITY_LABEL}}", details.EntityLabel)
            .Replace("{{STATUS}}", details.Status)
            .Replace("{{AMOUNT}}", details.Amount)
            .Replace("{{REFERENCE}}", details.Reference)
            .Replace("{{NOTE}}", details.Note)
            .Replace("{{ERROR}}", details.Error)
            .Replace(
                "{{INTERNAL_CONTEXT}}",
                includeInternalContext
                    ? $"Accounting follow-up: review the current refund status for this {details.EntityLabel} in the admin panel."
                    : string.Empty);

    private static IReadOnlyList<string> ResolveInternalEmails(
        Organization organization,
        string? customerEmail,
        IReadOnlyList<string> organizationSpecificEmails)
    {
        var emails = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        if (!string.IsNullOrWhiteSpace(organization.ContactEmail) &&
            IsValidEmailAddress(organization.ContactEmail) &&
            !string.Equals(organization.ContactEmail, customerEmail, StringComparison.InvariantCultureIgnoreCase))
        {
            emails.Add(organization.ContactEmail);
        }

        foreach (var email in organization.OrganizationMembers
                     .Where(item =>
                         item.Status == OrganizationMemberStatusConstants.Active &&
                         item.Role is OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator)
                     .SelectMany(item => item.Customer.Identities)
                     .Where(item => !string.IsNullOrWhiteSpace(item.Email) && item.EmailVerified == true && IsValidEmailAddress(item.Email!))
                     .Select(item => item.Email!))
        {
            if (!string.Equals(email, customerEmail, StringComparison.InvariantCultureIgnoreCase))
            {
                emails.Add(email);
            }
        }

        foreach (var email in organizationSpecificEmails)
        {
            if (!string.IsNullOrWhiteSpace(email) &&
                IsValidEmailAddress(email) &&
                !string.Equals(email, customerEmail, StringComparison.InvariantCultureIgnoreCase))
            {
                emails.Add(email);
            }
        }

        return emails.ToList();
    }

    private static bool IsValidEmailAddress(string value)
    {
        try
        {
            _ = new MailAddress(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private sealed record RefundNotificationDetails(
        string CustomerSubject,
        string InternalSubject,
        string CustomerSummary,
        string InternalSummary,
        string EntityLabel,
        string Amount,
        string Reference,
        string Note,
        string Error,
        string Status);
}
