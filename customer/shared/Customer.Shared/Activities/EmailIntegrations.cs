using Api.Shared.Services.Models;
using Customer.Shared.Configurations;
using Customer.Shared.Models;
using Customer.Shared.Repositories;
using Enterprise.Shared.Email;
using Temporalio.Activities;

namespace Customer.Shared.Activities;

public class EmailIntegrations(EmailConfiguration emailConfiguration, IRepositoryFactory repositoryFactory, IEmailService emailService)
{
    [Activity]
    public async Task SendCustomerFeedbackReceivedEmailAsync(string customerFeedbackId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var customerFeedback = await repositoryFactory.CustomerFeedbackRepository.GetByIdAsync(customerFeedbackId, cancellationToken);
        if (customerFeedback is null)
        {
            return;
        }

        await using var htmlTemplateStream = typeof(EmailIntegrations).Assembly.GetManifestResourceStream(
            "Customer.Shared.EmailTemplates.CustomerFeedbackReceived.template.html");
        ArgumentNullException.ThrowIfNull(htmlTemplateStream);
        using var htmlReader = new StreamReader(htmlTemplateStream);
        var html = await htmlReader.ReadToEndAsync(cancellationToken);

        await using var textTemplateStream = typeof(EmailIntegrations).Assembly.GetManifestResourceStream(
            "Customer.Shared.EmailTemplates.CustomerFeedbackReceived.template.txt");
        ArgumentNullException.ThrowIfNull(textTemplateStream);
        using var textReader = new StreamReader(textTemplateStream);
        var text = await textReader.ReadToEndAsync(cancellationToken);

        var channel = customerFeedback.Channel switch
        {
            FeedbackChannelTypeConstants.Web => "Web",
            FeedbackChannelTypeConstants.Slack => "Slack",
            FeedbackChannelTypeConstants.MsTeams => "MsTeams",
            _ => throw new ArgumentOutOfRangeException()
        };

        html = html
            .Replace("{{CHANNEL}}", channel)
            .Replace("{{CUSTOMER_NAME}}", customerFeedback.Customer.ToDisplayableName())
            .Replace("{{EMAILS}}", string.Join(',', customerFeedback.Customer.Identities.ToStringEmails()))
            .Replace("{{FEEDBACK_CONTENT}}", string.IsNullOrWhiteSpace(customerFeedback.Content) ? string.Empty : customerFeedback.Content);

        text = text
            .Replace("{{CHANNEL}}", channel)
            .Replace("{{CUSTOMER_NAME}}", customerFeedback.Customer.ToDisplayableName())
            .Replace("{{EMAILS}}", string.Join(',', customerFeedback.Customer.Identities.ToStringEmails()))
            .Replace("{{FEEDBACK_CONTENT}}", string.IsNullOrWhiteSpace(customerFeedback.Content) ? string.Empty : customerFeedback.Content);

        var subject = $"New Customer Feedback Received through {channel}";

        await emailService.SendRawEmailAsync(
            subject,
            text,
            html,
            $"Skedular {emailConfiguration.NewCustomerFeedbackSubmittedEmailSender}",
            emailConfiguration.NewCustomerFeedbackSubmittedEmailReceivers,
            [],
            [],
            [],
            cancellationToken);
    }
}
