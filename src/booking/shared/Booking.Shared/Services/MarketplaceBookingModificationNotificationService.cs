using System.Globalization;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;

namespace Booking.Shared.Services;

public interface IMarketplaceBookingModificationNotificationService
{
    Task<(string Subject, string Text, string Html)> RenderAsync(
        MarketplaceBookingModification modification,
        string recipientName,
        bool organizationRecipient,
        CancellationToken cancellationToken);
}

public class MarketplaceBookingModificationNotificationService(IRepositoryFactory repositoryFactory)
    : IMarketplaceBookingModificationNotificationService
{
    public async Task<(string Subject, string Text, string Html)> RenderAsync(
        MarketplaceBookingModification modification,
        string recipientName,
        bool organizationRecipient,
        CancellationToken cancellationToken)
    {
        var (text, html) = await LoadAsync(
            $"Booking.Shared.EmailTemplates.MarketplaceBookingModification{(organizationRecipient ? "Organization" : "Customer")}.template",
            cancellationToken);
        var originalWindow = FormatWindow(modification.OriginalFrom, modification.OriginalUntil);
        var resultWindow = FormatWindow(modification.ResultFrom, modification.ResultUntil);
        var originalResources = await FormatResourcesAsync([.. modification.OriginalResourceIds], cancellationToken);
        var resultResources = await FormatResourcesAsync([.. modification.ResultResourceIds], cancellationToken);
        var reason = string.IsNullOrWhiteSpace(modification.Reason) ? "Not provided" : modification.Reason.Trim();

        return (
            organizationRecipient ? "A customer booking was updated" : "Your booking was updated",
            Apply(text, recipientName, originalWindow, resultWindow, reason, originalResources, resultResources),
            Apply(html, recipientName, originalWindow, resultWindow, reason, originalResources, resultResources));
    }

    private static async Task<(string Text, string Html)> LoadAsync(string templateBase, CancellationToken cancellationToken)
    {
        await using var htmlStream = typeof(MarketplaceBookingModificationNotificationService)
            .Assembly.GetManifestResourceStream($"{templateBase}.html");
        await using var textStream = typeof(MarketplaceBookingModificationNotificationService)
            .Assembly.GetManifestResourceStream($"{templateBase}.txt");
        ArgumentNullException.ThrowIfNull(htmlStream);
        ArgumentNullException.ThrowIfNull(textStream);
        using var htmlReader = new StreamReader(htmlStream);
        using var textReader = new StreamReader(textStream);
        return (await textReader.ReadToEndAsync(cancellationToken), await htmlReader.ReadToEndAsync(cancellationToken));
    }

    private static string Apply(string template, string recipientName, string originalWindow, string resultWindow, string reason,
        string originalResources, string resultResources) => template
        .Replace("{{RECIPIENT_NAME}}", recipientName)
        .Replace("{{ORIGINAL_WINDOW}}", originalWindow)
        .Replace("{{RESULT_WINDOW}}", resultWindow)
        .Replace("{{REASON}}", reason)
        .Replace("{{ORIGINAL_RESOURCES}}", originalResources)
        .Replace("{{RESULT_RESOURCES}}", resultResources);

    private async Task<string> FormatResourcesAsync(IReadOnlyList<string> resourceIds, CancellationToken cancellationToken)
    {
        if (resourceIds.Count == 0)
        {
            return "None";
        }

        var resources = await repositoryFactory.ResourceRepository.GetByIdsAsync(resourceIds, false, cancellationToken);
        return string.Join(", ", resourceIds.Select(id => resources.FirstOrDefault(resource => resource.Id == id)?.Name ?? "Resource unavailable"));
    }

    private static string FormatWindow(DateTimeOffset from, DateTimeOffset until) =>
        $"{from.ToString("MMMM d, yyyy h:mm tt", CultureInfo.InvariantCulture)} to {until.ToString("h:mm tt", CultureInfo.InvariantCulture)}";
}
