using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.InvoicePaymentTermsServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetDueDateShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Add_Organization_Due_Days_To_Invoice_Date(
        InvoicePaymentTermsService sut,
        DateTimeOffset invoiceDate,
        int invoiceDueInDays)
    {
        invoiceDueInDays = Math.Abs(invoiceDueInDays % 365) + 1;

        var result = sut.GetDueDate(invoiceDate, invoiceDueInDays);

        result.ShouldBe(new DateTimeOffset(invoiceDate.UtcDateTime.Date.AddDays(invoiceDueInDays), TimeSpan.Zero));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Fall_Back_To_Default_Due_Days_When_Organization_Value_Is_Not_Positive(
        InvoicePaymentTermsService sut,
        DateTimeOffset invoiceDate)
    {
        var result = sut.GetDueDate(invoiceDate, 0);

        result.ShouldBe(
            new DateTimeOffset(
                invoiceDate.UtcDateTime.Date.AddDays(InvoicePaymentTermsService.DefaultInvoiceDueInDays),
                TimeSpan.Zero));
    }
}
