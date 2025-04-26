using Booking.Shared.Repositories;

namespace Booking.Jobs.Jobs;

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
                    booking.InvolvedCustomers = [booking.Customer];
                    booking.InvolvedOrganizations = booking.Organization is null ? [] : [booking.Organization];
                    booking.InvolvedLocations = booking.ResourceBookingSlots
                        .Where(item => item.Resource.Location is not null)
                        .Select(item => item.Resource.Location)
                        .GroupBy(item => item!.Id)
                        .Select(item => item.First())
                        .ToList()!;
                    booking.InvolvedTeams = booking.Team is null ? [] : [booking.Team];

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
