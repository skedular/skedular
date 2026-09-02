using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.OrganizationArrearsBillingPlannerServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class OrganizationArrearsBillingPlannerServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Group_Charge_Segments_Into_Per_Customer_Drafts(
        [Frozen]
        IOrganizationArrearsChargeSegmentService organizationArrearsChargeSegmentService,
        OrganizationArrearsBillingPlannerService sut)
    {
        var billingPeriod = new BillingPeriod(
            new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero));
        var firstBooking = CreateBooking(
            ProductPricingCadence.Daily,
            10m,
            1,
            new DateTimeOffset(2026, 3, 10, 8, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 3, 10, 10, 0, 0, TimeSpan.Zero));
        var secondBooking = CreateBooking(
            ProductPricingCadence.Daily,
            25m,
            1,
            new DateTimeOffset(2026, 3, 11, 8, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 3, 11, 12, 0, 0, TimeSpan.Zero));

        A.CallTo(() => organizationArrearsChargeSegmentService.BuildChargeSegments(firstBooking, OrganizationBillingCycle.Monthly))
            .Returns(
            [
                new ArrearsChargeSegment(
                    "booking-1:customer-1",
                    firstBooking.Id,
                    "org-1",
                    "customer-1",
                    Currency.Nzd,
                    new BillingPeriod(firstBooking.From, firstBooking.Until),
                    firstBooking.Until,
                    20m,
                    "Area Pass"),
            ]);
        A.CallTo(() => organizationArrearsChargeSegmentService.BuildChargeSegments(secondBooking, OrganizationBillingCycle.Monthly))
            .Returns(
            [
                new ArrearsChargeSegment(
                    "booking-2:customer-1",
                    secondBooking.Id,
                    "org-1",
                    "customer-1",
                    Currency.Nzd,
                    new BillingPeriod(secondBooking.From, secondBooking.Until),
                    secondBooking.Until,
                    25m,
                    "Area Pass"),
            ]);

        var result = sut.BuildInvoiceDrafts(billingPeriod, OrganizationBillingCycle.Monthly, [firstBooking, secondBooking]);

        result.Count.ShouldBe(1);
        result.Single().CustomerId.ShouldBe("customer-1");
        result.Single().Lines.Count.ShouldBe(2);
        result.Single().TotalAmount.ShouldBe(45m);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Exclude_Already_Processed_Charge_Segments_From_Drafts(
        [Frozen]
        IOrganizationArrearsChargeSegmentService organizationArrearsChargeSegmentService,
        OrganizationArrearsBillingPlannerService sut)
    {
        var billingPeriod = new BillingPeriod(
            new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero));
        var booking = CreateBooking(
            ProductPricingCadence.Daily,
            25m,
            1,
            new DateTimeOffset(2026, 3, 11, 8, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 3, 11, 12, 0, 0, TimeSpan.Zero));
        const string excludedSegmentKey = "booking-1:customer-1";
        A.CallTo(() => organizationArrearsChargeSegmentService.BuildChargeSegments(booking, OrganizationBillingCycle.Monthly))
            .Returns(
            [
                new ArrearsChargeSegment(
                    excludedSegmentKey,
                    booking.Id,
                    "org-1",
                    "customer-1",
                    Currency.Nzd,
                    new BillingPeriod(booking.From, booking.Until),
                    booking.Until,
                    25m,
                    "Area Pass"),
            ]);

        var result = sut.BuildInvoiceDrafts(billingPeriod, OrganizationBillingCycle.Monthly, [booking], [excludedSegmentKey]);

        result.ShouldBeEmpty();
    }

    private static Shared.Models.Booking CreateBooking(
        ProductPricingCadence purchaseCadence,
        decimal price,
        int quantity,
        DateTimeOffset from,
        DateTimeOffset until,
        ProductPricingBillingMode billingMode = ProductPricingBillingMode.InArrears) =>
        new()
        {
            Id = "booking-1",
            From = from,
            Until = until,
            InvolvedOrganizations =
            [
                new Organization
                {
                    Id = "org-1",
                },
            ],
            InvolvedCustomers =
            [
                new Customer
                {
                    Id = "customer-1",
                },
            ],
            CreatedByCustomer = new Customer
            {
                Id = "customer-1",
            },
            MarketplaceBooking = new MarketplaceBooking
            {
                Quantity = quantity,
                Currency = Currency.Nzd,
                BillingMode = billingMode,
                ProductPricing = ProductPricing.Empty("pricing-1") with
                {
                    PurchaseCadence = purchaseCadence,
                    Price = price,
                    BillingMode = billingMode,
                    ListingMetadata = ListingMetadata.Empty with
                    {
                        Title = "Area Pass",
                    },
                },
            },
        };
}
