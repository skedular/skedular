using Booking.Shared.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Organization = Booking.Shared.Database.Entities.Organization;

namespace Booking.Shared.Services;

public interface IOrganizationArrearsInvoiceService
{
    IDocument GenerateInvoice(Organization organization, ArrearsInvoiceDraft draft, string invoiceNumber);
}

public class OrganizationArrearsInvoiceService : IOrganizationArrearsInvoiceService
{
    public IDocument GenerateInvoice(Organization organization, ArrearsInvoiceDraft draft, string invoiceNumber) =>
        new OrganizationArrearsInvoiceDocument(organization, draft, invoiceNumber);

    private sealed class OrganizationArrearsInvoiceDocument(Organization organization, ArrearsInvoiceDraft draft, string invoiceNumber) : IDocument
    {
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container) =>
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(column =>
                {
                    column.Item().Text(organization.Name ?? "Organization").FontSize(18).Bold();
                    column.Item().Text($"Invoice #{invoiceNumber}");
                    column.Item().Text(
                        $"Billing period: {draft.BillingPeriod.StartInclusive:yyyy-MM-dd} - {draft.BillingPeriod.EndExclusive:yyyy-MM-dd}");
                });

                page.Content().PaddingVertical(20).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(4);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Description").Bold();
                        header.Cell().Text("From").Bold();
                        header.Cell().Text("Until").Bold();
                        header.Cell().AlignRight().Text("Amount").Bold();
                    });

                    foreach (var line in draft.Lines)
                    {
                        table.Cell().Text(line.Description);
                        table.Cell().Text(line.ServicePeriod.StartInclusive.ToString("yyyy-MM-dd"));
                        table.Cell().Text(line.ServicePeriod.EndExclusive.ToString("yyyy-MM-dd"));
                        table.Cell().AlignRight().Text($"{line.Amount:0.00} {draft.Currency}");
                    }
                });

                page.Footer().AlignRight().Text($"Total: {draft.TotalAmount:0.00} {draft.Currency}").Bold();
            });
    }
}
