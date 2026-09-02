using System.Globalization;
using Api.Shared.Grpc.Skedular.Organization.Billing.V1;
using Api.Shared.Grpc.Skedular.Organization.Core.V1;
using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Time;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Organization = Api.Shared.Grpc.Skedular.Organization.Core.V1.Organization;
using OrganizationBillingCycle = Api.Shared.Services.Models.OrganizationBillingCycle;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;
using ProductVersion = Booking.Shared.Database.Entities.ProductVersion;
using RecurringBooking = Booking.Shared.Database.Entities.RecurringBooking;

namespace Booking.Shared.Services;

/// <summary>
///     Service for generating booking invoices.
/// </summary>
public interface IBookingInvoiceService
{
    Task<IDocument?> GenerateEntitlementInvoiceAsync(string purchaseId, CancellationToken cancellationToken);

    /// <summary>
    ///     Generates an invoice document for the specified booking.
    /// </summary>
    Task<IDocument?> GenerateInvoiceAsync(string bookingId, CancellationToken cancellationToken);

    /// <summary>
    ///     Generates an invoice document for the specified recurring booking cycle.
    /// </summary>
    Task<IDocument?> GenerateRecurringInvoiceAsync(string recurringBookingId, CancellationToken cancellationToken);
}

/// <summary>
///     Implementation of the booking invoice service.
/// </summary>
public class BookingInvoiceService(
    IRepositoryFactory repositoryFactory,
    OrganizationConfiguration organizationConfiguration,
    OrganizationService.OrganizationServiceClient organizationServiceClient,
    OrganizationBillingService.OrganizationBillingServiceClient organizationBillingServiceClient,
    IInvoicePaymentTermsService invoicePaymentTermsService,
    IRecurringInvoiceBillingScheduleService recurringInvoiceBillingScheduleService,
    IProductVersionHelperService productVersionHelperService,
    IEntityMapper entityMapper,
    IOrganizationArrearsBillingPlannerService organizationArrearsBillingPlannerService) : IBookingInvoiceService
{
    public async Task<IDocument?> GenerateEntitlementInvoiceAsync(string purchaseId, CancellationToken cancellationToken)
    {
        var purchase = await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(purchaseId, cancellationToken);
        if (purchase is null)
        {
            return null;
        }

        var productVersion = purchase.ProductVersion;
        ArgumentNullException.ThrowIfNull(productVersion);
        var (organization, bankAccount) = await GetOrganizationAndBankAccountAsync(purchase.OrganizationId, cancellationToken);
        var dueDate = invoicePaymentTermsService.GetDueDate(purchase.CreatedAt, organization.BillingDetails?.InvoiceDueInDays);
        return new EntitlementInvoiceDocument(purchase, bankAccount, organization, productVersion, dueDate);
    }

    public async Task<IDocument?> GenerateInvoiceAsync(string bookingId, CancellationToken cancellationToken)
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
        var dueDate = invoicePaymentTermsService.GetDueDate(booking.CreatedAt, organization.BillingDetails?.InvoiceDueInDays);

        return new BookingInvoiceDocument(
            booking,
            bankAccount,
            organization,
            productVersion,
            dueDate,
            productVersionHelperService);
    }

    public async Task<IDocument?> GenerateRecurringInvoiceAsync(string recurringBookingId, CancellationToken cancellationToken)
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
        var dueDate = invoicePaymentTermsService.GetDueDate(recurringBooking.CreatedAt, organization.BillingDetails?.InvoiceDueInDays);
        var billingDefinition = recurringInvoiceBillingScheduleService.GetSchedule(
            recurringBooking,
            marketplaceBooking,
            productVersion.Product.Organization.BillingCycle.ToOrganizationBillingCycle());

        if (marketplaceBooking.BillingMode.ToProductPricingBillingMode() != ProductPricingBillingMode.InArrears)
        {
            return new RecurringInvoiceDocument(
                recurringBooking,
                bankAccount,
                organization,
                productVersion,
                dueDate,
                billingDefinition);
        }

        ArgumentNullException.ThrowIfNull(productVersion.Product);
        ArgumentNullException.ThrowIfNull(productVersion.Product.Organization);

        var recurringBookingModel = entityMapper.MapTo(recurringBooking);
        var draft = organizationArrearsBillingPlannerService.BuildInitialRecurringInvoiceDraft(
            recurringBookingModel,
            productVersion.Product.Organization.BillingCycle.ToOrganizationBillingCycle());
        if (draft is null)
        {
            return null;
        }

        return new RecurringInvoiceDocument(
            recurringBooking,
            bankAccount,
            organization,
            productVersion,
            dueDate,
            billingDefinition,
            draft.Lines.FirstOrDefault());
    }

    public static string FormatInvoicePeriod(DateTimeOffset from, DateTimeOffset until)
    {
        var isDateOnlyRange = from.TimeOfDay == TimeSpan.Zero && until.TimeOfDay == TimeSpan.Zero;
        return isDateOnlyRange
            ? $"{from.ToShortDate()} - {until.ToShortDate()}"
            : $"{from.ToShortDate()} {from.ToShortTime()} - {until.ToShortDate()} {until.ToShortTime()}";
    }

    public static string BuildBookingInvoiceLineDescription(
        ProductVersion productVersion,
        ProductPricing pricing,
        string details)
    {
        var title = productVersion.ListingMetadata?.Title ?? $"Marketplace {productVersion.Type} booking";
        var description = $"{title}{Environment.NewLine}{details}";

        return pricing.FulfillmentType == ProductPricingFulfillmentType.Entitlement &&
               pricing.EntitlementCreditQuantity is > 0
            ? $"{description}{Environment.NewLine}Credits included: {pricing.EntitlementCreditQuantity.Value}"
            : description;
    }

    public static string BuildEntitlementInvoiceLineDescription(
        ProductVersion productVersion,
        ProductPricing pricing,
        DateTimeOffset serviceStartAt)
    {
        var productTitle = productVersion.ListingMetadata?.Title ?? "Entitlement credits";
        var pricingOptionName = pricing.ListingMetadata?.Title;
        var description = string.IsNullOrWhiteSpace(pricingOptionName)
            ? productTitle
            : $"{productTitle}{Environment.NewLine}Pricing option: {pricingOptionName}";
        description +=
            $"{Environment.NewLine}Validity: {serviceStartAt:yyyy-MM-dd} to {serviceStartAt.AddDays(pricing.EntitlementValidityDays ?? 0):yyyy-MM-dd}";

        return pricing.EntitlementCreditQuantity is > 0
            ? $"{description}{Environment.NewLine}Credits included: {pricing.EntitlementCreditQuantity.Value}"
            : description;
    }

    private async Task<(Organization Organization, BankAccount BankAccount)> GetOrganizationAndBankAccountAsync(
        string organizationId,
        CancellationToken cancellationToken)
    {
        var bankAccountConnection = await organizationBillingServiceClient.Admin_GetBankAccountsAsync(
            new Admin_GetBankAccountsInput
            {
                After = string.Empty,
                First = ((int?)null).ToNullInt(),
                Before = string.Empty,
                Last = ((int?)null).ToNullInt(),
                Where = new BankAccountWhereInput
                {
                    OrganizationId = organizationId,
                },
            },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
        var bankAccount = bankAccountConnection.Edges.Select(item => item.Node).First(item => item.IsDefault);
        var organization = await organizationServiceClient.Admin_GetAsync(
            new Admin_GetInput
            {
                Id = organizationId,
            },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
        ArgumentNullException.ThrowIfNull(organization);

        return (organization, bankAccount);
    }

    public static (DateTimeOffset StartInclusive, DateTimeOffset EndInclusive) ResolveRecurringInvoiceDisplayPeriod(
        RecurringBooking recurringBooking,
        RecurringInvoiceBillingDefinition billingDefinition)
    {
        ArgumentNullException.ThrowIfNull(recurringBooking.MarketplaceBooking);

        var fullTermStart = recurringBooking.StartDate;
        var fullTermEndInclusive = recurringBooking.EndDate ?? recurringBooking.StartDate;

        if (billingDefinition.Source != XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle)
        {
            return (fullTermStart, fullTermEndInclusive);
        }

        var periodStart = recurringBooking.CreatedAt > fullTermStart
            ? recurringBooking.CreatedAt
            : fullTermStart;
        var periodEndExclusive = billingDefinition.Cadence switch
        {
            ProductPricingCadence.Daily => periodStart.AddDays(1),
            ProductPricingCadence.Weekly => periodStart.AddDays(7),
            ProductPricingCadence.Fortnightly => periodStart.AddDays(14),
            ProductPricingCadence.Monthly => periodStart.AddMonths(1),
            ProductPricingCadence.TwoMonths => periodStart.AddMonths(2),
            ProductPricingCadence.Quarterly => periodStart.AddMonths(3),
            ProductPricingCadence.FourMonths => periodStart.AddMonths(4),
            ProductPricingCadence.FiveMonths => periodStart.AddMonths(5),
            ProductPricingCadence.SixMonths => periodStart.AddMonths(6),
            ProductPricingCadence.Yearly => periodStart.AddYears(1),
            _ => periodStart.AddDays(1),
        };
        var periodEndInclusive = periodEndExclusive.AddDays(-1);

        return (
            periodStart,
            periodEndInclusive > fullTermEndInclusive
                ? fullTermEndInclusive
                : periodEndInclusive);
    }

    private abstract class InvoiceDocumentBase(
        BankAccount bankAccount,
        Organization organization,
        ProductVersion productVersion,
        DateTimeOffset dueDate) : IDocument
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
                    if (IsRegisteredForTax(taxDetails))
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
                column.Item().Component(new TotalsAmountComponent(currency, GetTotalAmount()));

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

        protected static bool IsRegisteredForTax(TaxDetails? taxDetails) =>
            taxDetails is { IsRegistered: true };
    }

    private class BookingInvoiceDocument(
        Database.Entities.Booking booking,
        BankAccount bankAccount,
        Organization organization,
        ProductVersion productVersion,
        DateTimeOffset dueDate,
        IProductVersionHelperService productVersionHelperService)
        : InvoiceDocumentBase(bankAccount, organization, productVersion, dueDate)
    {
        protected override DateTimeOffset GetInvoiceDate() => booking.CreatedAt;
        protected override string? GetInvoiceNumber() => booking.MarketplaceBooking?.InvoiceNumber;
        protected override string GetPaymentMethod() => booking.MarketplaceBooking?.PaymentMethod ?? string.Empty;
        protected override decimal GetTotalAmountExcludeTax() => booking.MarketplaceBooking?.TotalAmountExcludeTax ?? 0;
        protected override decimal GetTaxAmount() => booking.MarketplaceBooking?.TaxAmount ?? 0;
        protected override decimal GetTaxRatePercentage() => booking.MarketplaceBooking?.TaxRatePercentage ?? 0;
        protected override decimal GetTotalAmount() => booking.MarketplaceBooking?.TotalAmount ?? 0;

        protected override string GetDescription()
        {
            var marketplaceBooking = booking.MarketplaceBooking;
            ArgumentNullException.ThrowIfNull(marketplaceBooking);

            return BuildBookingInvoiceLineDescription(
                ProductVersion,
                marketplaceBooking.ProductPricing,
                FormatInvoicePeriod(booking.From, booking.Until));
        }

        protected override decimal GetQuantity()
        {
            var marketplaceBooking = booking.MarketplaceBooking;
            ArgumentNullException.ThrowIfNull(marketplaceBooking);

            var pricing = ResolvePricing();
            return marketplaceBooking.Quantity;
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
            return $"{ResolveUnitPrice().ToRoundedPrice()} {pricing.PurchaseCadence.ToInvoicePriceUnitName()}";
        }

        private ProductPricing ResolvePricing()
        {
            var marketplaceBooking = booking.MarketplaceBooking;
            ArgumentNullException.ThrowIfNull(marketplaceBooking);
            ArgumentNullException.ThrowIfNull(ProductVersion.PricingOptions);

            var pricing = productVersionHelperService.FindMatchingPricing([.. ProductVersion.PricingOptions], marketplaceBooking.ProductPricing);
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

    private sealed class EntitlementInvoiceDocument(
        EntitlementPurchase purchase,
        BankAccount bankAccount,
        Organization organization,
        ProductVersion productVersion,
        DateTimeOffset dueDate)
        : InvoiceDocumentBase(bankAccount, organization, productVersion, dueDate)
    {
        protected override DateTimeOffset GetInvoiceDate() => purchase.CreatedAt;
        protected override string? GetInvoiceNumber() => purchase.InvoiceNumber;

        protected override string GetDescription() =>
            BuildEntitlementInvoiceLineDescription(ProductVersion, purchase.ProductPricing, purchase.ServiceStartAt);

        protected override decimal GetQuantity() => 1;
        protected override decimal GetLineAmount() => purchase.Amount;
        protected override string GetUnitPriceLabel() => purchase.Amount.ToRoundedPrice();
        protected override string GetPaymentMethod() => purchase.PaymentMethod;
        protected override decimal GetTotalAmount() => purchase.Amount;

        protected override decimal GetTaxRatePercentage() =>
            Organization.TaxDetails is { IsRegistered: true } taxDetails ? Convert.ToDecimal(taxDetails.TaxRatePercentage) : 0m;

        protected override decimal GetTotalAmountExcludeTax() =>
            purchase.ProductPricing.IsTaxInclusive
                ? purchase.Amount / (1m + GetTaxRatePercentage() / 100m)
                : purchase.Amount;

        protected override decimal GetTaxAmount() =>
            purchase.ProductPricing.IsTaxInclusive
                ? purchase.Amount - GetTotalAmountExcludeTax()
                : GetTotalAmountExcludeTax() * GetTaxRatePercentage() / 100m;
    }

    private class RecurringInvoiceDocument(
        RecurringBooking recurringBooking,
        BankAccount bankAccount,
        Organization organization,
        ProductVersion productVersion,
        DateTimeOffset dueDate,
        RecurringInvoiceBillingDefinition billingDefinition,
        ArrearsInvoiceDraftLine? initialArrearsLine = null)
        : InvoiceDocumentBase(bankAccount, organization, productVersion, dueDate)
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
            if (initialArrearsLine is not null)
            {
                return initialArrearsLine.Description;
            }

            var marketplaceBooking = recurringBooking.MarketplaceBooking;
            ArgumentNullException.ThrowIfNull(marketplaceBooking);
            var (displayStart, displayEnd) = ResolveRecurringInvoiceDisplayPeriod(recurringBooking, billingDefinition);

            return BuildBookingInvoiceLineDescription(
                ProductVersion,
                marketplaceBooking.ProductPricing,
                $"{marketplaceBooking.ProductPricing.PurchaseCadence.ToProductPricingCadenceName()} pass{Environment.NewLine}" +
                $"{displayStart.ToShortDate()} - {displayEnd.ToShortDate()}");
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

            return initialArrearsLine is not null
                ? $"{unitPrice.ToRoundedPrice()} {marketplaceBooking.ProductVersion.Product.Organization.BillingCycle.ToOrganizationBillingCycle() switch
                {
                    OrganizationBillingCycle.Weekly => "weekly",
                    OrganizationBillingCycle.Fortnightly => "fortnightly",
                    OrganizationBillingCycle.Monthly => "monthly",
                    _ => marketplaceBooking.ProductPricing.PurchaseCadence.ToInvoicePriceUnitName(),
                }}"
                : $"{unitPrice.ToRoundedPrice()} {billingDefinition.Cadence.ToInvoicePriceUnitName()}";
        }

        private (decimal TotalAmountExcludeTax, decimal TaxAmount, decimal TaxRatePercentage, decimal TotalAmount) CalculateRecurringAmounts()
        {
            var marketplaceBooking = recurringBooking.MarketplaceBooking;
            ArgumentNullException.ThrowIfNull(marketplaceBooking);

            if (marketplaceBooking.BillingMode.ToProductPricingBillingMode() == ProductPricingBillingMode.InArrears &&
                marketplaceBooking is { TotalAmountExcludeTax: not null, TaxAmount: not null, TaxRatePercentage: not null, TotalAmount: not null })
            {
                return (
                    marketplaceBooking.TotalAmountExcludeTax.Value,
                    marketplaceBooking.TaxAmount.Value,
                    marketplaceBooking.TaxRatePercentage.Value,
                    marketplaceBooking.TotalAmount.Value);
            }

            var totalPrice = billingDefinition.InvoiceAmount.RoundedDecimal();
            if (!IsRegisteredForTax(Organization.TaxDetails))
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

    private class TotalsAmountComponent(string currency, decimal totalAmount) : IComponent
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
                table.Cell().AlignRight().Text($"Amount {currency.ToInvoiceCurrencyName()}").Bold();
                table.Cell().PaddingBottom(5).AlignRight().Text(totalAmountText);

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
