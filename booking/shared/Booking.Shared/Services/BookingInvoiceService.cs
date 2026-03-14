using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Api.Shared.Services.Models;
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

public interface IBookingInvoiceService
{
    Task<IDocument?> GenerateInvoiceAsync(string bookingId, bool fullyPaid, CancellationToken cancellationToken);
}

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
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(marketplaceBooking.ProductVersion.Id, cancellationToken);
        if (productVersion is null)
        {
            throw new ProductVersionNotFound();
        }

        var organizationId = productVersion.Product.Organization.Id;
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

        return new InvoiceDocument(
            booking,
            bankAccount,
            organization,
            productVersion,
            marketplaceBooking.PaymentExpiry,
            fullyPaid,
            productVersionHelperService);
    }

    private class InvoiceDocument(
        Database.Entities.Booking booking,
        BankAccount bankAccount,
        Organization organization,
        ProductVersion productVersion,
        DateTimeOffset dueDate,
        bool fullyPaid,
        IProductVersionHelperService productVersionHelperService) : IDocument
    {
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
                    var marketplaceBooking = booking.MarketplaceBooking;
                    ArgumentNullException.ThrowIfNull(marketplaceBooking);

                    column.Item().Text("Invoice Date").SemiBold().FontSize(13);
                    column.Item().Text(booking.CreatedAt.ToShortDate()).FontSize(10);

                    if (!string.IsNullOrWhiteSpace(marketplaceBooking.InvoiceNumber))
                    {
                        column.Item().Text(string.Empty);
                        column.Item().Text("Invoice Number").SemiBold().FontSize(13);
                        column.Item().Text(marketplaceBooking.InvoiceNumber).FontSize(10);
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

                column.Item().Element(ComposeTable);
                column.Item().Component(new TotalExcludeGstComponent(booking));
                column.Item().Component(new TotalAmountComponent(booking, productVersion, fullyPaid));

                var marketplaceBooking = booking.MarketplaceBooking;
                ArgumentNullException.ThrowIfNull(marketplaceBooking);

                if (marketplaceBooking.PaymentMethod == PaymentMethodConstants.BankTransfer)
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

                var marketplaceBooking = booking.MarketplaceBooking;
                ArgumentNullException.ThrowIfNull(marketplaceBooking);

                ArgumentNullException.ThrowIfNull(productVersion.PricingOptions);

                var pricing = productVersionHelperService.FindMatchingPricing(productVersion.PricingOptions,
                    marketplaceBooking.ProductPricing);

                ArgumentNullException.ThrowIfNull(pricing);

                table.Cell().Element(CellStyle).Padding(8)
                    .Text(
                        $"{productVersion.ListingMetadata?.Title}{Environment.NewLine}{booking.From.ToShortDate()}{Environment.NewLine}{booking.From.ToShortTime()} - {booking.Until.ToShortTime()}");

                var totalMinutes = (int)(booking.Until - booking.From).TotalMinutes;
                var quantity = pricing.Cadence switch
                {
                    ProductPricingCadence.OneTime => marketplaceBooking.Quantity,
                    ProductPricingCadence.PerMinute => marketplaceBooking.Quantity * totalMinutes,
                    ProductPricingCadence.Per15Minutes => marketplaceBooking.Quantity * (totalMinutes / 15),
                    ProductPricingCadence.Per30Minutes => marketplaceBooking.Quantity * (totalMinutes / 30),
                    ProductPricingCadence.PerHour => marketplaceBooking.Quantity * (totalMinutes / 60),
                    // TODO: 20260302 : Morteza: Implement other cadence 
                    _ => throw new ArgumentOutOfRangeException()
                };

                table.Cell().Element(CellStyle).AlignRight().Text(quantity.ToString());

                var price = organization.TaxDetails is null
                    ? pricing.Price
                    : pricing.IsTaxInclusive
                        ? pricing.Price * 100 / (Convert.ToDecimal(organization.TaxDetails.TaxRatePercentage) + 100)
                        : pricing.Price;

                table.Cell().Element(CellStyle).AlignRight()
                    .Text($"{price.ToRoundedPrice()} {pricing.Cadence.ToInvoicePriceUnitName()}");

                var totalPrice = pricing.Cadence switch
                {
                    ProductPricingCadence.OneTime => price * quantity,
                    ProductPricingCadence.PerMinute => price * quantity,
                    ProductPricingCadence.Per15Minutes => price * quantity,
                    ProductPricingCadence.Per30Minutes => price * quantity,
                    ProductPricingCadence.PerHour => price * quantity,
                    // TODO: 20260302 : Morteza: Implement other cadence 
                    _ => throw new ArgumentOutOfRangeException()
                };

                table.Cell().Element(CellStyle).AlignRight().Text(totalPrice.ToRoundedPrice());

                static IContainer CellStyle(IContainer container)
                {
                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                }
            });

        private class TotalExcludeGstComponent(Database.Entities.Booking booking) : IComponent
        {
            public void Compose(IContainer container) =>
                container.Table(table =>
                {
                    var marketplaceBooking = booking.MarketplaceBooking;
                    ArgumentNullException.ThrowIfNull(marketplaceBooking);

                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(5);
                        columns.RelativeColumn();
                    });

                    table.Cell().AlignRight().Text("Subtotal");
                    table.Cell().PaddingBottom(5).AlignRight().Text(marketplaceBooking.TotalAmountExcludeTax!.Value.ToRoundedPrice());

                    table.Cell().AlignRight().Text($"TOTAL GST {marketplaceBooking.TaxRatePercentage!.Value.RoundedDecimal()}%");
                    table.Cell().PaddingBottom(5).AlignRight().Text(marketplaceBooking.TaxAmount!.Value.ToRoundedPrice());

                    table.Cell().PaddingLeft(300).LineHorizontal(1);
                    table.Cell().LineHorizontal(1);
                });
        }

        private class TotalAmountComponent(Database.Entities.Booking booking, ProductVersion productVersion, bool fullyPaid)
            : IComponent
        {
            public void Compose(IContainer container)
            {
                var currency = productVersion.Currency;
                ArgumentException.ThrowIfNullOrWhiteSpace(currency);

                var marketplaceBooking = booking.MarketplaceBooking;
                ArgumentNullException.ThrowIfNull(marketplaceBooking);

                container.Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(5);
                        columns.RelativeColumn();
                    });

                    var totalAmount = marketplaceBooking.TotalAmount!.Value.ToRoundedPrice();
                    table.Cell().AlignRight().Text($"TOTAL {currency.ToInvoiceCurrencyName()}").Bold();
                    table.Cell().PaddingBottom(5).AlignRight().Text(totalAmount);

                    if (fullyPaid)
                    {
                        table.Cell().AlignRight().Text("Amount Paid");
                        table.Cell().PaddingBottom(5).AlignRight().Text(totalAmount);
                    }
                    else
                    {
                        table.Cell().AlignRight().Text("Amount Paid");
                        table.Cell().PaddingBottom(5).AlignRight().Text("0.00");
                    }

                    if (fullyPaid)
                    {
                        table.Cell().AlignRight().Text("Amount Due");
                        table.Cell().PaddingBottom(5).AlignRight().Text("0.00");
                    }
                    else
                    {
                        table.Cell().AlignRight().Text("Amount Due");
                        table.Cell().PaddingBottom(5).AlignRight().Text(totalAmount);
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
}
