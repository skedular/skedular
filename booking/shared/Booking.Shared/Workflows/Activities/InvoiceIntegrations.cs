using Api.Shared.Clients.Configurations.Grpc;
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

namespace Booking.Shared.Workflows.Activities;

public class InvoiceIntegrations(
    CoreConfiguration coreConfiguration,
    CoreService.CoreServiceClient coreServiceClient,
    IRepositoryFactory repositoryFactory,
    IBookingInvoiceService bookingInvoiceService,
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
