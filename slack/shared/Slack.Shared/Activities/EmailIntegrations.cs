using Enterprise.Shared.Email;
using Slack.Shared.Configurations;
using Slack.Shared.Repositories;
using Temporalio.Activities;

namespace Slack.Shared.Activities;

public class EmailIntegrations(EmailConfiguration emailConfiguration, IRepositoryFactory repositoryFactory, IEmailService emailService)
{
    [Activity]
    public async Task SendNewSlackWorkspaceJoinedEmailAsync(string workspaceId)
    {
        if (!emailConfiguration.EnableNewSlackWorkspaceJoinedEmail)
        {
            return;
        }

        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var workspace = await repositoryFactory.WorkspaceRepository.GetByIdAsync(workspaceId, cancellationToken);
        if (workspace is null)
        {
            return;
        }

        await using var htmlTemplateStream =
            typeof(EmailIntegrations).Assembly.GetManifestResourceStream("Slack.Shared.EmailTemplates.NewSlackWorkspaceJoined.template.html");
        ArgumentNullException.ThrowIfNull(htmlTemplateStream);
        using var htmlReader = new StreamReader(htmlTemplateStream);
        var html = await htmlReader.ReadToEndAsync(cancellationToken);

        await using var textTemplateStream =
            typeof(EmailIntegrations).Assembly.GetManifestResourceStream("Slack.Shared.EmailTemplates.NewSlackWorkspaceJoined.template.txt");
        ArgumentNullException.ThrowIfNull(textTemplateStream);
        using var textReader = new StreamReader(textTemplateStream);
        var text = await textReader.ReadToEndAsync(cancellationToken);

        html = html
            .Replace("{{WORKSPACE_ID}}", workspace.Id)
            .Replace("{{WORKSPACE_NAME}}", workspace.Name);

        text = text
            .Replace("{{WORKSPACE_ID}}", workspace.Id)
            .Replace("{{WORKSPACE_NAME}}", workspace.Name);

        await emailService.SendRawEmailAsync(
            "New customer has joined Skedular",
            text,
            html,
            $"Skedular {emailConfiguration.NewSlackWorkspaceJoinedEmailSender}",
            emailConfiguration.NewSlackWorkspaceJoinedEmailReceivers,
            [],
            [],
            [],
            cancellationToken);
    }
}
