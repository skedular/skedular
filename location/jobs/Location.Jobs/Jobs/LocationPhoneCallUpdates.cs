using Api.Shared.Services;
using Location.Shared.Repositories;

namespace Location.Jobs.Jobs;

public class LocationPhoneCallUpdates(IServiceProvider serviceProvider, ILogger<LocationPhoneCallUpdates> logger)
    : BackgroundService
{
    private readonly string _jobName = typeof(LocationPhoneCallUpdates).FullName!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var locations = await repositoryFactory.LocationRepository.GetAllAsync(false, cancellationToken);

                foreach (var location in locations
                             .Where(item => item.Organization.UniqueAlphanumericName == Constants.SkedularPublicLocationsUniqueAlphanumericName))
                {
                    if (location.ExtraMetadata?.ContactDetails?.ContactPhones is null)
                    {
                        continue;
                    }

                    location.ExtraMetadata = location.ExtraMetadata with
                    {
                        ContactDetails = location.ExtraMetadata.ContactDetails with
                        {
                            ContactPhones = location.ExtraMetadata.ContactDetails.ContactPhones
                                .Select(item =>
                                    !item.StartsWith("CALL ", StringComparison.InvariantCultureIgnoreCase) ? item : item["CALL ".Length..])
                                .ToList()
                        }
                    };

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
