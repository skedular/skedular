using Api.Shared.Services.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Booking.Jobs.Jobs;

public class MigrateBookingSchedules(IServiceProvider serviceProvider, ILogger<MigrateBookingSchedules> logger) : BackgroundService
{
    private readonly string _jobName = typeof(MigrateBookingSchedules).FullName!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();

                var bookings = await repositoryFactory.BookingRepository
                    .Query(new Specification<Shared.Database.Entities.Booking>())
                    .ToListAsync(cancellationToken);

                bookings.ForEach(booking =>
                {
                    booking.BookingSchedules = new BookingSchedules(new List<BookingSchedule> { new(booking.From, booking.Until) });
                    repositoryFactory.BookingRepository.Update(booking);
                });

                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

                await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
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
