using Team.Shared.Repositories;

namespace Team.Jobs.Jobs;

public class BookingDataModelMigrationJob(IServiceProvider serviceProvider, ILogger<BookingDataModelMigrationJob> logger) : BackgroundService
{
    private readonly string _jobName = typeof(BookingDataModelMigrationJob).FullName!;

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
                    booking.InvolvedTeams = [booking.Team];
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
