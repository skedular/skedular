using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.OrganizationArrearsChargeSegmentServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class OrganizationArrearsChargeSegmentServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Build_Single_Per_Hour_Charge_Segment_For_In_Arrears_Booking(OrganizationArrearsChargeSegmentService sut)
    {
        var booking = CreateBooking(
            ProductPricingCadence.PerHour,
            ProductPricingCadence.PerHour,
            10m,
            2,
            new DateTimeOffset(2026, 3, 22, 8, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 3, 22, 12, 0, 0, TimeSpan.Zero));

        var result = sut.BuildChargeSegments(booking, OrganizationBillingCycle.Monthly);

        result.Count.ShouldBe(1);
        result.Single().Amount.ShouldBe(80m);
        result.Single().EarnedAt.ShouldBe(booking.Until.AddTicks(-1));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Prorate_Partial_Per_Hour_Charge_Segment_For_In_Arrears_Booking(OrganizationArrearsChargeSegmentService sut)
    {
        var booking = CreateBooking(
            ProductPricingCadence.PerHour,
            ProductPricingCadence.PerHour,
            10m,
            2,
            new DateTimeOffset(2026, 3, 22, 8, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 3, 22, 9, 30, 0, TimeSpan.Zero));

        var result = sut.BuildChargeSegments(booking, OrganizationBillingCycle.Monthly);

        result.Count.ShouldBe(1);
        result.Single().Amount.ShouldBe(30m);
        result.Single().EarnedAt.ShouldBe(booking.Until.AddTicks(-1));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Split_Multi_Month_Cadence_Into_Monthly_Installments_When_Billing_Cycle_Is_Monthly(
        OrganizationArrearsChargeSegmentService sut)
    {
        var booking = CreateBooking(
            ProductPricingCadence.Quarterly,
            ProductPricingCadence.Daily,
            100m,
            1,
            new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero));

        var result = sut.BuildChargeSegments(booking, OrganizationBillingCycle.Monthly);

        result.Count.ShouldBe(3);
        result.Select(item => item.Amount).Sum().ShouldBe(100m);
        result.Select(item => item.Amount).ToList().ShouldBe([34.44m, 31.11m, 34.45m]);
        result.Select(item => item.EarnedAt).ToList().ShouldBe([
            new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.Zero).AddTicks(-1),
            new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero).AddTicks(-1),
            new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero).AddTicks(-1)
        ]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Ignore_Upfront_Bookings(OrganizationArrearsChargeSegmentService sut)
    {
        var booking = CreateBooking(
            ProductPricingCadence.PerHour,
            ProductPricingCadence.PerHour,
            10m,
            1,
            new DateTimeOffset(2026, 3, 22, 8, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 3, 22, 12, 0, 0, TimeSpan.Zero),
            ProductPricingBillingMode.Upfront);

        var result = sut.BuildChargeSegments(booking, OrganizationBillingCycle.Monthly);

        result.ShouldBeEmpty();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Split_Six_Month_Cadence_Into_Weekly_Installments_When_Billing_Cycle_Is_Weekly(OrganizationArrearsChargeSegmentService sut)
    {
        var booking = CreateBooking(
            ProductPricingCadence.SixMonths,
            ProductPricingCadence.Daily,
            260m,
            1,
            new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 7, 2, 0, 0, 0, TimeSpan.Zero));

        var result = sut.BuildChargeSegments(booking, OrganizationBillingCycle.Weekly);

        result.Count.ShouldBeGreaterThan(20);
        result.First().ServicePeriod.ShouldBe(new BillingPeriod(
            new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 1, 5, 0, 0, 0, TimeSpan.Zero)));
        result.Last().ServicePeriod.ShouldBe(new BillingPeriod(
            new DateTimeOffset(2026, 6, 29, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 7, 2, 0, 0, 0, TimeSpan.Zero)));
        result.Select(item => item.Amount).Sum().ShouldBe(260m);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Prorate_Short_Final_Weekly_Period_When_Billing_Cycle_Is_Weekly(OrganizationArrearsChargeSegmentService sut)
    {
        var booking = CreateBooking(
            ProductPricingCadence.Monthly,
            ProductPricingCadence.Daily,
            100m,
            1,
            new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.Zero));

        var result = sut.BuildChargeSegments(booking, OrganizationBillingCycle.Weekly).ToList();

        result.Count.ShouldBe(5);
        result.Select(item => item.Amount).Sum().ShouldBe(100m);
        result.Select(item => item.Amount).ToList().ShouldBe([12.90m, 22.58m, 22.58m, 22.58m, 19.36m]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Align_Weekly_Installments_To_Monday_Billing_Boundaries(OrganizationArrearsChargeSegmentService sut)
    {
        var booking = CreateBooking(
            ProductPricingCadence.Monthly,
            ProductPricingCadence.Daily,
            140m,
            1,
            new DateTimeOffset(2026, 1, 7, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 1, 21, 0, 0, 0, TimeSpan.Zero));

        var result = sut.BuildChargeSegments(booking, OrganizationBillingCycle.Weekly).ToList();

        result.Select(item => item.ServicePeriod).ToList().ShouldBe(
        [
            new BillingPeriod(
                new DateTimeOffset(2026, 1, 7, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 1, 12, 0, 0, 0, TimeSpan.Zero)),
            new BillingPeriod(
                new DateTimeOffset(2026, 1, 12, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 1, 19, 0, 0, 0, TimeSpan.Zero)),
            new BillingPeriod(
                new DateTimeOffset(2026, 1, 19, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 1, 21, 0, 0, 0, TimeSpan.Zero))
        ]);
        result.Select(item => item.Amount).ToList().ShouldBe([50m, 70m, 20m]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Mark_Weekly_Boundary_Segment_As_Earned_Inside_The_Current_Billing_Period(OrganizationArrearsChargeSegmentService sut)
    {
        var booking = CreateBooking(
            ProductPricingCadence.Monthly,
            ProductPricingCadence.Daily,
            140m,
            1,
            new DateTimeOffset(2026, 1, 7, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 1, 21, 0, 0, 0, TimeSpan.Zero));

        var result = sut.BuildChargeSegments(booking, OrganizationBillingCycle.Weekly).ToList();

        result.Select(item => item.EarnedAt).ToList().ShouldBe(
        [
            new DateTimeOffset(2026, 1, 12, 0, 0, 0, TimeSpan.Zero).AddTicks(-1),
            new DateTimeOffset(2026, 1, 19, 0, 0, 0, TimeSpan.Zero).AddTicks(-1),
            new DateTimeOffset(2026, 1, 21, 0, 0, 0, TimeSpan.Zero).AddTicks(-1)
        ]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Align_Fortnightly_Installments_To_Fortnight_Billing_Boundaries(OrganizationArrearsChargeSegmentService sut)
    {
        var booking = CreateBooking(
            ProductPricingCadence.Quarterly,
            ProductPricingCadence.Daily,
            280m,
            1,
            new DateTimeOffset(2026, 3, 25, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 4, 23, 0, 0, 0, TimeSpan.Zero));

        var result = sut.BuildChargeSegments(booking, OrganizationBillingCycle.Fortnightly).ToList();

        result.Select(item => item.ServicePeriod).ToList().ShouldBe(
        [
            new BillingPeriod(
                new DateTimeOffset(2026, 3, 25, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 3, 30, 0, 0, 0, TimeSpan.Zero)),
            new BillingPeriod(
                new DateTimeOffset(2026, 3, 30, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 4, 13, 0, 0, 0, TimeSpan.Zero)),
            new BillingPeriod(
                new DateTimeOffset(2026, 4, 13, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 4, 23, 0, 0, 0, TimeSpan.Zero))
        ]);
        result.Select(item => item.Amount).ToList().ShouldBe([48.28m, 135.17m, 96.55m]);
    }

    private static Models.Booking CreateBooking(
        ProductPricingCadence purchaseCadence,
        ProductPricingCadence bookingCadence,
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
            InvolvedOrganizations = [new Organization { Id = "org-1" }],
            InvolvedCustomers = [new Customer { Id = "customer-1" }],
            CreatedByCustomer = new Customer { Id = "customer-1" },
            MarketplaceBooking = new MarketplaceBooking
            {
                Quantity = quantity,
                Currency = Currency.Nzd,
                BillingMode = billingMode,
                ProductPricing = ProductPricing.Empty("pricing-1") with
                {
                    PurchaseCadence = purchaseCadence,
                    BookingCadence = bookingCadence,
                    Price = price,
                    BillingMode = billingMode,
                    ListingMetadata = ListingMetadata.Empty with { Title = "Area Pass" }
                }
            }
        };
}
