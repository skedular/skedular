using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Grpc.Skedular.Booking.Core.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;
using Microsoft.EntityFrameworkCore;
using Temporalio.Activities;
using LocationEntity = Location.Shared.Database.Entities.Location;
using ResourceEntity = Location.Shared.Database.Entities.Resource;

namespace Location.Shared.Activities;

public class LocationBookingDerivedState(
    IRepositoryFactory repositoryFactory,
    BookingConfiguration bookingConfiguration,
    BookingService.BookingServiceClient bookingServiceClient,
    IRandomHelper randomHelper,
    ICachedLocationService cachedLocationService)
{
    [Activity]
    public async Task RecomputeAsync(string locationId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken);
        if (location is null || location.IsDeleted())
        {
            return;
        }

        var bookings = await GetBookingsAsync(locationId, cancellationToken);
        var resourceIds = bookings.SelectMany(item => item.ResourceIds).Distinct().ToList();
        var resources = resourceIds.Count == 0
            ? new Dictionary<string, ResourceEntity>()
            : (await repositoryFactory.ResourceRepository.GetByIdsWithOrganizationTagsUntrackedAsync(resourceIds, cancellationToken))
            .ToDictionary(item => item.Id);

        await ReplaceDailyRecordingsAsync(location, bookings, resources, cancellationToken);

        _ = repositoryFactory.LocationRepository.Update(location);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        await cachedLocationService.UpdateByIdAsync(location.Id, cancellationToken);
    }

    private async Task ReplaceDailyRecordingsAsync(
        LocationEntity location,
        List<BookingSnapshot> bookings,
        Dictionary<string, ResourceEntity> resources,
        CancellationToken cancellationToken)
    {
        var existingDailyBookings = await repositoryFactory.DbContext.DailyBookingCountRecording
            .Where(item => item.Location.Id == location.Id)
            .ToListAsync(cancellationToken);
        var existingDeskBookings = await repositoryFactory.DbContext.DailyDeskBookingCountRecording
            .Where(item => item.Location.Id == location.Id)
            .ToListAsync(cancellationToken);
        var existingRoomBookings = await repositoryFactory.DbContext.DailyRoomBookingCountRecording
            .Where(item => item.Location.Id == location.Id)
            .ToListAsync(cancellationToken);

        repositoryFactory.DbContext.DailyBookingCountRecording.RemoveRange(existingDailyBookings);
        repositoryFactory.DbContext.DailyDeskBookingCountRecording.RemoveRange(existingDeskBookings);
        repositoryFactory.DbContext.DailyRoomBookingCountRecording.RemoveRange(existingRoomBookings);

        foreach (var groupedBooking in bookings.GroupBy(item => item.From.StartOfDay()))
        {
            var dayBookings = groupedBooking.ToList();

            _ = repositoryFactory.DbContext.DailyBookingCountRecording.Add(new DailyBookingCountRecording
            {
                Id = randomHelper.Generate(), Date = groupedBooking.Key, Count = dayBookings.Count, Location = location
            });

            _ = repositoryFactory.DbContext.DailyDeskBookingCountRecording.Add(new DailyDeskBookingCountRecording
            {
                Id = randomHelper.Generate(),
                Date = groupedBooking.Key,
                Count = dayBookings.Count(booking =>
                    booking.ResourceIds.Any(resourceId =>
                        resources.TryGetValue(resourceId, out var resource) &&
                        resource.OrganizationTags.Any(tag => tag.Type == OrganizationTagTypeConstants.ResourceDesk))),
                Location = location
            });

            _ = repositoryFactory.DbContext.DailyRoomBookingCountRecording.Add(new DailyRoomBookingCountRecording
            {
                Id = randomHelper.Generate(),
                Date = groupedBooking.Key,
                Count = dayBookings.Count(booking =>
                    booking.ResourceIds.Any(resourceId =>
                        resources.TryGetValue(resourceId, out var resource) &&
                        resource.OrganizationTags.Any(tag => tag.Type == OrganizationTagTypeConstants.ResourceRoom))),
                Location = location
            });
        }
    }

    private async Task<List<BookingSnapshot>> GetBookingsAsync(string locationId, CancellationToken cancellationToken)
    {
        // T008 investigation (2026-04-29): BookingRepository.GetPaginatedBookingsUntrackedAsync
        // applies .Where(item => !item.DeletedAt.HasValue) unconditionally (line ~147 of BookingRepository.cs).
        // Cancelled bookings are soft-deleted on the booking side, so they are already excluded by the
        // server before the gRPC response is sent. No additional client-side cancellation filter is needed here.
        // The BookingWhereInput.deletedByCustomerId field in the proto is unused for this flow.
        var result = new List<BookingSnapshot>();
        string? after = null;

        do
        {
            var response = await bookingServiceClient.Admin_GetPaginatedBookingsAsync(
                new Admin_GetPaginatedBookingsInput
                {
                    After = after ?? string.Empty,
                    First = 1000,
                    Before = string.Empty,
                    Last = ((int?)null).ToNullInt(),
                    Where = new BookingWhereInput { LocationIds = { locationId } },
                    OrderBy = { new BookingOrderInput { Direction = OrderDirection.Ascending, Field = BookingOrderField.From } }
                },
                bookingConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken);

            result.AddRange(response.Edges.Select(edge => new BookingSnapshot(
                edge.Node.From.ToDateTimeOffset(),
                edge.Node.Resources.Select(resource => resource.Id).ToList())));

            after = response.PageInfo.HasNextPage ? response.PageInfo.EndCursor : null;
        } while (!string.IsNullOrWhiteSpace(after));

        return result;
    }

    private sealed record BookingSnapshot(DateTimeOffset From, List<string> ResourceIds);
}
