using Booking.Shared.Repositories;
using Enterprise.Shared.Time;

namespace Booking.Jobs.Jobs;

public class BookingTimeSyncJob(IServiceProvider serviceProvider, ILogger<DeskRoomToResourceSyncJob> logger) : BackgroundService
{
    private readonly string _jobName = typeof(DeskRoomToResourceSyncJob).FullName!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var bookings = await repositoryFactory.BookingRepository.GetAllAsync(cancellationToken);

                foreach (var booking in bookings)
                {
                    if (booking.To is not { Minute: 59, Second: 59 })
                    {
                        continue;
                    }

                    booking.To = booking.To.StartOfDay().AddDays(1);
                    repositoryFactory.BookingRepository.Update(booking);
                }

                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

                logger.LogInformation("Finished running job: {job}", _jobName);

                await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", _jobName);
            }
        } while (true);
    }
}
