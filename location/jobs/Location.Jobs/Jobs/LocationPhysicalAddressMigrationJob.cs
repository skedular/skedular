using Enterprise.Shared.Random;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;

namespace Location.Jobs.Jobs;

public class LocationPhysicalAddressMigrationJob(
    IServiceProvider serviceProvider,
    IRandomHelper randomHelper,
    ILogger<LocationPhysicalAddressMigrationJob> logger)
    : BackgroundService
{
    private readonly string _jobName = typeof(LocationPhysicalAddressMigrationJob).FullName!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();

                var locations = await repositoryFactory.LocationRepository.GetAllAsync(false, cancellationToken);
                foreach (var location in locations)
                {
                    if (location.Address is null)
                    {
                        continue;
                    }

                    if (location.PhysicalAddress is not null)
                    {
                        continue;
                    }

                    repositoryFactory.LocationPhysicalAddressRepository.Add(new LocationPhysicalAddress
                    {
                        Id = randomHelper.Generate(),
                        Latitude = location.Address.Latitude,
                        Longitude = location.Address.Longitude,
                        AddressLine1 = location.Address.AddressLine1,
                        AddressLine2 = location.Address.AddressLine2,
                        Suburb = location.Address.Suburb,
                        City = location.Address.City,
                        Province = location.Address.Province,
                        Zipcode = location.Address.Zipcode,
                        Country = location.Address.Country,
                        Location = location
                    });

                    await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                }

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
