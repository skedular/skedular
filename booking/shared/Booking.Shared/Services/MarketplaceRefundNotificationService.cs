using System.Net.Mail;
using Api.Shared.Services.Models;
using Booking.Shared.Configurations;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared;
using Enterprise.Shared.Email;
using Organization = Booking.Shared.Database.Entities.Organization;

namespace Booking.Shared.Services;

public interface IMarketplaceRefundNotificationService
{
    Task NotifyStatusChangedAsync(MarketplaceRefund refund, CancellationToken cancellationToken);
}

public class MarketplaceRefundNotificationService(
    EmailConfiguration emailConfiguration,
    IRepositoryFactory repositoryFactory,
    IEmailService emailService) : IMarketplaceRefundNotificationService
{
    public async Task NotifyStatusChangedAsync(MarketplaceRefund refund, CancellationToken cancellationToken)
    {
        if (!ShouldNotify(refund.Status))
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
        }

        if (internalEmails.Count != 0)
        {
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
                internalEmails,
                [],
                [],
                [],
                cancellationToken);
        }
    }

    private static bool ShouldNotify(string status) =>
        status is MarketplaceRefundStatusConstants.Requested
            or MarketplaceRefundStatusConstants.PendingAccounting
            or MarketplaceRefundStatusConstants.ManualRequired
            or MarketplaceRefundStatusConstants.ManualCompleted
            or MarketplaceRefundStatusConstants.Completed
            or MarketplaceRefundStatusConstants.Failed;

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
            MarketplaceRefundStatusConstants.PendingAccounting => new RefundNotificationDetails(
                $"Refund approved and queued with {organization.Name ?? "Skedular"}",
                $"Refund queued for accounting for {entityLabel}",
                $"Your {entityLabel} refund has been approved locally and is queued for accounting processing.",
                $"A {entityLabel} refund has been approved locally and queued for accounting processing.",
                entityLabel,
                amountLabel,
                reference,
                note,
                error,
                refund.Status),
            MarketplaceRefundStatusConstants.ManualRequired => new RefundNotificationDetails(
                $"Refund needs manual follow-up with {organization.Name ?? "Skedular"}",
                $"Refund needs manual follow-up for {entityLabel}",
                BuildManualRequiredCustomerSummary(refund, entityLabel),
                BuildManualRequiredInternalSummary(refund, entityLabel),
                entityLabel,
                amountLabel,
                reference,
                note,
                error,
                refund.Status),
            MarketplaceRefundStatusConstants.ManualCompleted => new RefundNotificationDetails(
                $"Refund completed manually with {organization.Name ?? "Skedular"}",
                $"Refund completed manually for {entityLabel}",
                $"Your {entityLabel} refund has been completed manually by the team.",
                $"A {entityLabel} refund has been marked as completed manually.",
                entityLabel,
                amountLabel,
                reference,
                note,
                error,
                refund.Status),
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
                refund.Status)
        };
    }

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

    private static string BuildManualRequiredCustomerSummary(MarketplaceRefund refund, string entityLabel) =>
        string.IsNullOrWhiteSpace(refund.LastError)
            ? $"Your {entityLabel} refund requires manual follow-up from the team before it can complete."
            : $"Your {entityLabel} refund requires manual follow-up from the team before it can complete. Current issue: {refund.LastError}";

    private static string BuildManualRequiredInternalSummary(MarketplaceRefund refund, string entityLabel) =>
        string.IsNullOrWhiteSpace(refund.LastError)
            ? $"A {entityLabel} refund has been moved to manual follow-up."
            : $"A {entityLabel} refund has been moved to manual follow-up. Current issue: {refund.LastError}";

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

    private static ICollection<string> ResolveInternalEmails(
        Organization organization,
        string? customerEmail,
        ICollection<string> organizationSpecificEmails)
    {
        var emails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (!string.IsNullOrWhiteSpace(organization.ContactEmail) &&
            IsValidEmailAddress(organization.ContactEmail) &&
            !string.Equals(organization.ContactEmail, customerEmail, StringComparison.OrdinalIgnoreCase))
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
            if (!string.Equals(email, customerEmail, StringComparison.OrdinalIgnoreCase))
            {
                emails.Add(email);
            }
        }

        foreach (var email in organizationSpecificEmails)
        {
            if (!string.IsNullOrWhiteSpace(email) &&
                IsValidEmailAddress(email) &&
                !string.Equals(email, customerEmail, StringComparison.OrdinalIgnoreCase))
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
