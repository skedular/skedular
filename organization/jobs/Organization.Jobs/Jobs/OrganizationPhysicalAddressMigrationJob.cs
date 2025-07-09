using Enterprise.Shared.Random;
using Organization.Shared.Database.Entities;
using Organization.Shared.Repositories;

namespace Organization.Jobs.Jobs;

public class OrganizationPhysicalAddressMigrationJob(
    IServiceProvider serviceProvider,
    IRandomHelper randomHelper,
    ILogger<OrganizationPhysicalAddressMigrationJob> logger)
    : BackgroundService
{
    private readonly string _jobName = typeof(OrganizationPhysicalAddressMigrationJob).FullName!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();

                var organizations = await repositoryFactory.OrganizationRepository.GetAllAsync(cancellationToken);
                foreach (var organization in organizations)
                {
                    if (organization.Address is null)
                    {
                        continue;
                    }

                    if (organization.PhysicalAddress is not null)
                    {
                        continue;
                    }

                    repositoryFactory.OrganizationPhysicalAddressRepository.Add(new OrganizationPhysicalAddress
                    {
                        Id = randomHelper.Generate(),
                        Latitude = organization.Address.Latitude,
                        Longitude = organization.Address.Longitude,
                        AddressLine1 = organization.Address.AddressLine1,
                        AddressLine2 = organization.Address.AddressLine2,
                        Suburb = organization.Address.Suburb,
                        City = organization.Address.City,
                        Province = organization.Address.Province,
                        Zipcode = organization.Address.Zipcode,
                        Country = organization.Address.Country,
                        Organization = organization
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
