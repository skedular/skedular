using Booking.Shared.Services;
using Microsoft.Extensions.Hosting;
using QuestPDF.Companion;
using Temporalio.Activities;

namespace Booking.Shared.Workflows.Activities;

public class InvoiceIntegrations(IBookingInvoiceService bookingInvoiceService, IHostEnvironment hostEnvironment)
{
    [Activity]
    public async Task GenerateAndSendInvoiceAsync(GenerateAndSendInvoiceInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var invoiceDocument = await bookingInvoiceService.GenerateInvoiceAsync(args.BookingId, args.FullyPaid, cancellationToken);
        if (invoiceDocument is null)
        {
            return;
        }

        if (hostEnvironment.IsDevelopment())
        {
            // ReSharper disable once MethodSupportsCancellation
            invoiceDocument.ShowInCompanionAsync();
        }
    }
}
