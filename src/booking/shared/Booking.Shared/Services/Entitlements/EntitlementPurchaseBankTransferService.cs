using System.Globalization;
using Api.Shared.Grpc.Skedular.Organization.Billing.V1;
using Api.Shared.Services.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;

namespace Booking.Shared.Services.Entitlements;

public interface IEntitlementPurchaseBankTransferService
{
    Task<EntitlementPurchaseBankTransferAction> CreateInvoiceAsync(string purchaseId, CancellationToken cancellationToken);
}

public sealed record EntitlementPurchaseBankTransferAction(string InvoiceNumber, string PaymentInstructions);

/// <summary>
///     Creates the durable, customer-visible manual-settlement action for a standalone
///     entitlement purchase. It deliberately does not use booking invoice workflows:
///     an entitlement purchase is an order, not a reservation.
/// </summary>
public sealed class EntitlementPurchaseBankTransferService(
    IRepositoryFactory repositoryFactory,
    IOrganizationInvoiceCounterService organizationInvoiceCounterService,
    OrganizationConfiguration organizationConfiguration,
    OrganizationBillingService.OrganizationBillingServiceClient organizationBillingServiceClient) : IEntitlementPurchaseBankTransferService
{
    public async Task<EntitlementPurchaseBankTransferAction> CreateInvoiceAsync(string purchaseId, CancellationToken cancellationToken)
    {
        var purchase = await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(purchaseId, cancellationToken)
                       ?? throw new InvalidOperationException("The entitlement purchase could not be found.");
        if (purchase.PaymentStatus != PaymentStatusConstants.Pending ||
            purchase.PaymentMethod != PaymentMethod.BankTransfer.ToPaymentMethod())
        {
            throw new InvalidOperationException("Only pending bank-transfer entitlement purchases can create an invoice.");
        }

        if (!string.IsNullOrWhiteSpace(purchase.InvoiceNumber) && !string.IsNullOrWhiteSpace(purchase.PaymentInstructions))
        {
            return new EntitlementPurchaseBankTransferAction(purchase.InvoiceNumber, purchase.PaymentInstructions);
        }

        var accounts = await organizationBillingServiceClient.Admin_GetBankAccountsAsync(
            new Admin_GetBankAccountsInput
            {
                After = string.Empty,
                First = ((int?)null).ToNullInt(),
                Before = string.Empty,
                Last = ((int?)null).ToNullInt(),
                Where = new BankAccountWhereInput
                {
                    OrganizationId = purchase.OrganizationId,
                },
            },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
        var account = accounts.Edges.Select(item => item.Node).FirstOrDefault(item => item.IsDefault)
                      ?? throw new InvalidOperationException("A default bank account is required for bank-transfer entitlement purchases.");

        var invoiceNumber = purchase.InvoiceNumber ?? await organizationInvoiceCounterService.GetNextInvoiceNumberIdAsync(
            purchase.OrganizationId,
            cancellationToken);
        var paymentInstructions = string.Format(
            CultureInfo.InvariantCulture,
            "Invoice {0}: transfer {1} {2} to {3} ({4}), account {5}, using reference {6}. Payment is due by {7:yyyy-MM-dd HH:mm 'UTC'}.",
            invoiceNumber,
            purchase.Amount,
            purchase.Currency,
            account.AccountHolderName,
            account.BankName,
            account.AccountNumber,
            purchase.Id,
            purchase.PaymentExpiry);

        if (!await repositoryFactory.EntitlementPurchaseRepository.UpdateBankTransferInvoiceAsync(
                purchase.Id,
                invoiceNumber,
                paymentInstructions,
                cancellationToken))
        {
            throw new InvalidOperationException("The entitlement purchase could not be updated with bank-transfer invoice details.");
        }

        return new EntitlementPurchaseBankTransferAction(invoiceNumber, paymentInstructions);
    }
}
