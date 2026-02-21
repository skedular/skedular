using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Core.V1;
using Api.Shared.Services.Models;
using Booking.Shared.Configurations;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Email;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Grpc;
using Google.Protobuf;
using Microsoft.Extensions.Hosting;
using QuestPDF.Companion;
using QuestPDF.Fluent;
using Temporalio.Activities;
using Constants = Booking.Shared.GraphQL.Constants;

namespace Booking.Shared.Activities;

public class InvoiceIntegrations(
    EmailConfiguration emailConfiguration,
    CoreConfiguration coreConfiguration,
    CoreService.CoreServiceClient coreServiceClient,
    IRepositoryFactory repositoryFactory,
    IBookingInvoiceService bookingInvoiceService,
    IDbTransactionBuilder transactionBuilder,
    IOrganizationInvoiceCounterService organizationInvoiceCounterService,
    IEmailService emailService,
    IHostEnvironment hostEnvironment,
    IGraphQlTopicEventSender graphQlTopicEventSender)
{
    [Activity]
    public async Task GenerateAndSendInvoiceAsync(GenerateAndSendInvoiceInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(args.BookingId, cancellationToken);
        if (booking is null || booking.IsDeleted() || booking.MarketplaceBooking is null)
        {
            return;
        }

        var marketplaceBooking = booking.MarketplaceBooking;
        var productVersionIds = marketplaceBooking.LineItems.Select(item => item.ProductVersionId).Distinct().ToList();
        var productVersions = await repositoryFactory.ProductVersionRepository.GetByIdsAsync(productVersionIds, cancellationToken);
        var organizationId = productVersions.First().Product.Organization.Id;

        if (string.IsNullOrWhiteSpace(marketplaceBooking.InvoiceNumber))
        {
            await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);
            marketplaceBooking.InvoiceNumber =
                await organizationInvoiceCounterService.GetNextInvoiceNumberIdAsync(organizationId, cancellationToken);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        var invoiceDocument = await bookingInvoiceService.GenerateInvoiceAsync(args.BookingId, args.FullyPaid, cancellationToken);
        if (invoiceDocument is null)
        {
            return;
        }

        await using var pdfStream = new MemoryStream();
        invoiceDocument.GeneratePdf(pdfStream);
        pdfStream.Seek(0, SeekOrigin.Begin);

        var call = coreServiceClient.Admin_UploadToPrivateStorage(
            coreConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        ArgumentNullException.ThrowIfNull(call);

        int bytesRead;
        var buffer = new byte[64 * 1024];
        while ((bytesRead = await pdfStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
        {
            var request = new UploadFileRequest
            {
                Extension = ".pdf", ContentType = "application/pdf", Chunk = ByteString.CopyFrom(buffer, 0, bytesRead)
            };

            await call.RequestStream.WriteAsync(request, cancellationToken);
        }

        await call.RequestStream.CompleteAsync();

        var fileUploadResponse = await call.ResponseAsync;
        marketplaceBooking.InvoiceUrl = fileUploadResponse.Original.Url;
        repositoryFactory.MarketplaceBookingRepository.Update(marketplaceBooking);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        if (hostEnvironment.IsDevelopment())
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            // ReSharper disable once MethodSupportsCancellation
            invoiceDocument.ShowInCompanionAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        await SendInvoiceEmailAsync(args, booking, organizationId, pdfStream, cancellationToken);

        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, booking.Id, cancellationToken);
    }

    private async Task SendInvoiceEmailAsync(
        GenerateAndSendInvoiceInput args,
        Database.Entities.Booking booking,
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

        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                               organizationId,
                               null,
                               false,
                               false,
                               cancellationToken) ??
                           throw new OrganizationNotFound();

        await using var htmlTemplateStream = typeof(InvoiceIntegrations).Assembly.GetManifestResourceStream(
            "Booking.Shared.EmailTemplates.BookingInvoice.template.html");
        ArgumentNullException.ThrowIfNull(htmlTemplateStream);
        using var htmlReader = new StreamReader(htmlTemplateStream);
        var html = await htmlReader.ReadToEndAsync(cancellationToken);

        await using var textTemplateStream = typeof(InvoiceIntegrations).Assembly.GetManifestResourceStream(
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

        var attachments = new List<EmailAttachment> { new(pdfStream, $"{marketplaceBooking.InvoiceNumber}.pdf", "application/pdf") };

        var subject = args.FullyPaid
            ? $"Invoice #{marketplaceBooking.InvoiceNumber} from {organization.Name}"
            : $"Invoice #{marketplaceBooking.InvoiceNumber} from {organization.Name} is due";

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
