using AutoFixture;
using Booking.Api.GraphQL.Booking;
using Booking.Shared.Database.Entities;
using BookingModel = Booking.Shared.Models.Booking;
using BookingEntity = Booking.Shared.Database.Entities.Booking;

namespace Booking.Api.UnitTests.GraphQL.Booking.RootQueryTests;

public sealed class BookingFailureQueryFixtureCustomizer : IFixtureCustomizer
{
    public void Customize(IFixture fixture)
    {
        foreach (var behavior in fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList())
        {
            fixture.Behaviors.Remove(behavior);
        }

        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        fixture.Register(() => new MarketplaceBookingDetails
        {
            Id = Guid.CreateVersion7().ToString("N"),
        });
        fixture.Register(() => new BookingEntity
        {
            Id = Guid.CreateVersion7().ToString("N"),
        });
        fixture.Register(() => new MarketplaceBookingFailure
        {
            Id = Guid.CreateVersion7().ToString("N"),
        });
        fixture.Register(() => new MarketplaceBookingFailureDetails
        {
            Id = Guid.CreateVersion7().ToString("N"),
        });
        fixture.Register(() => new BookingModel
        {
            Id = Guid.CreateVersion7().ToString("N"),
        });
        fixture.Register(() => new BookingDetails
        {
            Id = Guid.CreateVersion7().ToString("N"),
        });
    }
}
