using Booking.Shared.Database.Entities;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Booking.Jobs.Jobs;

public class GenerateResourceBookingSlotJob(
    IServiceProvider serviceProvider,
    TimeProvider timeProvider,
    ILogger<GenerateResourceBookingSlotJob> logger)
    : BackgroundService
{
    private readonly string _jobName = typeof(GenerateResourceBookingSlotJob).FullName!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var bookingInternalPublisher = scope.ServiceProvider.GetRequiredService<IBookingInternalPublisher>();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var resourceIds = await repositoryFactory.ResourceRepository.Query(
                        new Specification<Resource> { Criteria = query => !query.DeletedAt.HasValue }).Select(item => item.Id)
                    .ToListAsync(cancellationToken);

                await bookingInternalPublisher.PublishGenerateResourceBookingSlotAsync(resourceIds, cancellationToken);

                await Task.Delay(TimeSpan.FromHours(6), cancellationToken);
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
