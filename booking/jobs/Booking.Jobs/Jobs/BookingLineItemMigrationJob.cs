using Booking.Shared.Repositories;

namespace Booking.Jobs.Jobs;

public class BookingLineItemMigrationJob(IServiceProvider serviceProvider, ILogger<BookingLineItemMigrationJob> logger) : BackgroundService
{
    private readonly string _jobName = typeof(BookingLineItemMigrationJob).FullName!;

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
                    booking.LineItems ??= [];
                    repositoryFactory.BookingRepository.Update(booking);
                }

                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
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
