using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Core.V1;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Grpc;
using Google.Protobuf;
using Microsoft.Extensions.Hosting;
using QuestPDF.Companion;
using QuestPDF.Fluent;
using Temporalio.Activities;

namespace Booking.Shared.Activities;

public class InvoiceIntegrations(
    CoreConfiguration coreConfiguration,
    CoreService.CoreServiceClient coreServiceClient,
    IRepositoryFactory repositoryFactory,
    IBookingInvoiceService bookingInvoiceService,
    IDbTransactionBuilder transactionBuilder,
    IOrganizationInvoiceCounterService organizationInvoiceCounterService,
    IHostEnvironment hostEnvironment)
{
    [Activity]
    public async Task GenerateAndSendInvoiceAsync(GenerateAndSendInvoiceInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(args.BookingId, cancellationToken);
        if (booking is null || booking.IsDeleted())
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(booking.InvoiceNumber))
        {
            var productVersionIds = booking.LineItems.Select(item => item.ProductVersionId).Distinct().ToList();
            var productVersions = await repositoryFactory.ProductVersionRepository.GetByIdsAsync(productVersionIds, cancellationToken);
            if (productVersions.Count != productVersionIds.Count)
            {
                throw new InvalidOperationException();
            }

            var organizationIds = productVersions.Select(item => item.Product.Organization.Id).Distinct().ToList();
            if (organizationIds.Count > 1)
            {
                throw new CrossOrganizationProductBookingNotAllowed();
            }

            var organizationId = productVersions.First().Product.Organization.Id;
            await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);
            booking.InvoiceNumber = await organizationInvoiceCounterService.GetNextInvoiceNumberIdAsync(organizationId, cancellationToken);
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
        booking.InvoiceUrl = fileUploadResponse.Original.Url;
        repositoryFactory.BookingRepository.Update(booking);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        if (hostEnvironment.IsDevelopment())
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            // ReSharper disable once MethodSupportsCancellation
            invoiceDocument.ShowInCompanionAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
