using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.InvoicePaymentTermsServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetInvoiceDueInDaysShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_Organization_Value_When_Positive(
        InvoicePaymentTermsService sut,
        int invoiceDueInDays)
    {
        invoiceDueInDays = Math.Abs(invoiceDueInDays % 365) + 1;

        var result = sut.GetInvoiceDueInDays(invoiceDueInDays);

        result.ShouldBe(invoiceDueInDays);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Default_Value_When_Organization_Value_Is_Not_Positive(
        InvoicePaymentTermsService sut)
    {
        var result = sut.GetInvoiceDueInDays(0);

        result.ShouldBe(InvoicePaymentTermsService.DefaultInvoiceDueInDays);
    }
}
