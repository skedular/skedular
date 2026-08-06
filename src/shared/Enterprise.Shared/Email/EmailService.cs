using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Enterprise.Shared.Email;

public interface IEmailService
{
    Task SendEmailAsync(
        string templateId,
        string templateData,
        string sender,
        IReadOnlyList<string> toAddresses,
        IReadOnlyList<string> ccAddresses,
        IReadOnlyList<string> bccAddresses,
        CancellationToken cancellationToken);

    Task SendRawEmailAsync(
        string subject,
        string bodyText,
        string bodyHtml,
        string sender,
        IReadOnlyList<string> toAddresses,
        IReadOnlyList<string> ccAddresses,
        IReadOnlyList<string> bccAddresses,
        IReadOnlyList<EmailAttachment> emailAttachments,
        CancellationToken cancellationToken);
}

public class EmailService(ILogger<EmailService> logger) : IEmailService
{
    public async Task SendEmailAsync(
        string templateId,
        string templateData,
        string sender,
        IReadOnlyList<string> toAddresses,
        IReadOnlyList<string> ccAddresses,
        IReadOnlyList<string> bccAddresses,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Sending templated email. TemplateId={TemplateId}, To={ToCount}, Cc={CcCount}, Bcc={BccCount}",
            templateId,
            toAddresses.Count,
            ccAddresses.Count,
            bccAddresses.Count);

        using var client = new AmazonSimpleEmailServiceClient();
        var request = new SendTemplatedEmailRequest
        {
            Source = sender,
            Destination = new Destination
            {
                ToAddresses = toAddresses.ToList(),
                CcAddresses = ccAddresses.ToList(),
                BccAddresses = bccAddresses.ToList(),
            },
            Template = templateId,
            TemplateData = templateData,
        };

        await client.SendTemplatedEmailAsync(request, cancellationToken);
        logger.LogInformation("Templated email sent successfully. TemplateId={TemplateId}", templateId);
    }

    public async Task SendRawEmailAsync(
        string subject,
        string bodyText,
        string bodyHtml,
        string sender,
        IReadOnlyList<string> toAddresses,
        IReadOnlyList<string> ccAddresses,
        IReadOnlyList<string> bccAddresses,
        IReadOnlyList<EmailAttachment> emailAttachments,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Sending raw email. SubjectLength={SubjectLength}, To={ToCount}, Cc={CcCount}, Bcc={BccCount}, Attachments={AttachmentCount}",
            subject.Length,
            toAddresses.Count,
            ccAddresses.Count,
            bccAddresses.Count,
            emailAttachments.Count);

        var message = new MimeMessage();

        message.From.Add(MailboxAddress.Parse(sender));
        message.To.AddRange(toAddresses.Select(MailboxAddress.Parse));
        message.Cc.AddRange(ccAddresses.Select(MailboxAddress.Parse));
        message.Bcc.AddRange(bccAddresses.Select(MailboxAddress.Parse));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            TextBody = bodyText,
            HtmlBody = bodyHtml,
        };

        foreach (var emailAttachment in emailAttachments)
        {
            emailAttachment.Stream.Seek(0, SeekOrigin.Begin);

            await bodyBuilder.Attachments.AddAsync(
                emailAttachment.Name,
                emailAttachment.Stream,
                ContentType.Parse(emailAttachment.MimeType),
                cancellationToken);
        }

        message.Body = bodyBuilder.ToMessageBody();

        using var memoryStream = new MemoryStream();
        await message.WriteToAsync(memoryStream, cancellationToken);
        memoryStream.Seek(0, SeekOrigin.Begin);

        var rawMessage = new RawMessage(memoryStream);
        var request = new SendRawEmailRequest
        {
            RawMessage = rawMessage,
        };

        using var client = new AmazonSimpleEmailServiceClient();
        await client.SendRawEmailAsync(request, cancellationToken);
        logger.LogInformation("Raw email sent successfully. SubjectLength={SubjectLength}", subject.Length);
    }
}
