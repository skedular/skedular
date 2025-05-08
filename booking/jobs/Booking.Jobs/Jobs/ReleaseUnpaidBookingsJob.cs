using Booking.Shared.Publishers;
using Booking.Shared.Repositories;

namespace Booking.Jobs.Jobs;

public class ReleaseUnpaidBookingsJob(IServiceProvider serviceProvider, ILogger<ReleaseUnpaidBookingsJob> logger) : BackgroundService
{
    private readonly string _jobName = typeof(ReleaseUnpaidBookingsJob).FullName!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var bookingInternalPublisher = scope.ServiceProvider.GetRequiredService<IBookingInternalPublisher>();
                var bookings = await repositoryFactory.BookingRepository.GetAllExpiredBookingsAsync(cancellationToken);

                if (bookings.Count != 0)
                {
                    await bookingInternalPublisher.PublishPurgeExpiredBookingAsync(bookings.Select(item => item.Id), cancellationToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
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
