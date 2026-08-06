using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Grpc.Skedular.Core.Core.V1;
using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Activities;
using Booking.Shared.Configurations;
using Booking.Shared.Repositories;
using Enterprise.Shared.Email;
using Enterprise.Shared.Grpc;
using Google.Protobuf;
using Microsoft.Extensions.Hosting;
using QuestPDF.Companion;
using QuestPDF.Fluent;
using BookingEntity = Booking.Shared.Database.Entities.Booking;
using RecurringBookingEntity = Booking.Shared.Database.Entities.RecurringBooking;

namespace Booking.Shared.Services;

public interface ISkedularInvoiceService
{
    Task GenerateAndSendInvoiceAsync(
        GenerateAndSendInvoiceInput args,
        BookingEntity booking,
        string organizationId,
        CancellationToken cancellationToken);

    Task GenerateAndSendRecurringInvoiceAsync(
        GenerateAndSendRecurringInvoiceInput args,
        RecurringBookingEntity recurringBooking,
        string organizationId,
        CancellationToken cancellationToken);
}

public class SkedularInvoiceService(
    EmailConfiguration emailConfiguration,
    CoreConfiguration coreConfiguration,
    CoreService.CoreServiceClient coreServiceClient,
    IRepositoryFactory repositoryFactory,
    IBookingInvoiceService bookingInvoiceService,
    IEmailService emailService,
    IHostEnvironment hostEnvironment) : ISkedularInvoiceService
{
    public async Task GenerateAndSendInvoiceAsync(
        GenerateAndSendInvoiceInput args,
        BookingEntity booking,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var marketplaceBooking = booking.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        var invoiceDocument = await bookingInvoiceService.GenerateInvoiceAsync(args.BookingId, cancellationToken);
        if (invoiceDocument is null)
        {
            return;
        }

        await using var pdfStream = new MemoryStream();
        invoiceDocument.GeneratePdf(pdfStream);
        pdfStream.Seek(0, SeekOrigin.Begin);

        marketplaceBooking.InvoiceUrl = await UploadInvoicePdfAsync(pdfStream, cancellationToken);
        repositoryFactory.MarketplaceBookingRepository.Update(marketplaceBooking);
        await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForMarketplaceBookingAsync(
            marketplaceBooking.Id, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        if (hostEnvironment.IsDevelopment())
        {
#pragma warning disable CS4014
            invoiceDocument.ShowInCompanionAsync();
#pragma warning restore CS4014
        }

        await SendInvoiceEmailAsync(args, booking, organizationId, pdfStream, cancellationToken);
    }

    public async Task GenerateAndSendRecurringInvoiceAsync(
        GenerateAndSendRecurringInvoiceInput args,
        RecurringBookingEntity recurringBooking,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var marketplaceBooking = recurringBooking.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        var invoiceDocument = await bookingInvoiceService.GenerateRecurringInvoiceAsync(args.RecurringBookingId, cancellationToken);
        if (invoiceDocument is null)
        {
            return;
        }

        await using var pdfStream = new MemoryStream();
        invoiceDocument.GeneratePdf(pdfStream);
        pdfStream.Seek(0, SeekOrigin.Begin);

        marketplaceBooking.InvoiceUrl = await UploadInvoicePdfAsync(pdfStream, cancellationToken);
        repositoryFactory.MarketplaceBookingRepository.Update(marketplaceBooking);
        await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForMarketplaceBookingAsync(
            marketplaceBooking.Id, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        if (hostEnvironment.IsDevelopment())
        {
#pragma warning disable CS4014
            invoiceDocument.ShowInCompanionAsync();
#pragma warning restore CS4014
        }

        await SendRecurringInvoiceEmailAsync(args, recurringBooking, organizationId, pdfStream, cancellationToken);
    }

    private async Task<string> UploadInvoicePdfAsync(MemoryStream pdfStream, CancellationToken cancellationToken)
    {
        pdfStream.Seek(0, SeekOrigin.Begin);

        using var call = coreServiceClient.Admin_UploadToPrivateStorage(
            coreConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        ArgumentNullException.ThrowIfNull(call);

        int bytesRead;
        var buffer = new byte[64 * 1024];
        while ((bytesRead = await pdfStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
        {
            await call.RequestStream.WriteAsync(
                new UploadFileRequest
                {
                    Extension = ".pdf",
                    ContentType = "application/pdf",
                    Chunk = ByteString.CopyFrom(buffer, 0, bytesRead),
                },
                cancellationToken);
        }

        await call.RequestStream.CompleteAsync();

        return (await call.ResponseAsync).Original.Url;
    }

    private async Task SendInvoiceEmailAsync(
        GenerateAndSendInvoiceInput args,
        BookingEntity booking,
        string organizationId,
        MemoryStream pdfStream,
        CancellationToken cancellationToken)
    {
        var marketplaceBooking = booking.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        if (args.InvoiceEmailList.Count == 0)
        {
            return;
        }

        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               organizationId,
                               null,
                               false,
                               false,
                               cancellationToken) ??
                           throw new OrganizationNotFound();

        await using var htmlTemplateStream = typeof(SkedularInvoiceService).Assembly.GetManifestResourceStream(
            "Booking.Shared.EmailTemplates.BookingInvoice.template.html");
        ArgumentNullException.ThrowIfNull(htmlTemplateStream);
        using var htmlReader = new StreamReader(htmlTemplateStream);
        var html = await htmlReader.ReadToEndAsync(cancellationToken);

        await using var textTemplateStream = typeof(SkedularInvoiceService).Assembly.GetManifestResourceStream(
            "Booking.Shared.EmailTemplates.BookingInvoice.template.txt");
        ArgumentNullException.ThrowIfNull(textTemplateStream);
        using var textReader = new StreamReader(textTemplateStream);
        var text = await textReader.ReadToEndAsync(cancellationToken);

        html = html
            .Replace("{{COMPANY_NAME}}", organization.Name)
            .Replace("{{INVOICE_NUMBER}}", marketplaceBooking.InvoiceNumber)
            .Replace("{{RECIPIENT_NAME}}", booking.CreatedByCustomer is null ? string.Empty : booking.CreatedByCustomer.ToDisplayableName());

        text = text
            .Replace("{{COMPANY_NAME}}", organization.Name)
            .Replace("{{INVOICE_NUMBER}}", marketplaceBooking.InvoiceNumber)
            .Replace("{{RECIPIENT_NAME}}", booking.CreatedByCustomer is null ? string.Empty : booking.CreatedByCustomer.ToDisplayableName());

        var attachments = new List<EmailAttachment>
        {
            new(pdfStream, $"{marketplaceBooking.InvoiceNumber}.pdf", "application/pdf"),
        };
        var subject = $"Invoice #{marketplaceBooking.InvoiceNumber} from {organization.Name}";

        await emailService.SendRawEmailAsync(
            subject,
            text,
            html,
            $"{organization.Name} {emailConfiguration.BookingInvoiceEmailSender}",
            args.InvoiceEmailList,
            [],
            [],
            attachments,
            cancellationToken);
    }

    private async Task SendRecurringInvoiceEmailAsync(
        GenerateAndSendRecurringInvoiceInput args,
        RecurringBookingEntity recurringBooking,
        string organizationId,
        MemoryStream pdfStream,
        CancellationToken cancellationToken)
    {
        var marketplaceBooking = recurringBooking.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        if (args.InvoiceEmailList.Count == 0)
        {
            return;
        }

        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               organizationId,
                               null,
                               false,
                               false,
                               cancellationToken) ??
                           throw new OrganizationNotFound();

        await using var htmlTemplateStream = typeof(SkedularInvoiceService).Assembly.GetManifestResourceStream(
            "Booking.Shared.EmailTemplates.BookingInvoice.template.html");
        ArgumentNullException.ThrowIfNull(htmlTemplateStream);
        using var htmlReader = new StreamReader(htmlTemplateStream);
        var html = await htmlReader.ReadToEndAsync(cancellationToken);

        await using var textTemplateStream = typeof(SkedularInvoiceService).Assembly.GetManifestResourceStream(
            "Booking.Shared.EmailTemplates.BookingInvoice.template.txt");
        ArgumentNullException.ThrowIfNull(textTemplateStream);
        using var textReader = new StreamReader(textTemplateStream);
        var text = await textReader.ReadToEndAsync(cancellationToken);

        var recipientName = recurringBooking.CreatedByCustomer?.ToDisplayableName()
                            ?? recurringBooking.InvolvedCustomers.FirstOrDefault()?.ToDisplayableName()
                            ?? string.Empty;

        html = html
            .Replace("{{COMPANY_NAME}}", organization.Name)
            .Replace("{{INVOICE_NUMBER}}", marketplaceBooking.InvoiceNumber)
            .Replace("{{RECIPIENT_NAME}}", recipientName);

        text = text
            .Replace("{{COMPANY_NAME}}", organization.Name)
            .Replace("{{INVOICE_NUMBER}}", marketplaceBooking.InvoiceNumber)
            .Replace("{{RECIPIENT_NAME}}", recipientName);

        var attachments = new List<EmailAttachment>
        {
            new(pdfStream, $"{marketplaceBooking.InvoiceNumber}.pdf", "application/pdf"),
        };
        var subject = $"Invoice #{marketplaceBooking.InvoiceNumber} from {organization.Name}";

        await emailService.SendRawEmailAsync(
            subject,
            text,
            html,
            $"{organization.Name} {emailConfiguration.BookingInvoiceEmailSender}",
            args.InvoiceEmailList,
            [],
            [],
            attachments,
            cancellationToken);
    }
}
