using System.Globalization;
using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Time;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Organization = Api.Shared.Services.Grpc.Skedular.Organization.V1.Organization;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;
using ProductVersion = Booking.Shared.Database.Entities.ProductVersion;

namespace Booking.Shared.Services;

/// <summary>
///     Service for generating booking invoices.
/// </summary>
public interface IBookingInvoiceService
{
    /// <summary>
    ///     Generates an invoice document for the specified booking.
    /// </summary>
    Task<IDocument?> GenerateInvoiceAsync(string bookingId, bool fullyPaid, CancellationToken cancellationToken);

    /// <summary>
    ///     Generates an invoice document for the specified recurring booking cycle.
    /// </summary>
    Task<IDocument?> GenerateRecurringInvoiceAsync(string recurringBookingId, bool fullyPaid, CancellationToken cancellationToken);
}

/// <summary>
///     Implementation of the booking invoice service.
/// </summary>
public class BookingInvoiceService(
    IRepositoryFactory repositoryFactory,
    OrganizationConfiguration organizationConfiguration,
    OrganizationService.OrganizationServiceClient organizationServiceClient,
    IProductVersionHelperService productVersionHelperService) : IBookingInvoiceService
{
    public async Task<IDocument?> GenerateInvoiceAsync(string bookingId, bool fullyPaid, CancellationToken cancellationToken)
    {
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (booking is null || booking.IsDeleted() || booking.MarketplaceBooking is null)
        {
            return null;
        }

        var marketplaceBooking = booking.MarketplaceBooking;
        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(marketplaceBooking.ProductVersion.Id, cancellationToken);
        if (productVersion is null)
        {
            throw new ProductVersionNotFound();
        }

        var (organization, bankAccount) = await GetOrganizationAndBankAccountAsync(productVersion.Product.Organization.Id, cancellationToken);

        return new BookingInvoiceDocument(
            booking,
            bankAccount,
            organization,
            productVersion,
            marketplaceBooking.PaymentExpiry,
            fullyPaid,
            productVersionHelperService);
    }

    public async Task<IDocument?> GenerateRecurringInvoiceAsync(string recurringBookingId, bool fullyPaid, CancellationToken cancellationToken)
    {
        var recurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(recurringBookingId, cancellationToken);
        if (recurringBooking is null || recurringBooking.IsDeleted() || recurringBooking.MarketplaceBooking is null)
        {
            return null;
        }

        var marketplaceBooking = recurringBooking.MarketplaceBooking;
        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(marketplaceBooking.ProductVersion.Id, cancellationToken);
        if (productVersion is null)
        {
            throw new ProductVersionNotFound();
        }

        var (organization, bankAccount) = await GetOrganizationAndBankAccountAsync(productVersion.Product.Organization.Id, cancellationToken);

        return new RecurringInvoiceDocument(recurringBooking, bankAccount, organization, productVersion, marketplaceBooking.PaymentExpiry, fullyPaid);
    }

    private async Task<(Organization Organization, BankAccount BankAccount)> GetOrganizationAndBankAccountAsync(
        string organizationId,
        CancellationToken cancellationToken)
    {
        var bankAccountConnection = await organizationServiceClient.Admin_GetBankAccountsAsync(
            new Admin_GetBankAccountsInput
            {
                After = string.Empty,
                First = ((int?)null).ToNullInt(),
                Before = string.Empty,
                Last = ((int?)null).ToNullInt(),
                Where = new BankAccountWhereInput { OrganizationId = organizationId }
            },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
        var bankAccount = bankAccountConnection.Edges.Select(item => item.Node).First(item => item.IsDefault);
        var organization = await organizationServiceClient.Admin_GetAsync(
            new Admin_GetInput { Id = organizationId },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
        ArgumentNullException.ThrowIfNull(organization);

        return (organization, bankAccount);
    }

    private abstract class InvoiceDocumentBase(
        BankAccount bankAccount,
        Organization organization,
        ProductVersion productVersion,
        DateTimeOffset dueDate,
        bool fullyPaid) : IDocument
    {
        protected ProductVersion ProductVersion => productVersion;
        protected Organization Organization => organization;

        public void Compose(IDocumentContainer container) =>
            container
                .Page(page =>
                {
                    page.Margin(50);

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });

        private void ComposeHeader(IContainer container) =>
            container.Row(row =>
            {
                row.RelativeItem(2).Column(column => column.Item().Text("TAX INVOICE").Bold().FontSize(20));

                row.RelativeItem().AlignRight().Column(column =>
                {
                    column.Item().Text("Invoice Date").SemiBold().FontSize(13);
                    column.Item().Text(GetInvoiceDate().ToShortDate()).FontSize(10);

                    if (!string.IsNullOrWhiteSpace(GetInvoiceNumber()))
                    {
                        column.Item().Text(string.Empty);
                        column.Item().Text("Invoice Number").SemiBold().FontSize(13);
                        column.Item().Text(GetInvoiceNumber()).FontSize(10);
                    }

                    var taxDetails = organization.TaxDetails;
                    if (taxDetails is not null)
                    {
                        column.Item().Text(string.Empty);
                        column.Item().Text("GST Number").SemiBold().FontSize(13);
                        column.Item().Text(taxDetails.TaxId).FontSize(10);
                    }
                });

                row.RelativeItem().AlignRight().Column(column =>
                {
                    column.Item().Text(organization.Name).FontSize(10);

                    if (organization.PhysicalAddress is not null)
                    {
                        column.Item().Text(organization.PhysicalAddress.FormattedAddress).FontSize(10);
                    }
                });
            });

        private void ComposeContent(IContainer container) =>
            container.PaddingVertical(40).Column(column =>
            {
                column.Spacing(20);
                var currency = productVersion.Currency;
                ArgumentException.ThrowIfNullOrWhiteSpace(currency);

                column.Item().Element(ComposeTable);
                column.Item().Component(new TotalsExcludeGstComponent(GetTotalAmountExcludeTax(), GetTaxRatePercentage(), GetTaxAmount()));
                column.Item().Component(new TotalsAmountComponent(currency, GetTotalAmount(), fullyPaid));

                if (GetPaymentMethod() == PaymentMethodConstants.BankTransfer)
                {
                    column.Item().Component(new DueDateSection(dueDate, bankAccount));
                }
            });

        private void ComposeTable(IContainer container) =>
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().BorderBottom(1).PaddingBottom(3).Text("Description").Bold();
                    header.Cell().BorderBottom(1).PaddingBottom(3).AlignRight().Text("Quantity").Bold();
                    header.Cell().BorderBottom(1).PaddingBottom(3).AlignRight().Text("Unit Price").Bold();

                    var currency = productVersion.Currency;
                    ArgumentException.ThrowIfNullOrWhiteSpace(currency);

                    header.Cell().BorderBottom(1).PaddingBottom(3).AlignRight().Text($"Amount {currency.ToInvoiceCurrencyName()}").Bold();
                });

                table.Cell().Element(CellStyle).Padding(8).Text(GetDescription());
                table.Cell().Element(CellStyle).AlignRight().Text(FormatQuantity(GetQuantity()));
                table.Cell().Element(CellStyle).AlignRight().Text(GetUnitPriceLabel());
                table.Cell().Element(CellStyle).AlignRight().Text(GetLineAmount().ToRoundedPrice());

                static IContainer CellStyle(IContainer container)
                {
                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                }

                static string FormatQuantity(decimal quantity)
                {
                    return quantity == decimal.Truncate(quantity)
                        ? decimal.Truncate(quantity).ToString(CultureInfo.InvariantCulture)
                        : quantity.ToString("0.####", CultureInfo.InvariantCulture);
                }
            });

        protected abstract DateTimeOffset GetInvoiceDate();
        protected abstract string? GetInvoiceNumber();
        protected abstract string GetDescription();
        protected abstract decimal GetQuantity();
        protected abstract decimal GetLineAmount();
        protected abstract string GetUnitPriceLabel();
        protected abstract string GetPaymentMethod();
        protected abstract decimal GetTotalAmountExcludeTax();
        protected abstract decimal GetTaxAmount();
        protected abstract decimal GetTaxRatePercentage();
        protected abstract decimal GetTotalAmount();
    }

    private class BookingInvoiceDocument(
        Database.Entities.Booking booking,
        BankAccount bankAccount,
        Organization organization,
        ProductVersion productVersion,
        DateTimeOffset dueDate,
        bool fullyPaid,
        IProductVersionHelperService productVersionHelperService)
        : InvoiceDocumentBase(bankAccount, organization, productVersion, dueDate, fullyPaid)
    {
        protected override DateTimeOffset GetInvoiceDate() => booking.CreatedAt;
        protected override string? GetInvoiceNumber() => booking.MarketplaceBooking?.InvoiceNumber;
        protected override string GetPaymentMethod() => booking.MarketplaceBooking?.PaymentMethod ?? string.Empty;
        protected override decimal GetTotalAmountExcludeTax() => booking.MarketplaceBooking?.TotalAmountExcludeTax ?? 0;
        protected override decimal GetTaxAmount() => booking.MarketplaceBooking?.TaxAmount ?? 0;
        protected override decimal GetTaxRatePercentage() => booking.MarketplaceBooking?.TaxRatePercentage ?? 0;
        protected override decimal GetTotalAmount() => booking.MarketplaceBooking?.TotalAmount ?? 0;

        protected override string GetDescription() =>
            $"{ProductVersion.ListingMetadata?.Title}{Environment.NewLine}{booking.From.ToShortDate()}{Environment.NewLine}{booking.From.ToShortTime()} - {booking.Until.ToShortTime()}";

        protected override decimal GetQuantity()
        {
            var marketplaceBooking = booking.MarketplaceBooking;
            ArgumentNullException.ThrowIfNull(marketplaceBooking);

            var pricing = ResolvePricing();
            var totalMinutes = (decimal)(booking.Until - booking.From).TotalMinutes;

            return pricing.BookingCadence switch
            {
                ProductPricingCadence.OneTime => marketplaceBooking.Quantity,
                ProductPricingCadence.HalfDay => marketplaceBooking.Quantity,
                ProductPricingCadence.Daily => marketplaceBooking.Quantity,
                ProductPricingCadence.PerMinute => marketplaceBooking.Quantity * totalMinutes,
                ProductPricingCadence.Per15Minutes => marketplaceBooking.Quantity * (totalMinutes / 15m),
                ProductPricingCadence.Per30Minutes => marketplaceBooking.Quantity * (totalMinutes / 30m),
                ProductPricingCadence.PerHour => marketplaceBooking.Quantity * (totalMinutes / 60m),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected override decimal GetLineAmount()
        {
            var marketplaceBooking = booking.MarketplaceBooking;
            ArgumentNullException.ThrowIfNull(marketplaceBooking);

            var unitPrice = ResolveUnitPrice();
            return unitPrice * GetQuantity();
        }

        protected override string GetUnitPriceLabel()
        {
            var pricing = ResolvePricing();
            return $"{ResolveUnitPrice().ToRoundedPrice()} {pricing.BookingCadence.ToInvoicePriceUnitName()}";
        }

        private ProductPricing ResolvePricing()
        {
            var marketplaceBooking = booking.MarketplaceBooking;
            ArgumentNullException.ThrowIfNull(marketplaceBooking);
            ArgumentNullException.ThrowIfNull(ProductVersion.PricingOptions);

            var pricing = productVersionHelperService.FindMatchingPricing(ProductVersion.PricingOptions, marketplaceBooking.ProductPricing);
            ArgumentNullException.ThrowIfNull(pricing);
            return pricing;
        }

        private decimal ResolveUnitPrice()
        {
            var pricing = ResolvePricing();
            return ResolveExcludingTax(pricing.Price, pricing.IsTaxInclusive);
        }

        private decimal ResolveExcludingTax(decimal price, bool isTaxInclusive)
        {
            var taxRate = booking.MarketplaceBooking?.TaxRatePercentage;
            if (taxRate is null or 0)
            {
                return price;
            }

            return isTaxInclusive ? price * 100 / (taxRate.Value + 100) : price;
        }
    }

    private class RecurringInvoiceDocument(
        RecurringBooking recurringBooking,
        BankAccount bankAccount,
        Organization organization,
        ProductVersion productVersion,
        DateTimeOffset dueDate,
        bool fullyPaid)
        : InvoiceDocumentBase(bankAccount, organization, productVersion, dueDate, fullyPaid)
    {
        protected override DateTimeOffset GetInvoiceDate() => recurringBooking.CreatedAt;
        protected override string? GetInvoiceNumber() => recurringBooking.MarketplaceBooking?.InvoiceNumber;
        protected override string GetPaymentMethod() => recurringBooking.MarketplaceBooking?.PaymentMethod ?? string.Empty;
        protected override decimal GetTotalAmountExcludeTax() => CalculateRecurringAmounts().TotalAmountExcludeTax;
        protected override decimal GetTaxAmount() => CalculateRecurringAmounts().TaxAmount;
        protected override decimal GetTaxRatePercentage() => CalculateRecurringAmounts().TaxRatePercentage;
        protected override decimal GetTotalAmount() => CalculateRecurringAmounts().TotalAmount;

        protected override string GetDescription()
        {
            var marketplaceBooking = recurringBooking.MarketplaceBooking;
            ArgumentNullException.ThrowIfNull(marketplaceBooking);
            var cycleEnd = recurringBooking.EndDate ?? recurringBooking.StartDate;

            return
                $"{ProductVersion.ListingMetadata?.Title}{Environment.NewLine}" +
                $"{marketplaceBooking.ProductPricing.PurchaseCadence.ToProductPricingCadenceName()} pass{Environment.NewLine}" +
                $"{recurringBooking.StartDate.ToShortDate()} - {cycleEnd.ToShortDate()}";
        }

        protected override decimal GetQuantity() => recurringBooking.MarketplaceBooking?.Quantity ?? 0;

        protected override decimal GetLineAmount() => CalculateRecurringAmounts().TotalAmountExcludeTax;

        protected override string GetUnitPriceLabel()
        {
            var marketplaceBooking = recurringBooking.MarketplaceBooking;
            ArgumentNullException.ThrowIfNull(marketplaceBooking);

            var subtotal = CalculateRecurringAmounts().TotalAmountExcludeTax;
            var quantity = marketplaceBooking.Quantity;
            var unitPrice = quantity > 0 ? subtotal / quantity : subtotal;

            return $"{unitPrice.ToRoundedPrice()} {marketplaceBooking.ProductPricing.PurchaseCadence.ToInvoicePriceUnitName()}";
        }

        private (decimal TotalAmountExcludeTax, decimal TaxAmount, decimal TaxRatePercentage, decimal TotalAmount) CalculateRecurringAmounts()
        {
            var marketplaceBooking = recurringBooking.MarketplaceBooking;
            ArgumentNullException.ThrowIfNull(marketplaceBooking);

            var totalPrice = (marketplaceBooking.ProductPricing.Price * marketplaceBooking.Quantity).RoundedDecimal();
            if (Organization.TaxDetails is null)
            {
                return (totalPrice, 0.00m, 0.00m, totalPrice);
            }

            var taxRatePercentage = Convert.ToDecimal(Organization.TaxDetails.TaxRatePercentage).RoundedDecimal();
            if (marketplaceBooking.ProductPricing.IsTaxInclusive)
            {
                var totalAmount = totalPrice;
                var totalAmountExcludeTax = (totalAmount * 100 / (100 + taxRatePercentage)).RoundedDecimal();
                var taxAmount = (totalAmount - totalAmountExcludeTax).RoundedDecimal();
                return (totalAmountExcludeTax, taxAmount, taxRatePercentage, totalAmount);
            }

            var subtotal = totalPrice;
            var taxAmountExclusive = (subtotal * taxRatePercentage / 100).RoundedDecimal();
            return (subtotal, taxAmountExclusive, taxRatePercentage, (subtotal + taxAmountExclusive).RoundedDecimal());
        }
    }

    private class TotalsExcludeGstComponent(decimal totalExcludeTax, decimal taxRatePercentage, decimal taxAmount) : IComponent
    {
        public void Compose(IContainer container) =>
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(5);
                    columns.RelativeColumn();
                });

                table.Cell().AlignRight().Text("Subtotal");
                table.Cell().PaddingBottom(5).AlignRight().Text(totalExcludeTax.ToRoundedPrice());

                table.Cell().AlignRight().Text($"TOTAL GST {taxRatePercentage.RoundedDecimal()}%");
                table.Cell().PaddingBottom(5).AlignRight().Text(taxAmount.ToRoundedPrice());

                table.Cell().PaddingLeft(300).LineHorizontal(1);
                table.Cell().LineHorizontal(1);
            });
    }

    private class TotalsAmountComponent(string currency, decimal totalAmount, bool fullyPaid) : IComponent
    {
        public void Compose(IContainer container)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(currency);

            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(5);
                    columns.RelativeColumn();
                });

                var totalAmountText = totalAmount.ToRoundedPrice();
                table.Cell().AlignRight().Text($"TOTAL {currency.ToInvoiceCurrencyName()}").Bold();
                table.Cell().PaddingBottom(5).AlignRight().Text(totalAmountText);

                if (fullyPaid)
                {
                    table.Cell().AlignRight().Text("Amount Paid");
                    table.Cell().PaddingBottom(5).AlignRight().Text(totalAmountText);
                    table.Cell().AlignRight().Text("Amount Due");
                    table.Cell().PaddingBottom(5).AlignRight().Text("0.00");
                }
                else
                {
                    table.Cell().AlignRight().Text("Amount Paid");
                    table.Cell().PaddingBottom(5).AlignRight().Text("0.00");
                    table.Cell().AlignRight().Text("Amount Due");
                    table.Cell().PaddingBottom(5).AlignRight().Text(totalAmountText);
                }

                table.Cell().PaddingLeft(300).LineHorizontal(1);
                table.Cell().LineHorizontal(1);
            });
        }
    }

    private class DueDateSection(DateTimeOffset dueDate, BankAccount bankAccount) : IComponent
    {
        public void Compose(IContainer container) =>
            container.ShowEntire().Column(column =>
            {
                column.Spacing(2);

                column.Item().Text($"Due Date: {dueDate.ToShortDate()}").Bold();
                column.Item().Text("Invoice to be paid into account:");
                column.Item().Text($"Bank Name: {bankAccount.BankName}");
                column.Item().Text($"Account Holder Name: {bankAccount.AccountHolderName}");
                column.Item().Text($"Account Number: {bankAccount.AccountNumber}");
                column.Item().Text($"Country: {bankAccount.Country}");
                column.Item().Text("**Please use correct invoice number when paying**");
            });
    }
}
