using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Services;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;

namespace Booking.Shared.UnitTests.Services.OrganizationArrearsInvoiceServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GenerateInvoiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_A_Document_For_An_Empty_Draft(OrganizationArrearsInvoiceService sut)
    {
        var organization = new OrganizationEntity
        {
            Id = "org-1",
            Name = "Test Organization",
        };
        var draft = new ArrearsInvoiceDraft(
            "org-1",
            "customer-1",
            Currency.Nzd,
            new BillingPeriod(
                new DateTimeOffset(2026, 8, 1, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 9, 1, 0, 0, 0, TimeSpan.Zero)),
            []);

        sut.GenerateInvoice(organization, draft, "INV-1").ShouldNotBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Expose_Default_Document_Metadata(OrganizationArrearsInvoiceService sut)
    {
        var organization = new OrganizationEntity
        {
            Id = "org-1",
            Name = "Test Organization",
        };
        var draft = new ArrearsInvoiceDraft(
            "org-1", "customer-1", Currency.Nzd,
            new BillingPeriod(
                new DateTimeOffset(2026, 8, 1, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 9, 1, 0, 0, 0, TimeSpan.Zero)), []);

        sut.GenerateInvoice(organization, draft, "INV-1").GetMetadata().ShouldNotBeNull();
    }
}
