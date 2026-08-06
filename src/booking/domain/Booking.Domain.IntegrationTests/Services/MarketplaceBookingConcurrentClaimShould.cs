using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Microsoft.Extensions.DependencyInjection;
using BookingEntity = Booking.Shared.Database.Entities.Booking;
using ResourceEntity = Booking.Shared.Database.Entities.Resource;
using ResourceBookingSlotEntity = Booking.Shared.Database.Entities.ResourceBookingSlot;

namespace Booking.Domain.IntegrationTests.Services;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class MarketplaceBookingConcurrentClaimShould(
    IRepositoryFactory repositoryFactory,
    IServiceScopeFactory scopeFactory)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Persist_One_Winner_And_One_Availability_Failure_Loser(
        string resourceId,
        string firstBookingId,
        string secondBookingId,
        CancellationToken cancellationToken)
    {
        var from = new DateTimeOffset(2026, 7, 26, 9, 0, 0, TimeSpan.Zero);
        var resource = repositoryFactory.ResourceRepository.Add(new ResourceEntity
        {
            Id = resourceId,
            Capacity = 1,
        });
        repositoryFactory.ResourceBookingSlotRepository.AddRange([
            new ResourceBookingSlotEntity
            {
                Id = $"{resourceId}-slot",
                Resource = resource,
                ResourceId = resourceId,
                Start = from,
                Available = true,
            },
        ]);
        AddBooking(firstBookingId, from);
        AddBooking(secondBookingId, from);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var results = await Task.WhenAll(
            TryClaimAsync(firstBookingId, resourceId, cancellationToken),
            TryClaimAsync(secondBookingId, resourceId, cancellationToken));

        results.Count(item => item.Claimed).ShouldBe(1);
        results.Count(item => !item.Claimed).ShouldBe(1);
        results.Single(item => !item.Claimed).UnavailableResourceIds.ShouldContain(resourceId);
    }

    private void AddBooking(string bookingId, DateTimeOffset from) => repositoryFactory.BookingRepository.Add(new BookingEntity
    {
        Id = bookingId,
        From = from,
        Until = from.AddHours(1),
        Category = BookingCategory.WorkingFromCoworkingSpace.ToBookingCategory(),
        Channel = BookingChannel.Marketplace.ToBookingChannel(),
        Schedules = [],
    });

    private async Task<ResourceSlotClaimResult> TryClaimAsync(
        string bookingId,
        string resourceId,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var scopedRepositories = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
        var booking = await scopedRepositories.BookingRepository.GetByIdAsync(bookingId, cancellationToken);
        booking.ShouldNotBeNull();
        return await scopedRepositories.ResourceRepository.TryClaimCompleteSlotSetAsync(booking!, [resourceId], cancellationToken);
    }
}
