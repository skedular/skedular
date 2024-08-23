using Api.Shared.Services.Grpc.UnityHub.Customer.V1;
using Api.Shared.Services.Grpc.UnityHub.Location.V1;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Time;
using Organization.Shared.Configurations;
using VersionInput = Api.Shared.Services.Grpc.UnityHub.Customer.V1.VersionInput;

namespace Organization.Api.Jobs;

public class ConnectionKeepAliveJob(
    IServiceProvider serviceProvider,
    ILogger<ConnectionKeepAliveJob> logger,
    ITimeHelper timeHelper) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await timeHelper.RandomSleepWhileStartingUpAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var customerConfiguration = scope.ServiceProvider.GetRequiredService<CustomerConfiguration>();
                var customerServiceClient =
                    scope.ServiceProvider.GetRequiredService<CustomerService.CustomerServiceClient>();

                var locationConfiguration = scope.ServiceProvider.GetRequiredService<LocationConfiguration>();
                var locationServiceClient =
                    scope.ServiceProvider.GetRequiredService<LocationService.LocationServiceClient>();

                await customerServiceClient.GetVersionAsync(
                    new VersionInput(),
                    customerConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken);

                await locationServiceClient.GetVersionAsync(
                    new global::Api.Shared.Services.Grpc.UnityHub.Location.V1.VersionInput(),
                    locationConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken);

                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", nameof(ConnectionKeepAliveJob));
            }
        } while (true);
    }
}
