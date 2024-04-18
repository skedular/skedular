using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace Notification.Processors.Services;

public interface IEmailService
{
    Task SendEmailAsync(
        string templateId,
        string templateData,
        string sender,
        ICollection<string> toAddresses,
        ICollection<string> ccAddresses,
        ICollection<string> bccAddresses,
        CancellationToken cancellationToken);
}

public class EmailService : IEmailService
{
    public async Task SendEmailAsync(
        string templateId,
        string templateData,
        string sender,
        ICollection<string> toAddresses,
        ICollection<string> ccAddresses,
        ICollection<string> bccAddresses,
        CancellationToken cancellationToken)
    {
        using var client = new AmazonSimpleEmailServiceClient();
        var request = new SendTemplatedEmailRequest
        {
            Source = sender,
            Destination = new Destination
            {
                ToAddresses = toAddresses.ToList(),
                CcAddresses = ccAddresses.ToList(),
                BccAddresses = bccAddresses.ToList()
            },
            Template = templateId,
            TemplateData = templateData
        };

        await client.SendTemplatedEmailAsync(request, cancellationToken);
    }
}
