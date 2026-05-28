using Enterprise.Shared.Email;
using Location.Shared.Configurations;
using Location.Shared.Repositories;
using Temporalio.Activities;

namespace Location.Shared.Activities;

public class EmailIntegrations(
    EmailConfiguration emailConfiguration,
    IRepositoryFactory repositoryFactory,
    IEmailService emailService)
{
    [Activity]
    public async Task SendNewLocationJoinedEmailAsync(string locationId)
    {
        if (!emailConfiguration.EnableNewLocationJoinedEmail)
        {
            return;
        }

        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var location = await repositoryFactory.LocationRepository.GetByIdUntrackedAsync(locationId, cancellationToken);
        if (location is null)
        {
            return;
        }

        await using var htmlTemplateStream =
            typeof(EmailIntegrations).Assembly.GetManifestResourceStream("Location.Shared.EmailTemplates.NewLocationJoined.template.html");
        ArgumentNullException.ThrowIfNull(htmlTemplateStream);
        using var htmlReader = new StreamReader(htmlTemplateStream);
        var html = await htmlReader.ReadToEndAsync(cancellationToken);

        await using var textTemplateStream =
            typeof(EmailIntegrations).Assembly.GetManifestResourceStream("Location.Shared.EmailTemplates.NewLocationJoined.template.txt");
        ArgumentNullException.ThrowIfNull(textTemplateStream);
        using var textReader = new StreamReader(textTemplateStream);
        var text = await textReader.ReadToEndAsync(cancellationToken);

        html = html
            .Replace("{{LOCATION_ID}}", location.Id)
            .Replace("{{LOCATION_NAME}}", location.Name);

        text = text
            .Replace("{{LOCATION_ID}}", location.Id)
            .Replace("{{LOCATION_NAME}}", location.Name);

        await emailService.SendRawEmailAsync(
            "New location has joined Skedular",
            text,
            html,
            emailConfiguration.NewLocationJoinedEmailSender,
            emailConfiguration.NewLocationJoinedEmailReceivers,
            [],
            [],
            [],
            cancellationToken);
    }
}
