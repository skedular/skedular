using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Location.Shared.Repositories;

public interface ILocationBookingRecordingRepository : IRepository<DailyBookingCountRecording>
{
    Task ReplaceDailyRecordingsAsync(
        string locationId,
        IReadOnlyList<DailyBookingCountRecording> bookingRecordings,
        IReadOnlyList<DailyDeskBookingCountRecording> deskRecordings,
        IReadOnlyList<DailyRoomBookingCountRecording> roomRecordings,
        CancellationToken cancellationToken);
}

public class LocationBookingRecordingRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, DailyBookingCountRecording>(dbContext, timeProvider),
        ILocationBookingRecordingRepository
{
    public async Task ReplaceDailyRecordingsAsync(
        string locationId,
        IReadOnlyList<DailyBookingCountRecording> bookingRecordings,
        IReadOnlyList<DailyDeskBookingCountRecording> deskRecordings,
        IReadOnlyList<DailyRoomBookingCountRecording> roomRecordings,
        CancellationToken cancellationToken)
    {
        var existingDailyBookings = await DbContext.DailyBookingCountRecording
            .Where(item => item.Location.Id == locationId)
            .ToListAsync(cancellationToken);
        var existingDeskBookings = await DbContext.DailyDeskBookingCountRecording
            .Where(item => item.Location.Id == locationId)
            .ToListAsync(cancellationToken);
        var existingRoomBookings = await DbContext.DailyRoomBookingCountRecording
            .Where(item => item.Location.Id == locationId)
            .ToListAsync(cancellationToken);

        DbContext.DailyBookingCountRecording.RemoveRange(existingDailyBookings);
        DbContext.DailyDeskBookingCountRecording.RemoveRange(existingDeskBookings);
        DbContext.DailyRoomBookingCountRecording.RemoveRange(existingRoomBookings);
        await DbContext.DailyBookingCountRecording.AddRangeAsync(bookingRecordings, cancellationToken);
        await DbContext.DailyDeskBookingCountRecording.AddRangeAsync(deskRecordings, cancellationToken);
        await DbContext.DailyRoomBookingCountRecording.AddRangeAsync(roomRecordings, cancellationToken);
    }
}
