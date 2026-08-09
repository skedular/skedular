using System.Net.Mail;
using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Customer = Booking.Shared.Database.Entities.Customer;
using MarketplaceBookingFailure = Booking.Shared.Database.Entities.MarketplaceBookingFailure;

namespace Booking.Shared.Services;

public interface IMarketplaceBookingFailureNotificationService
{
    Task<IReadOnlyCollection<MarketplaceBookingFailureRecipient>> ResolveRecipientsAsync(
        MarketplaceBookingFailure failure,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<MarketplaceBookingFailureRecipient>> ResolveRecipientsAsync(
        Customer? customer,
        IReadOnlyCollection<string> organizationIds,
        CancellationToken cancellationToken);

    Task<(string Subject, string Text, string Html)> RenderAsync(
        MarketplaceBookingFailure failure,
        bool internalAudience,
        string recipientName,
        CancellationToken cancellationToken);
}

public class MarketplaceBookingFailureNotificationService(IRepositoryFactory repositoryFactory) : IMarketplaceBookingFailureNotificationService
{
    public async Task<IReadOnlyCollection<MarketplaceBookingFailureRecipient>> ResolveRecipientsAsync(
        MarketplaceBookingFailure failure,
        CancellationToken cancellationToken)
    {
        var recipientSource = await ResolveRecipientSourceAsync(failure, cancellationToken);
        if (recipientSource is null)
        {
            return [];
        }

        return await ResolveRecipientsAsync(
            recipientSource.CreatedByCustomer,
            recipientSource.OrganizationIds,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<MarketplaceBookingFailureRecipient>> ResolveRecipientsAsync(
        Customer? customer,
        IReadOnlyCollection<string> organizationIds,
        CancellationToken cancellationToken)
    {
        var recipients = new List<MarketplaceBookingFailureRecipient>();
        if (customer is not null)
        {
            recipients.Add(new MarketplaceBookingFailureRecipient(
                customer.Id,
                MarketplaceBookingFailureDeliveryAudienceConstants.Customer,
                MarketplaceBookingFailureDeliveryChannelConstants.InApplication,
                customer.Id,
                null));

            foreach (var email in GetVerifiedEmails(customer))
            {
                recipients.Add(new MarketplaceBookingFailureRecipient(
                    email,
                    MarketplaceBookingFailureDeliveryAudienceConstants.Customer,
                    MarketplaceBookingFailureDeliveryChannelConstants.Email,
                    customer.Id,
                    email));
            }
        }

        foreach (var organizationId in organizationIds.Distinct())
        {
            var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                organizationId,
                null,
                false,
                false,
                cancellationToken);
            if (organization is null)
            {
                continue;
            }

            var audience = organization.Type == OrganizationTypeConstants.Host
                ? MarketplaceBookingFailureDeliveryAudienceConstants.HostStakeholder
                : MarketplaceBookingFailureDeliveryAudienceConstants.SpacesStakeholder;
            foreach (var member in organization.OrganizationMembers.Where(item =>
                         item.Status == OrganizationMemberStatusConstants.Active &&
                         item.Role is OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator))
            {
                recipients.Add(new MarketplaceBookingFailureRecipient(
                    member.CustomerId,
                    audience,
                    MarketplaceBookingFailureDeliveryChannelConstants.InApplication,
                    member.CustomerId,
                    null));
                foreach (var email in GetVerifiedEmails(member.Customer))
                {
                    recipients.Add(new MarketplaceBookingFailureRecipient(
                        email,
                        audience,
                        MarketplaceBookingFailureDeliveryChannelConstants.Email,
                        member.CustomerId,
                        email));
                }
            }
        }

        return
        [
            .. recipients
                .GroupBy(item => $"{item.RecipientKey}\u001f{item.Channel}", StringComparer.OrdinalIgnoreCase)
                .Select(item => item.First()),
        ];
    }

    public async Task<(string Subject, string Text, string Html)> RenderAsync(
        MarketplaceBookingFailure failure,
        bool internalAudience,
        string recipientName,
        CancellationToken cancellationToken)
    {
        var audience = internalAudience ? "Internal" : "Customer";
        var templateBase = $"Booking.Shared.EmailTemplates.BookingFailure{audience}.template";
        var (text, html) = await LoadAsync(templateBase, cancellationToken);
        var summary = failure.Category switch
        {
            "AvailabilityConflict" => internalAudience
                ? "A booking could not be confirmed because the requested capacity is no longer available."
                : "We could not confirm your booking because the requested time is no longer available.",
            "PaymentExpired" => internalAudience
                ? "A booking payment expired and its capacity was released."
                : "Your booking payment expired, so the reserved capacity was released.",
            _ => internalAudience
                ? "A booking payment could not be completed and its capacity was released."
                : "We could not complete your booking payment, so the reserved capacity was released.",
        };
        var action = failure.CustomerAction == "Rebook" ? "start a new booking" : "review your subscription";
        return (
            internalAudience ? "Booking needs attention" : "Your booking could not be completed",
            Apply(text, recipientName, summary, action),
            Apply(html, recipientName, summary, action));
    }

    private async Task<RecipientSource?> ResolveRecipientSourceAsync(
        MarketplaceBookingFailure failure,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(failure.BookingId))
        {
            var booking = await repositoryFactory.BookingRepository.GetByIdAsync(failure.BookingId, cancellationToken);
            if (booking is not null)
            {
                return new RecipientSource(
                    booking.CreatedByCustomer,
                    [.. booking.InvolvedOrganizations.Select(item => item.Id).Distinct()]);
            }
        }

        if (!string.IsNullOrWhiteSpace(failure.RecurringBookingId))
        {
            var recurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(
                failure.RecurringBookingId,
                cancellationToken);
            if (recurringBooking is not null)
            {
                return new RecipientSource(
                    recurringBooking.CreatedByCustomer ?? recurringBooking.InvolvedCustomers.FirstOrDefault(),
                    [.. recurringBooking.InvolvedOrganizations.Select(item => item.Id).Distinct()]);
            }
        }

        return null;
    }

    private static async Task<(string Text, string Html)> LoadAsync(string templateBase, CancellationToken cancellationToken)
    {
        await using var htmlStream = typeof(MarketplaceBookingFailureNotificationService).Assembly.GetManifestResourceStream($"{templateBase}.html");
        await using var textStream = typeof(MarketplaceBookingFailureNotificationService).Assembly.GetManifestResourceStream($"{templateBase}.txt");
        ArgumentNullException.ThrowIfNull(htmlStream);
        ArgumentNullException.ThrowIfNull(textStream);
        using var htmlReader = new StreamReader(htmlStream);
        using var textReader = new StreamReader(textStream);
        return (await textReader.ReadToEndAsync(cancellationToken), await htmlReader.ReadToEndAsync(cancellationToken));
    }

    private static string Apply(string template, string recipientName, string summary, string action) => template
        .Replace("{{RECIPIENT_NAME}}", recipientName)
        .Replace("{{SUMMARY}}", summary)
        .Replace("{{CUSTOMER_ACTION}}", action);

    private static IEnumerable<string> GetVerifiedEmails(Customer customer) => customer.Identities
        .Where(item => item.EmailVerified == true && !string.IsNullOrWhiteSpace(item.Email) && IsValidEmailAddress(item.Email!))
        .Select(item => item.Email!);

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

    private sealed record RecipientSource(
        Customer? CreatedByCustomer,
        IReadOnlyList<string> OrganizationIds);
}
