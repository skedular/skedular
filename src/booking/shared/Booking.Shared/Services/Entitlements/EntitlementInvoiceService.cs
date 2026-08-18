using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Grpc;
using Google.Protobuf;
using QuestPDF.Fluent;
using CoreService = Api.Shared.Grpc.Skedular.Core.Core.V1.CoreService;
using UploadFileRequest = Api.Shared.Grpc.Skedular.Core.Core.V1.UploadFileRequest;

namespace Booking.Shared.Services.Entitlements;

public interface IEntitlementInvoiceService
{
    Task<string?> GenerateAsync(string purchaseId, CancellationToken cancellationToken);
}

public sealed class EntitlementInvoiceService(
    IRepositoryFactory repositoryFactory,
    CoreConfiguration coreConfiguration,
    CoreService.CoreServiceClient coreServiceClient,
    IXeroInvoiceService xeroInvoiceService,
    IBookingInvoiceService bookingInvoiceService,
    IOrganizationInvoiceCounterService organizationInvoiceCounterService) : IEntitlementInvoiceService
{
    public async Task<string?> GenerateAsync(string purchaseId, CancellationToken cancellationToken)
    {
        var purchase = await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(purchaseId, cancellationToken);
        if (purchase is null ||
            (purchase.PaymentStatus != PaymentStatusConstants.Pending && purchase.PaymentStatus != PaymentStatusConstants.Confirmed) ||
            !string.IsNullOrWhiteSpace(purchase.InvoiceUrl))
        {
            return purchase?.InvoiceUrl;
        }

        // Xero uses the local invoice number when creating the invoice. Allocate it
        // before either accounting or PDF generation so both paths use the same
        // organization-scoped SKD-###### format.
        purchase.InvoiceNumber ??= await organizationInvoiceCounterService.GetNextInvoiceNumberIdAsync(
            purchase.OrganizationId,
            cancellationToken);

        if (await xeroInvoiceService.TryHandleEntitlementPurchaseInvoiceAsync(purchase.OrganizationId, purchase, cancellationToken))
        {
            return purchase.InvoiceUrl;
        }

        var document = await bookingInvoiceService.GenerateEntitlementInvoiceAsync(purchaseId, cancellationToken);
        if (document is null)
        {
            return null;
        }

        await using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        stream.Position = 0;
        using var call =
            coreServiceClient.Admin_UploadToPrivateStorage(coreConfiguration.ApiKey.CreateMetadata(), cancellationToken: cancellationToken);
        var buffer = new byte[64 * 1024];
        int bytesRead;
        while ((bytesRead = await stream.ReadAsync(buffer, cancellationToken)) > 0)
        {
            await call.RequestStream.WriteAsync(new UploadFileRequest
            {
                Extension = ".pdf",
                ContentType = "application/pdf",
                Chunk = ByteString.CopyFrom(buffer, 0, bytesRead),
            }, cancellationToken);
        }

        await call.RequestStream.CompleteAsync();
        purchase.InvoiceUrl = (await call.ResponseAsync).Original.Url;
        repositoryFactory.EntitlementPurchaseRepository.Update(purchase);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        return purchase.InvoiceUrl;
    }
}
